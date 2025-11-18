using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities.Mission;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities
{
    [Flags]
    public enum SegmentFlags
    {
        None = 0,
        Alternate = 1 << 0, // All jumps and non-jump (spline) segments whose shape depends on a jump segment
        Jump = 1 << 1,
        UnknownCommand = 1 << 2,
        ToHome = 1 << 3,
        FromTakeoff = 1 << 4,
    }

    public class MissionSegmentizer
    {
        public enum SegmentKind
        {
            Straight,
            Spline,
            LoiterArc,
            ArcTurn,
        }

        public sealed class Segment
        {
            public SegmentKind Kind;
            public SegmentFlags Flags;
            public MissionNode StartNode;
            public MissionNode EndNode;         // null for things like loiter arcs
            public List<PointLatLngAlt> Path;
            public PointLatLngAlt Midpoint;
        }

        sealed class LoiterInfo
        {
            public MissionNode Node;
            public PointLatLngAlt Center;
            public PointLatLngAlt EntryPoint;
            public PointLatLngAlt ExitPoint;
            public double Radius;
            public bool IsClockwise;
        }

        static public List<Segment> BuildSegments(MissionGraph graph, VehicleClass vehicleClass, double loiterRadius)
        {
            var segments = new List<Segment>();
            var loiterInfoDict = new Dictionary<int, LoiterInfo>();
            foreach (var edge in graph.Edges)
            {
                var a = edge.FromNode;
                var b = edge.ToNode;

                if (a == null || b == null || !HasLatLon(a.Command) || !HasLatLon(b.Command))
                {
                    continue;
                }

                PointLatLngAlt overrideSrcPos = null;
                PointLatLngAlt overrideDestPos = null;
                if (NeedsLoiterExit(a, vehicleClass))
                {
                    var loiterInfo = EnsureLoiterInfo(ref loiterInfoDict, a, loiterRadius);
                    bool crosstrackTangent = LoiterXTrackTangent(a.Command);

                    var dest = new PointLatLngAlt(b.Command);
                    var distance = loiterInfo.Center.GetDistance2(dest);
                    var bearing = (distance > 1) ? loiterInfo.Center.GetBearing(dest) : 0.0;

                    if (crosstrackTangent)
                    {
                        double bearingChange;
                        if (distance < loiterInfo.Radius)
                        {
                            bearingChange = 0;
                        }
                        else
                        {
                            bearingChange = Math.Acos(loiterInfo.Radius / distance) * (180.0 / Math.PI);
                        }
                        if (loiterInfo.IsClockwise)
                        {
                            bearingChange = -bearingChange;
                        }
                        bearing += bearingChange;
                    }
                    loiterInfo.ExitPoint = loiterInfo.Center.newpos(bearing, loiterInfo.Radius);
                    overrideSrcPos = loiterInfo.ExitPoint;
                }
                double captureDistance = 0;
                if (NeedsLoiterCapture(b, vehicleClass))
                {
                    captureDistance = EnsureLoiterInfo(ref loiterInfoDict, b, loiterRadius).Radius;
                }

                SegmentFlags flags = SegmentFlags.None;
                if(!TryGetSegmentKind(a, b, vehicleClass, out SegmentKind kind))
                {
                    flags |= SegmentFlags.UnknownCommand;
                }
                if (edge.IsJump)
                {
                    flags |= SegmentFlags.Jump | SegmentFlags.Alternate;
                }
                if (IsTakeoff(a.Command))
                {
                    flags |= SegmentFlags.FromTakeoff;
                }

                switch (kind)
                {
                    case SegmentKind.Straight:
                    default:
                        segments.Add(GenerateStraightSegment(graph, edge, flags, overrideSrcPos, captureDistance, out overrideDestPos));
                        break;
                    case SegmentKind.Spline:
                        segments.AddRange(GenerateSplineSegments(graph, edge, flags, overrideSrcPos));
                        break;
                    case SegmentKind.ArcTurn:
                        // TODO: generate arc turn segment
                        segments.Add(GenerateStraightSegment(graph, edge, flags, overrideSrcPos, captureDistance, out overrideDestPos));
                        break;
                }

                if (!edge.IsJump && overrideDestPos != null && loiterInfoDict.ContainsKey(edge.ToNode.MissionIndex))
                {
                    loiterInfoDict[edge.ToNode.MissionIndex].EntryPoint = overrideDestPos;
                }
            }

            foreach (var loiterInfo in loiterInfoDict.Values)
            {
                if (loiterInfo.EntryPoint != null && loiterInfo.ExitPoint != null)
                {
                    segments.Add(GenerateLoiterArcSegment(loiterInfo));
                }
            }

            return segments;
        }

        static bool TryGetSegmentKind(MissionNode src, MissionNode dest, VehicleClass vehicleClass, out SegmentKind kind)
        {
            switch (dest.Command.id)
            {
                case (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT:
                    kind = SegmentKind.Spline;
                    return true;
                case (ushort)MAVLink.MAV_CMD.WAYPOINT:
                case (ushort)MAVLink.MAV_CMD.LOITER_UNLIM:
                case (ushort)MAVLink.MAV_CMD.LOITER_TURNS:
                case (ushort)MAVLink.MAV_CMD.LOITER_TIME:
                case (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT:
                case (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH:
                case (ushort)MAVLink.MAV_CMD.LAND:
                case (ushort)MAVLink.MAV_CMD.LAND_LOCAL:
                case (ushort)MAVLink.MAV_CMD.VTOL_LAND:
                case (ushort)MAVLink.MAV_CMD.PAYLOAD_PLACE:
                    kind = SegmentKind.Straight;
                    return true;
                case (ushort)36: // arc turn, not in MAVLink enum yet
                    kind = SegmentKind.ArcTurn;
                    return true;
                default:
                    kind = SegmentKind.Straight;
                    return false;
            }
        }

        static bool IsToHomeSegment(MissionEdge edge, MissionGraph graph)
        {
            return edge.ToNode.Command.id == (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH;
        }

        static Segment GenerateStraightSegment(MissionGraph graph, MissionEdge edge, SegmentFlags flags, PointLatLngAlt overrideSrcPos, double captureDistance, out PointLatLngAlt overrideDestPos)
        {
            var src = edge.FromNode;
            var dest = edge.ToNode;
            var srcPos = overrideSrcPos ?? new PointLatLngAlt(src.Command);
            var distance = srcPos.GetDistance2(new PointLatLngAlt(dest.Command));
            var bearing = (distance > 1e-6) ? srcPos.GetBearing(new PointLatLngAlt(dest.Command)) : 0;
            var destPos = new PointLatLngAlt(dest.Command);
            if (IsToHomeSegment(edge, graph))
            {
                flags |= SegmentFlags.ToHome;
                //destPos 
            }
            if (captureDistance > 0)
            {
                // Shorten the segment to end at capture distance from dest
                // (this also works if src is inside the capture distance; it turns around and extends out)
                overrideDestPos = srcPos.newpos(bearing, distance - captureDistance);
                distance = srcPos.GetDistance2(overrideDestPos);
                destPos = overrideDestPos;
            }
            else
            {
                overrideDestPos = null;
            }
            return new Segment
            {
                Kind = SegmentKind.Straight,
                Flags = flags,
                StartNode = src,
                EndNode = dest,
                Path = new List<PointLatLngAlt>
                {
                    srcPos,
                    destPos
                },
                Midpoint = srcPos.newpos(bearing, distance / 2.0),
            };
        }

        static List<Segment> GenerateSplineSegments(MissionGraph graph, MissionEdge edge, SegmentFlags flags, PointLatLngAlt overrideSrcPos)
        {
            var src = edge.FromNode;
            var dest = edge.ToNode;
            var srcPos = overrideSrcPos ?? new PointLatLngAlt(src.Command);
            var segments = new List<Segment>();

            var IncomingEdges = src.IncomingEdges;
            if (IncomingEdges.Count == 0)
            {
                IncomingEdges = new List<MissionEdge> { null };
            }
            var OutgoingEdges = src.OutgoingEdges;
            if (OutgoingEdges.Count == 0)
            {
                OutgoingEdges = new List<MissionEdge> { null };
            }


            foreach (var inEdge in IncomingEdges)
            {
                var prev = inEdge?.FromNode;
                var startType = SplineEndpointType.Stop;
                // This isn't perfect. If the previous command has no lat/lon, it takes the current
                // position of the vehicle whenever this command is loaded. That is hard to handle
                // (particularly with jumps) and is rare enough to be impractical to implement.
                if (prev != null && HasLatLon(prev.Command))
                {
                    if (IsSplineWP(src.Command))
                    {
                        startType = SplineEndpointType.Spline;
                    }
                    else if (!IsSplineStoppedCopter(src.Command))
                    {
                        startType = SplineEndpointType.Straight;
                    }
                }
                foreach (var outEdge in OutgoingEdges)
                {
                    var next = outEdge?.ToNode;
                    var endType = SplineEndpointType.Stop;
                    if (next != null && !dest.IsTerminal && HasLatLon(next.Command))
                    {
                        if (IsSplineWP(next.Command))
                        {
                            endType = SplineEndpointType.Spline;
                        }
                        else if (!IsSplineStoppedCopter(dest.Command))
                        {
                            endType = SplineEndpointType.Straight;
                        }
                    }
                    var prevPos = prev != null ? new PointLatLngAlt(prev.Command) : null;
                    var nextPos = next != null ? new PointLatLngAlt(next.Command) : null;
                    var splineGenerator = new Spline3(
                        prevPos,
                        srcPos,
                        new PointLatLngAlt(dest.Command),
                        nextPos,
                        startType,
                        endType
                    );
                    var path = splineGenerator.BuildPath();
                    var midpoint = splineGenerator.GetPointAt(0.5);
                    segments.Add(
                        new Segment
                        {
                            Kind = SegmentKind.Spline,
                            Flags = flags,
                            StartNode = src,
                            EndNode = dest,
                            Path = path,
                            Midpoint = midpoint,
                        }
                    );

                    // Every segment generated after this will be non-primary
                    flags |= SegmentFlags.Alternate;

                    if (endType == SplineEndpointType.Stop)
                    {
                        // Changine the next waypoint won't change the shape
                        break;
                    }
                }
                if (startType == SplineEndpointType.Stop)
                {
                    // Changing the previous waypoint won't change the shape
                    break;
                }
            }
            return segments;
        }

        static Segment GenerateLoiterArcSegment(LoiterInfo loiterInfo, int samples = 20)
        {
            var bearing1 = loiterInfo.Center.GetBearing(loiterInfo.EntryPoint);
            var bearing2 = loiterInfo.Center.GetBearing(loiterInfo.ExitPoint);
            while (loiterInfo.IsClockwise && bearing2 <= (bearing1 + 15))
            {
                bearing2 += 360.0;
            }
            while (!loiterInfo.IsClockwise && bearing2 >= (bearing1 - 15))
            {
                bearing2 -= 360.0;
            }
            var path = new List<PointLatLngAlt>();
            for (int i = 0; i <= samples; i++)
            {
                double bearing = bearing1 + (bearing2 - bearing1) * (i / (double)samples);
                var point = loiterInfo.Center.newpos(bearing, loiterInfo.Radius);
                path.Add(point);
            }

            return new Segment()
            {
                Kind = SegmentKind.LoiterArc,
                Flags = SegmentFlags.None,
                StartNode = loiterInfo.Node,
                EndNode = null,
                Path = path,
                Midpoint = loiterInfo.Center.newpos((bearing1 + bearing2) / 2.0, loiterInfo.Radius),
            };
        }

        static LoiterInfo EnsureLoiterInfo(ref Dictionary<int, LoiterInfo> loiterInfoDict, MissionNode node, double defaultLoiterRadius)
        {
            if (!loiterInfoDict.ContainsKey(node.MissionIndex))
            {
                var radius = LoiterRadius(node.Command, defaultLoiterRadius);
                bool isClockwise = radius >= 0;
                radius = Math.Abs(radius);
                loiterInfoDict.Add(
                    node.MissionIndex,
                    new LoiterInfo()
                    {
                        Node = node,
                        Center = new PointLatLngAlt(node.Command),
                        Radius = radius,
                        IsClockwise = isClockwise,
                    }
                );
            }
            return loiterInfoDict[node.MissionIndex];
        }
    }
}
