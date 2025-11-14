using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities;
using static MAVLink;

namespace MissionPlanner.ArduPilot
{
    public sealed class MissionNode
    {
        public int MissionIndex { get; }
        public Locationwp Command { get; }
        public bool CanContinue { get; internal set; } = true;
        public List<int> IncomingEdges { get; } = new List<int>();
        public List<int> OutgoingEdges { get; } = new List<int>();

        public MissionNode(int missionIndex, Locationwp command)
        {
            MissionIndex = missionIndex;
            Command = command;
        }
    }

    public sealed class MissionEdge
    {
        public int FromNode { get; }
        public int ToNode { get; }
        public bool IsJump { get; }
        public int? JumpRepeat { get; }

        public MissionEdge(int fromNode, int toNode, bool isJump, int? jumpRepeat = null)
        {
            FromNode = fromNode;
            ToNode = toNode;
            IsJump = isJump;
            JumpRepeat = jumpRepeat;
        }

        public override string ToString()
        {
            return $"{(IsJump ? "Jump" : "Seq")}: {FromNode} -> {ToNode}";
        }
    }

    public sealed class MissionGraph
    {
        public IReadOnlyList<MissionNode> Nodes { get; }
        public IReadOnlyList<MissionEdge> Edges { get; }

        public MissionGraph(List<MissionNode> nodes,
                            List<MissionEdge> edges)
        {
            Nodes = nodes;
            Edges = edges;
        }
    }

    public class MissionGraphBuilder
    {
        public static MissionGraph Build(PointLatLngAlt home, List<Locationwp> missionitems)
        {
            var nodes = new List<MissionNode>();
            var edges = new List<MissionEdge>();

            // Map from mission index (0-based) to node index. -1 means not a node.
            var missionToNode = new int[missionitems.Count];
            for (int i = 0; i < missionToNode.Length; i++)
            {
                missionToNode[i] = -1;
            }

            var jumpTags = new Dictionary<float, int>(); // key=tag, value=mission index

            if (home != PointLatLngAlt.Zero)
            {

                var homeCopy = new Locationwp
                {
                    id = 0,
                    lat = home.Lat,
                    lng = home.Lng,
                    alt = (float)home.Alt,
                    frame = (byte)MAVLink.MAV_FRAME.GLOBAL,
                };
                nodes.Add(new MissionNode(-1, homeCopy));
            }

            for (int i = 0; i < missionitems.Count; i++)
            {
                var cmd = missionitems[i];
                if (IsNode(cmd))
                {
                    var node = new MissionNode(i, cmd)
                    {
                        CanContinue = !IsTerminal(cmd.id)
                    };
                    nodes.Add(node);
                    missionToNode[i] = nodes.Count - 1;
                }
                if (cmd.id == (ushort)MAV_CMD.JUMP_TAG)
                {
                    jumpTags[cmd.p1] = i;
                }
                if ((cmd.id == (ushort)MAV_CMD.DO_JUMP || cmd.id == (ushort)MAV_CMD.DO_JUMP_TAG)
                    && cmd.p2 < 0 && nodes.Count > 0)
                {
                    nodes[nodes.Count - 1].CanContinue = false;
                }
            }

            // Sequential edges over nodes
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                if (nodes[i].CanContinue)
                {
                    edges.Add(new MissionEdge(i, i + 1, false));
                    nodes[i].OutgoingEdges.Add(edges.Count - 1);
                    nodes[i + 1].IncomingEdges.Add(edges.Count - 1);
                }
            }

            // Jump edges
            for (int i = 0; i < missionitems.Count; i++)
            {
                var item = missionitems[i];
                int jumpTargetMissionIndex;
                if (item.id == (ushort)MAV_CMD.DO_JUMP)
                {
                    jumpTargetMissionIndex = (int)item.p1 - 1; // p1 is 1-based seq
                }
                else if (item.id == (ushort)MAV_CMD.DO_JUMP_TAG)
                {
                    float tag = item.p1;
                    if (!jumpTags.TryGetValue(tag, out jumpTargetMissionIndex))
                        continue;   // invalid tag
                }
                else
                {
                    continue; // not a jump
                }

                if (jumpTargetMissionIndex < 0 || jumpTargetMissionIndex >= missionitems.Count)
                {
                    continue;   // invalid target
                }

                int srcNodeIndex = FindLastNodeBeforeOrAt(i, missionToNode);
                if (srcNodeIndex < 0 || srcNodeIndex >= nodes.Count)
                {
                    continue;   // source is not a node
                }

                int destNodeIndex = FindFirstNodeAtOrAfter(jumpTargetMissionIndex, missionToNode);
                if (destNodeIndex < 0 || destNodeIndex >= nodes.Count)
                {
                    continue;   // target is not a node
                }

                int jumpRepeat = (int)item.p2;

                edges.Add(new MissionEdge(
                    srcNodeIndex,
                    destNodeIndex,
                    true,
                    jumpRepeat));
                nodes[srcNodeIndex].OutgoingEdges.Add(edges.Count - 1);
                nodes[destNodeIndex].IncomingEdges.Add(edges.Count - 1);
            }

            return new MissionGraph(nodes, edges);
        }

        static int FindLastNodeBeforeOrAt(int missionIndex, int[] missionToNode)
        {
            if (missionToNode == null || missionToNode.Length == 0 || missionIndex < 0)
            {
                return -1;
            }

            if (missionIndex >= missionToNode.Length)
            {
                missionIndex = missionToNode.Length - 1;
            }

            for (int i = missionIndex; i >= 0; i--)
            {
                var nodeIndex = missionToNode[i];
                if (nodeIndex != -1)
                {
                    return nodeIndex;
                }
            }

            return -1;
        }

        static int FindFirstNodeAtOrAfter(int missionIndex, int[] missionToNode)
        {
            if (missionToNode == null || missionToNode.Length == 0 || missionIndex >= missionToNode.Length)
            {
                return -1;
            }

            if (missionIndex < 0)
            {
                missionIndex = 0;
            }

            for (int i = missionIndex; i < missionToNode.Length; i++)
            {
                var nodeIndex = missionToNode[i];
                if (nodeIndex != -1)
                {
                    return nodeIndex;
                }
            }

            return -1;
        }

        /// <summary>
        /// Whether a command is a connected point in a route (like nav commands in missions, or polygon fence points)
        /// </summary>
        static bool IsNode(Locationwp locationwp)
        {
            var command = locationwp.id;

            // Despite not being NAV commands, these fence polygon commands are connected nodes
            if (command == (ushort)MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION ||
                command == (ushort)MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION)
            {
                return true;
            }

            // The obsolete "ROI" command is in the "NAV" range, but is not actually a NAV command
            if (command == (ushort)MAV_CMD.ROI ||
                command == (ushort)MAV_CMD.DO_SET_ROI_LOCATION)
            {
                return false;
            }

            // These commands terminate the flight, so even though some do not have a location, they get a node
            if (IsTerminal(command))
            {
                return true;
            }

            // Otherwise, anything in the NAV command range that has a location is a connected node
            if (command < (ushort)MAV_CMD.LAST)
            {
                return HasLocation(locationwp);
            }

            return false;
        }

        static bool IsTerminal(ushort cmd)
        {
            switch (cmd)
            {
            case (ushort)MAV_CMD.LAND:
            case (ushort)MAV_CMD.VTOL_LAND:
            case (ushort)MAV_CMD.LAND_LOCAL:
            case (ushort)MAV_CMD.DO_RALLY_LAND:
            case (ushort)MAV_CMD.RETURN_TO_LAUNCH:
            case (ushort)MAV_CMD.DO_FLIGHTTERMINATION:
                return true;
            default:
                return false;
            }
        }


        /// <summary>
        /// Check if a MAV_CMD enum entry has the hasLocation attribute
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        static bool HasLocation(Locationwp locationwp)
        {
            var command = locationwp.id;
            bool has_lat_lon = !(locationwp.lat == 0 && locationwp.lng == 0) &&
                   !double.IsNaN(locationwp.lat) &&
                   !double.IsNaN(locationwp.lng);
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
    }
}
