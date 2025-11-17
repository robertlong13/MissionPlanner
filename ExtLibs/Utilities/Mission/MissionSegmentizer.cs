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
            public MissionNode EndNode;           // Equal to StartNode for synthetic segments (like loiter arcs)
            public List<PointLatLngAlt> Path;     // Always nonempty
        }

        static public List<Segment> BuildSegments(MissionGraph graph, VehicleClass vehicleClass, double loiterRadius)
        {
            var segments = new List<Segment>();
            PointLatLngAlt loiterCapturePoint = null;
            foreach (var edge in graph.Edges)
            {
                var a = graph.Nodes[edge.FromNode];
                var b = graph.Nodes[edge.ToNode];
                PointLatLngAlt overrideSrcPos = null;
                if (NeedsLoiterExit(a, vehicleClass))
                {
                    var center = new PointLatLngAlt(a.Command);
                    var radius = LoiterRadius(a.Command, loiterRadius);
                    bool isClockwise = radius >= 0;
                    radius = Math.Abs(radius);
                    bool crosstrackTangent = LoiterXTrackTangent(a.Command);

                    var dest = new PointLatLngAlt(b.Command);
                    var distance = center.GetDistance2(dest);
                    var bearing = (distance > 1) ? center.GetBearing(dest) : 0.0;

                    if (crosstrackTangent)
                    {
                        double bearingChange;
                        if (distance < radius)
                        {
                            bearingChange = 0;
                        }
                        else
                        {
                            bearingChange = Math.Acos(radius / distance) * (180.0 / Math.PI);
                        }
                        if (isClockwise)
                        {
                            bearingChange = -bearingChange;
                        }
                        bearing += bearingChange;
                    }
                    overrideSrcPos = center.newpos(bearing, radius);

                    if (loiterCapturePoint != null)
                    {
                        segments.Add(
                            new Segment
                            {
                                Kind = SegmentKind.LoiterArc,
                                IsPrimary = true,
                                StartNode = a,
                                Path = GenerateLoiterArcPath(
                                    center,
                                    loiterCapturePoint,
                                    overrideSrcPos,
                                    radius,
                                    isClockwise
                                )
                            }
                        );
                    }

                    loiterCapturePoint = null;
                }

                SegmentKind kind = GetSegmentKind(a, b, vehicleClass);

                switch (kind)
                {
                    case SegmentKind.Straight:
                    case SegmentKind.Unknown:
                    default:
                        segments.Add(GenerateStraightSegment(graph, edge, overrideSrcPos));
                        break;
                    case SegmentKind.Spline:
                        segments.AddRange(GenerateSplineSegments(graph, edge, overrideSrcPos));
                        break;
                    case SegmentKind.ArcTurn:
                        // TODO: generate arc turn segment
                        segments.Add(GenerateStraightSegment(graph, edge, overrideSrcPos));
                        break;
                }

                if (NeedsLoiterCapture(b, vehicleClass))
                {
                    var center = new PointLatLngAlt(b.Command);
                    var radius = Math.Abs(LoiterRadius(b.Command, loiterRadius));

                    // Pop points off the end of the path, and find a line segment that intersects the loiter circle
                    // to modify the end point to be on the loiter circle.
                    var path = segments[segments.Count - 1].Path;
                    for (int i = path.Count - 2; i >= 0; i--)
                    {
                        var p1 = path[i];
                        var p2 = path[i + 1];
                        if (p1.GetDistance2(center) < radius)
                        {
                            path.RemoveAt(i + 1);
                            continue;
                        }
                        var distToCenter = p2.GetDistance2(center);
                        var moveDirection = p2.GetBearing(p1);
                        path[path.Count - 1] = p2.newpos(
                            moveDirection,
                            radius - distToCenter
                        );
                    }
                    // If path has only 1 element, it means the entire segment is within the loiter circle.
                    // Draw a segment to the nearest point on the loiter circle.
                    if (path.Count == 1)
                    {
                        var moveDirection = center.GetBearing(path[0]);
                        path.Add(center.newpos(moveDirection, radius));
                    }
                    loiterCapturePoint = path[path.Count - 1];
                }
                else
                {
                    loiterCapturePoint = null;
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

        static Segment GenerateStraightSegment(MissionGraph graph, MissionEdge edge, PointLatLngAlt overrideSrcPos)
        {
            var src = graph.Nodes[edge.FromNode];
            var dest = graph.Nodes[edge.ToNode];
            var srcPos = overrideSrcPos ?? new PointLatLngAlt(src.Command);
            var segment = new Segment
            {
                Kind = SegmentKind.Straight,
                IsPrimary = !edge.IsJump,
                StartNode = src,
                EndNode = dest,
                Path = new List<PointLatLngAlt>
                    {
                        srcPos,
                        new PointLatLngAlt(dest.Command)
                    }
            };
            return segment;
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
                    var path = Spline3.BuildSegment(
                        prevPos,
                        srcPos,
                        new PointLatLngAlt(dest.Command),
                        nextPos,
                        startType,
                        endType
                    );
                    segments.Add(
                        new Segment
                        {
                            Kind = SegmentKind.Spline,
                            IsPrimary = isPrimary,
                            StartNode = src,
                            EndNode = dest,
                            Path = path,
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

        static List<PointLatLngAlt> GenerateLoiterArcPath(PointLatLngAlt center, PointLatLngAlt entryPoint, PointLatLngAlt exitPoint, double radius, bool isClockwise, int samples = 20)
        {
            var bearing1 = center.GetBearing(entryPoint);
            var bearing2 = center.GetBearing(exitPoint);
            while (isClockwise && bearing2 <= (bearing1 - 15))
            {
                bearing2 += 360.0;
            }
            while (!isClockwise && bearing2 >= (bearing1 - 15))
            {
                bearing2 -= 360.0;
            }
            var path = new List<PointLatLngAlt>();
            for (int i = 0; i <= samples; i++)
            {
                double bearing = bearing1 + (bearing2 - bearing1) * (i / (double)samples);
                var point = center.newpos(bearing, radius);
                path.Add(point);
            }
            return path;
        }
    }
}
