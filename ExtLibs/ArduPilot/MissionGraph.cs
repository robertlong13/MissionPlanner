using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities;
using static MAVLink;

namespace MissionPlanner.ArduPilot
{
    public enum MissionNodeKind
    {
        Home,
        Nav,         // WAYPOINT, SPLINE_WAYPOINT, LAND, TAKEOFF, VTOL_*, etc
        Circle,      // LOITER_TURNS, LOITER_TO_ALT, etc
        Roi,         // DO_SET_ROI
        PosBookmark, // DO_LAND_START
        FencePolygon,
        FenceCircle,
        Rally,
        OtherPositional,
        NonPositional, // DO_DIGICAM_CONTROL, DO_SET_SERVO, etc
    }

    public enum MissionEdgeKind
    {
        Sequential,
        Jump,
    }

    public sealed class MissionNode
    {
        public int NodeIndex { get; }
        public int MissionIndex { get; }     // -1 for home
        public MissionNodeKind Kind { get; }
        public ushort Command { get; }
        public PointLatLngAlt Position { get; }

        public MissionNode(int missionIndex,
                           MissionNodeKind kind,
                           ushort command,
                           PointLatLngAlt position)
        {
            MissionIndex = missionIndex;
            Kind = kind;
            Command = command;
            Position = position;
        }

        public override string ToString()
        {
            return $"{NodeIndex}: [{Kind}] seq={MissionIndex + 1} cmd={(MAVLink.MAV_CMD)Command}";
        }
    }

    public sealed class MissionEdge
    {
        public int FromNode { get; }
        public int ToNode { get; }
        public MissionEdgeKind Kind { get; }
        public int? JumpTargetSeq { get; }   // 1-based mission seq for DO_JUMP, if applicable
        public int? JumpRepeat { get; }      // repeat count from DO_JUMP

        public MissionEdge(int fromNode,
                           int toNode,
                           MissionEdgeKind kind,
                           int? jumpTargetSeq = null,
                           int? jumpRepeat = null)
        {
            FromNode = fromNode;
            ToNode = toNode;
            Kind = kind;
            JumpTargetSeq = jumpTargetSeq;
            JumpRepeat = jumpRepeat;
        }

        public override string ToString()
        {
            return $"{Kind}: {FromNode} -> {ToNode}";
        }
    }

    public sealed class MissionGraph
    {
        public IReadOnlyList<MissionNode> Nodes { get; }
        public IReadOnlyList<MissionEdge> Edges { get; }
        public int HomeNodeIndex { get; }

        public MissionGraph(List<MissionNode> nodes,
                            List<MissionEdge> edges,
                            int homeNodeIndex)
        {
            Nodes = nodes;
            Edges = edges;
            HomeNodeIndex = homeNodeIndex;
        }
    }

    public class MissionGraphBuilder
    {
        public static MissionGraph Build(PointLatLngAlt home, List<Locationwp> missionitems)
        {
            var navNodes = new List<MissionNode>();
            var navEdges = new List<MissionEdge>();
            var fenceNodes = new List<MissionNode>();
            var fenceEdges = new List<MissionEdge>();

            // Map from mission index (0-based) to node index, or -1
            var missionToNode = new int[missionitems.Count];
            for (int i = 0; i < missionToNode.Length; i++)
            {
                missionToNode[i] = -1;
            }

            var homeCopy = new PointLatLngAlt(home.Lat, home.Lng, home.Alt, "H");
            var node = new MissionNode(-1,
                                       MissionNodeKind.Home,
                                       0,  // no MAV_CMD
                                       homeCopy);
            navNodes.Add(node);

            // Mission nodes
            for (int i = 0; i < missionitems.Count; i++)
            {
                var cmd = missionitems[i].id;
                bool has_lat_lon = !(missionitems[i].lat == 0 && missionitems[i].lng == 0) &&
                                   !double.IsNaN(missionitems[i].lat) &&
                                   !double.IsNaN(missionitems[i].lng);
                var kind = ClassifyNodeKind(cmd, has_lat_lon);
                var position = new PointLatLngAlt(missionitems[i]) { Tag = (i + 1).ToString() };
                node = new MissionNode(i, kind, cmd, position);
            }

            // Sequential edges over nav nodes (plus optional edge from home to first nav)
            for (int i = 0; i < missionitems.Count; i++)
            {
                var item = missionitems[i];
                ushort cmd = item.id;
                int nodeIndex = missionToNode[i];

                if (nodeIndex == -1)
                    continue;

                if (IsNavCommand(cmd))
                {
                    if (lastNavNode != -1)
                    {
                        edges.Add(new MissionEdge(lastNavNode, nodeIndex, MissionEdgeKind.Sequential));
                    }
                    lastNavNode = nodeIndex;
                }
                else
                {
                    // ROI, fence, rally etc are not on the nav path;
                    // they stand alone and may get markers but no sequential leg.
                }
            }

            // Jump edges: from last nav before DO_JUMP to first nav at/after target
            for (int i = 0; i < missionitems.Count; i++)
            {
                var item = missionitems[i];
                if (item.id != (ushort)MAVLink.MAV_CMD.DO_JUMP)
                    continue;

                int jumpTargetSeq = (int)Math.Max(item.p1, 0);   // this is 1-based in missions
                int jumpRepeat = (int)item.p2;

                int srcNavNode = FindLastNavNodeBefore(i, missionitems, missionToNode);
                if (srcNavNode == -1)
                    continue;

                int dstNavNode = FindFirstNavNodeAtOrAfter(jumpTargetSeq - 1, missionitems, missionToNode);
                if (dstNavNode == -1)
                    continue;

                edges.Add(new MissionEdge(
                    srcNavNode,
                    dstNavNode,
                    MissionEdgeKind.Jump,
                    jumpTargetSeq,
                    jumpRepeat));
            }

            return new MissionGraph(nodes, edges, homeNodeIndex);
        }

        static MissionNodeKind ClassifyNodeKind(ushort cmd, bool has_lat_lon)
        {
            switch (cmd)
            {
                case (ushort)MAVLink.MAV_CMD.LOITER_TURNS:
                case (ushort)MAVLink.MAV_CMD.LOITER_TIME:
                case (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT:
                case (ushort)MAVLink.MAV_CMD.LOITER_UNLIM:
                    return MissionNodeKind.Circle;

                case (ushort)MAVLink.MAV_CMD.ROI:
                case (ushort)MAVLink.MAV_CMD.DO_SET_ROI_LOCATION:
                    return MissionNodeKind.Roi;

                case (ushort)MAVLink.MAV_CMD.DO_LAND_START:
                    return MissionNodeKind.PosBookmark;

                case (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION:
                case (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION:
                    return MissionNodeKind.FencePolygon;

                case (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION:
                case (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION:
                    return MissionNodeKind.FenceCircle;

                case (ushort)MAVLink.MAV_CMD.RALLY_POINT:
                    return MissionNodeKind.Rally;

                default:
                    if (IsNavCommand(cmd, has_lat_lon))
                        return MissionNodeKind.Nav;
                    else if (has_lat_lon)
                        return MissionNodeKind.OtherPositional;
                    else
                        return MissionNodeKind.NonPositional;
            }
        }

        static bool IsNavCommand(ushort command, bool has_lat_lon)
        {
            if (command >= (ushort)MAVLink.MAV_CMD.LAST)
                return false;

            if (command == (ushort)MAVLink.MAV_CMD.ROI)
                return false;

            return HasLocation(command, has_lat_lon);
        }


        /// <summary>
        /// Check if a MAV_CMD enum entry has the hasLocation attribute
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        static bool HasLocation(ushort command, bool has_lat_lon)
        {
            if (command >= (ushort)MAVLink.MAV_CMD.LAST)
                return false;

            var mavCmdType = typeof(MAVLink.MAV_CMD);

            if (!Enum.IsDefined(mavCmdType, command))
                return has_lat_lon; // unknown, assume positional if lat/lon present

            var name = ((MAVLink.MAV_CMD)command).ToString();
            var memberInfo = mavCmdType.GetMember(name).FirstOrDefault();
            if (memberInfo == null)
                return has_lat_lon; // should not be possible

            return memberInfo
                .GetCustomAttributes(false)
                .OfType<hasLocation>()
                .Any();
        }

        static int FindLastNavNodeBefore(int missionIndex,
                                         List<Locationwp> missionitems,
                                         int[] missionToNode)
        {
            for (int i = missionIndex - 1; i >= 0; i--)
            {
                ushort cmd = missionitems[i].id;
                if (!IsNavCommand(cmd))
                    continue;
                int nodeIndex = missionToNode[i];
                if (nodeIndex != -1)
                    return nodeIndex;
            }
            return -1;
        }

        static int FindFirstNavNodeAtOrAfter(int missionIndex,
                                             List<Locationwp> missionitems,
                                             int[] missionToNode)
        {
            for (int i = Math.Max(missionIndex, 0); i < missionitems.Count; i++)
            {
                ushort cmd = missionitems[i].id;
                if (!IsNavCommand(cmd))
                    continue;
                int nodeIndex = missionToNode[i];
                if (nodeIndex != -1)
                    return nodeIndex;
            }
            return -1;
        }
    }
}
