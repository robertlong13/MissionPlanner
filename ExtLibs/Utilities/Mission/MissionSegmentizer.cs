using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            public double Distance;
        }

        sealed class LoiterInfo
        {
            public MissionNode Node;
            public PointLatLngAlt Center;
            public double? EntryBearing;
            public double? ExitBearing;
            public double? AltExitBearing;
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

                if (a == null || b == null)
                {
                    continue;
                }

                PointLatLngAlt overrideSrcPos = null;
                PointLatLngAlt overrideDestPos = null;
                if (NeedsLoiterExit(a, vehicleClass))
                {
                    var loiterInfo = EnsureLoiterInfo(ref loiterInfoDict, a, loiterRadius);
                    var exitBearing = GetLoiterExitBearing(a, b, loiterInfo);
                    if (!edge.IsJump)
                    {
                        loiterInfo.ExitBearing = exitBearing;
                    }
                    if (exitBearing.HasValue)
                    {
                        overrideSrcPos = loiterInfo.Center.newpos(exitBearing.Value, loiterInfo.Radius);
                    }
                }
                if (NeedsLoiterCapture(b, vehicleClass))
                {
                    var loiterInfo = EnsureLoiterInfo(ref loiterInfoDict, b, loiterRadius);
                    var entryBearing = GetLoiterEntryBearing(a, loiterInfo, overrideSrcPos);
                    if (!edge.IsJump)
                    {
                        loiterInfo.EntryBearing = entryBearing;
                    }
                    if (entryBearing.HasValue)
                    {
                        overrideDestPos = loiterInfo.Center.newpos(entryBearing.Value, loiterInfo.Radius);
                    }
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
                    overrideSrcPos = GetTakeoffLocation(a, graph.Home);
                    flags |= SegmentFlags.FromTakeoff;
                }
                if (IsReturnHome(b.Command))
                {
                    overrideDestPos = new PointLatLngAlt(graph.Home);
                    flags |= SegmentFlags.ToHome;
                }
                if ((!HasLocation(a.Command) && overrideSrcPos == null) ||
                    (!HasLocation(b.Command) && overrideDestPos == null))
                {
                    continue;
                }

                switch (kind)
                {
                    case SegmentKind.Straight:
                    default:
                        segments.Add(GenerateStraightSegment(graph, edge, flags, overrideSrcPos, overrideDestPos));
                        break;
                    case SegmentKind.Spline:
                        segments.AddRange(GenerateSplineSegments(graph, edge, flags, overrideSrcPos));
                        break;
                    case SegmentKind.ArcTurn:
                        // TODO: generate arc turn segment
                        segments.Add(GenerateStraightSegment(graph, edge, flags, overrideSrcPos, overrideDestPos));
                        break;
                }
            }

            foreach (var loiterInfo in loiterInfoDict.Values)
            {
                if (loiterInfo.EntryBearing.HasValue)
                {
                    if (loiterInfo.ExitBearing.HasValue)
                    {
                        segments.Add(GenerateLoiterArcSegment(loiterInfo));
                    }
                    if (loiterInfo.AltExitBearing.HasValue)
                    {
                        //segments.Add(GenerateLoiterArcSegment(loiterInfo, isAlt: true));
                    }
                }
            }

            return segments;
        }

        static void UpdateLoiterInfoExits(ref LoiterInfo loiterInfo, double? exitBearing, bool isJump)
        {
            if (!isJump)
            {
                loiterInfo.ExitBearing = exitBearing;
                return;
            }
            if (!loiterInfo.EntryBearing.HasValue || !exitBearing.HasValue)
            {
                return;
            }
            // Track the furthest alternate exit bearing from the entry bearing
            double maxAngleDiff = 0.0;
            if (loiterInfo.ExitBearing.HasValue)
            {
                maxAngleDiff = Math.Max(
                    maxAngleDiff,
                    GetAngleDiff(loiterInfo.EntryBearing.Value, loiterInfo.ExitBearing.Value, loiterInfo.IsClockwise, false)
                );
            }
            if (loiterInfo.AltExitBearing.HasValue)
            {
                maxAngleDiff = Math.Max(
                    maxAngleDiff,
                    GetAngleDiff(loiterInfo.EntryBearing.Value, loiterInfo.AltExitBearing.Value, loiterInfo.IsClockwise, true)
                );
            }
            double angleDiff = GetAngleDiff(loiterInfo.EntryBearing.Value, exitBearing.Value, loiterInfo.IsClockwise, true);
            if (angleDiff > maxAngleDiff)
            {
                loiterInfo.AltExitBearing = exitBearing;
            }
        }

        static double GetAngleDiff(double entryBearing, double exitBearing, bool isClockwise, bool isAlt)
        {
            double sign = isClockwise ? 1.0 : -1.0;
            while (sign * exitBearing < sign * entryBearing)
            {
                exitBearing += sign * 360.0;
            }
            var angleDiff = Math.Abs(exitBearing - entryBearing);
            if (!isAlt && angleDiff < 15)
            {
                angleDiff += 360;
            }
            return angleDiff;
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

        static Segment GenerateStraightSegment(MissionGraph graph, MissionEdge edge, SegmentFlags flags, PointLatLngAlt overrideSrcPos, PointLatLngAlt overrideDestPos)
        {
            var src = edge.FromNode;
            var dest = edge.ToNode;
            var srcPos = overrideSrcPos ?? new PointLatLngAlt(src.Command);
            var destPos = overrideDestPos ?? new PointLatLngAlt(dest.Command);
            var distance = srcPos.GetDistance2(destPos);
            var midpoint = srcPos;
            if (distance > 1e-6)
            {
                var bearing = srcPos.GetBearing(destPos);
                midpoint = srcPos.newpos(bearing, distance / 2.0);
            }
            if (distance > 1e6)
            {
                Console.WriteLine("Uh oh");
                Debugger.Break();
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
                Midpoint = midpoint,
                Distance = distance,
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
            var OutgoingEdges = dest.OutgoingEdges;
            if (OutgoingEdges.Count == 0)
            {
                OutgoingEdges = new List<MissionEdge> { null };
            }


            foreach (var inEdge in IncomingEdges)
            {
                var prev = inEdge?.FromNode;
                PointLatLngAlt prevPos = null;
                if (prev != null && HasLocation(prev.Command))
                {
                    prevPos = new PointLatLngAlt(prev.Command);
                }
                if (prev != null && IsTakeoff(prev.Command))
                {
                    prevPos = GetTakeoffLocation(prev, graph.Home);
                }
                if (prevPos?.Lat == 0 && prevPos?.Lng == 0)
                {
                    prevPos = null;
                }
                var startType = SplineEndpointType.Stop;
                if (!IsSplineStoppedCopter(src.Command) && prevPos != null)
                {
                    if (src.Command.id == (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                    {
                        startType = SplineEndpointType.Spline;
                    }
                    else
                    {
                        startType = SplineEndpointType.Straight;
                    }
                }
                foreach (var outEdge in OutgoingEdges)
                {
                    var next = outEdge?.ToNode;
                    var endType = SplineEndpointType.Stop;
                    if (!IsSplineStoppedCopter(dest.Command) && next != null && HasLocation(next.Command))
                    {
                        if (next.Command.id == (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                        {
                            endType = SplineEndpointType.Spline;
                        }
                        else
                        {
                            endType = SplineEndpointType.Straight;
                        }
                    }
                    var nextPos = next != null ? new PointLatLngAlt(next.Command) : null;
                    Console.WriteLine($"Generating spline from {src.MissionIndex + 1}: Type={startType} to {dest.MissionIndex + 1}: Type={endType}");
                    Console.WriteLine($"  Prev: {(prev != null ? (prev.MissionIndex + 1).ToString() : "null")}");
                    Console.WriteLine($"  Next: {(next != null ? (next.MissionIndex + 1).ToString() : "null")}");
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

        static Segment GenerateLoiterArcSegment(LoiterInfo loiterInfo, bool isAlt = false, int samplesPerTurn = 40)
        {
            var bearing1 = (isAlt && loiterInfo.ExitBearing.HasValue) ? loiterInfo.ExitBearing.Value : loiterInfo.EntryBearing.Value;
            var bearing2 = isAlt ? loiterInfo.AltExitBearing.Value : loiterInfo.ExitBearing.Value;
            var angle_diff = GetAngleDiff(bearing1, bearing2, loiterInfo.IsClockwise, isAlt);
            int samples = (int)(samplesPerTurn * angle_diff / 360.0) + 2;
            if (!loiterInfo.IsClockwise) angle_diff *= -1;
            var path = new List<PointLatLngAlt>();
            for (int i = 0; i <= samples; i++)
            {
                double bearing = bearing1 + angle_diff * (i / (double)samples);
                var point = loiterInfo.Center.newpos(bearing, loiterInfo.Radius);
                path.Add(point);
            }

            return new Segment()
            {
                Kind = SegmentKind.LoiterArc,
                Flags = isAlt ? SegmentFlags.Alternate : SegmentFlags.None,
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

        static bool NeedsLoiterCapture(MissionNode dest, VehicleClass vehicleClass)
        {
            if (vehicleClass != VehicleClass.Plane)
            {
                return false;
            }
            return IsLoiter(dest.Command);
        }

        static bool NeedsLoiterExit(MissionNode src, VehicleClass vehicleClass)
        {
            if (vehicleClass != VehicleClass.Plane)
            {
                return false;
            }
            return IsLoiter(src.Command) && !IsTerminal(src.Command);
        }

        static double? GetLoiterExitBearing(MissionNode srcNode, MissionNode destNode, LoiterInfo loiterInfo)
        {
            bool crosstrackTangent = LoiterXTrackTangent(srcNode.Command);

            var destPoint = new PointLatLngAlt(destNode.Command);
           
            var distance = loiterInfo.Center.GetDistance2(destPoint);

            // If the next point is exactly at the loiter center, just assume it's North
            // (we have to pick somewhere on the circle to exit)
            var bearing = (distance > 1e-6) ? loiterInfo.Center.GetBearing(destPoint) : 0.0;

            double bearingChange = 0;
            if (distance < loiterInfo.Radius)
            {
                // When the destination is inside the loiter circle, we can't exit tangentially,
                // The alternative calculation is best explained in this PR: https://github.com/ArduPilot/ardupilot/pull/26102
                bearingChange = 2 * Math.Asin((loiterInfo.Radius - distance) / loiterInfo.Radius / 2) * (180.0 / Math.PI);
            }
            else if (crosstrackTangent)
            {
                // The center, the destination, and the exit point form a right triangle
                // center -> dest point is the hypotenuse
                // center -> exit point -> dest point is the right angle
                // We solve for the angle between the center->dest line and the center->exit line
                // and add that to the bearing to the dest point
                bearingChange = Math.Acos(loiterInfo.Radius / distance) * (180.0 / Math.PI);
            }
            if (loiterInfo.IsClockwise)
            {
                bearingChange = -bearingChange;
            }
            bearing += bearingChange;
            return bearing;
        }

        static double? GetLoiterEntryBearing(MissionNode srcNode, LoiterInfo loiterInfo, PointLatLngAlt overrideSrcPos)
        {
            var srcPos = new PointLatLngAlt(srcNode.Command);
            if (overrideSrcPos != null)
            {
                srcPos = overrideSrcPos;
            }
            var distance = loiterInfo.Center.GetDistance2(srcPos);
            var bearing = (distance > 1e-6) ? loiterInfo.Center.GetBearing(srcPos) : 0.0;
            return bearing;
        }
    }
}
