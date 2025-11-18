using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities.Mission;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities
{
    public class MissionSegmentizer
    {
        public enum SegmentKind
        {
            Straight,
            Spline,
            LoiterArc,  // Synthetic arc between entry/exit on circle
            ArcTurn,
            Unknown,    // Same as straght, but might use a different rendering style
        }

        public sealed class Segment
        {
            public SegmentKind Kind;              // Straight, Spline, LoiterArc, ArcTurn, etc.
            public bool IsPrimary;                // Primary or jump/alternate
            public MissionNode StartNode;
            public MissionNode EndNode;           // null for synthetic segments (like loiter arcs)
            public List<PointLatLngAlt> Path;     // Always nonempty
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
                var a = graph.Nodes[edge.FromNode];
                var b = graph.Nodes[edge.ToNode];

                if (a == null || b == null || !HasLatLon(a.Command) || !HasLatLon(b.Command))
                {
                    continue;
                }

                PointLatLngAlt overrideSrcPos = null;
                PointLatLngAlt overrideDestPos = null;
                if (NeedsLoiterExit(a, vehicleClass))
                {
                    var loiterInfo = EnsureLoiterInfo(ref loiterInfoDict, edge.FromNode, a, loiterRadius);
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
                    captureDistance = EnsureLoiterInfo(ref loiterInfoDict, edge.ToNode, b, loiterRadius).Radius;
                }

                SegmentKind kind = GetSegmentKind(a, b, vehicleClass);

                switch (kind)
                {
                    case SegmentKind.Straight:
                    case SegmentKind.Unknown:
                    default:
                        segments.Add(GenerateStraightSegment(graph, edge, overrideSrcPos, captureDistance, out overrideDestPos));
                        break;
                    case SegmentKind.Spline:
                        segments.AddRange(GenerateSplineSegments(graph, edge, overrideSrcPos));
                        break;
                    case SegmentKind.ArcTurn:
                        // TODO: generate arc turn segment
                        segments.Add(GenerateStraightSegment(graph, edge, overrideSrcPos, captureDistance, out overrideDestPos));
                        break;
                }

                if (!edge.IsJump && overrideDestPos != null && loiterInfoDict.ContainsKey(edge.ToNode))
                {
                    loiterInfoDict[edge.ToNode].EntryPoint = overrideDestPos;
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

        static SegmentKind GetSegmentKind(MissionNode src, MissionNode dest, VehicleClass vehicleClass)
        {
            switch (dest.Command.id)
            {
                case (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT:
                    return SegmentKind.Spline;
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
                    return SegmentKind.Straight;
                case (ushort)36: // arc turn, not in MAVLink enum yet
                    return SegmentKind.ArcTurn;
                default:
                    return SegmentKind.Unknown;
            }
        }

        static Segment GenerateStraightSegment(MissionGraph graph, MissionEdge edge, PointLatLngAlt overrideSrcPos, double captureDistance, out PointLatLngAlt overrideDestPos)
        {
            var src = graph.Nodes[edge.FromNode];
            var dest = graph.Nodes[edge.ToNode];
            var srcPos = overrideSrcPos ?? new PointLatLngAlt(src.Command);
            var distance = srcPos.GetDistance2(new PointLatLngAlt(dest.Command));
            var bearing = (distance > 1e-6) ? srcPos.GetBearing(new PointLatLngAlt(dest.Command)) : 0;
            var destPos = new PointLatLngAlt(dest.Command);
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
                IsPrimary = !edge.IsJump,
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

        static List<Segment> GenerateSplineSegments(MissionGraph graph, MissionEdge edge, PointLatLngAlt overrideSrcPos)
        {
            bool isPrimary = !edge.IsJump;
            var src = graph.Nodes[edge.FromNode];
            var dest = graph.Nodes[edge.ToNode];
            var srcPos = overrideSrcPos ?? new PointLatLngAlt(src.Command);
            var segments = new List<Segment>();

            var IncomingEdges = src.IncomingEdges.Select(idx => graph.Edges[idx]).ToList();
            if (IncomingEdges.Count == 0)
            {
                IncomingEdges.Add(null);
            }
            var OutgoingEdges = dest.OutgoingEdges.Select(idx => graph.Edges[idx]).ToList();
            if (OutgoingEdges.Count == 0)
            {
                OutgoingEdges.Add(null);
            }


            foreach (var inEdge in IncomingEdges)
            {
                var prev = inEdge != null ? graph.Nodes[inEdge.FromNode] : null;
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
                    var next = outEdge != null ? graph.Nodes[outEdge.ToNode] : null;
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
                            IsPrimary = isPrimary,
                            StartNode = src,
                            EndNode = dest,
                            Path = path,
                            Midpoint = midpoint,
                        }
                    );

                    // Every segment generated after this will be non-primary
                    isPrimary = false;

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
                IsPrimary = true,
                StartNode = loiterInfo.Node,
                EndNode = null,
                Path = path,
                Midpoint = loiterInfo.Center.newpos((bearing1 + bearing2) / 2.0, loiterInfo.Radius),
            };
        }

        static LoiterInfo EnsureLoiterInfo(ref Dictionary<int, LoiterInfo> loiterInfoDict, int nodeIndex, MissionNode node, double defaultLoiterRadius)
        {
            if (!loiterInfoDict.ContainsKey(nodeIndex))
            {
                var radius = LoiterRadius(node.Command, defaultLoiterRadius);
                bool isClockwise = radius >= 0;
                radius = Math.Abs(radius);
                loiterInfoDict.Add(
                    nodeIndex,
                    new LoiterInfo()
                    {
                        Node = node,
                        Center = new PointLatLngAlt(node.Command),
                        Radius = radius,
                        IsClockwise = isClockwise,
                    }
                );
            }
            return loiterInfoDict[nodeIndex];
        }
    }
}
