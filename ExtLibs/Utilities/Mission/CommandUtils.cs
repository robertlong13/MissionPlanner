using System;
using System.Collections.Generic;
using System.Linq;

namespace MissionPlanner.Utilities.Mission
{
    public static class CommandUtils
    {
        public static bool IsLoiter(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_TIME ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_TURNS ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT;
        }

        public static bool IsTakeoff(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.TAKEOFF ||
                   cmd == (ushort)MAVLink.MAV_CMD.TAKEOFF_LOCAL ||
                   cmd == (ushort)MAVLink.MAV_CMD.VTOL_TAKEOFF;
        }

        public static bool IsLand(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.LAND ||
                   cmd == (ushort)MAVLink.MAV_CMD.VTOL_LAND ||
                   cmd == (ushort)MAVLink.MAV_CMD.LAND_LOCAL;
        }

        public static double LoiterRadius(Locationwp cmd, double defaultRadius)
        {
            switch (cmd.id)
            {
            case (ushort)MAVLink.MAV_CMD.LOITER_TURNS:
            case (ushort)MAVLink.MAV_CMD.LOITER_TIME:
            case (ushort)MAVLink.MAV_CMD.LOITER_UNLIM:
                return cmd.p3 != 0 ? cmd.p3 : defaultRadius;
            case (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT:
                return cmd.p2 != 0 ? cmd.p2 : defaultRadius;
            default:
                throw new ArgumentException("Command is not a loiter command");
            }
        }

        public static bool IsLoiterXTrackTangent(Locationwp cmd)
        {
            return IsLoiter(cmd.id) && !IsTerminal(cmd.id) && cmd.p4 == 1;
        }

        public static double MarkerRadius(Locationwp cmd, double defaultLoiterRadius, double defaultWPRadius)
        {
            if (IsLoiter(cmd.id))
            {
                return Math.Abs(LoiterRadius(cmd, defaultLoiterRadius));
            }
            else
            {
                return Math.Abs(defaultWPRadius);
            }
        }

        public static bool HasLocation(Locationwp cmd)
        {
            if (!HasLatLon(cmd))
            {
                return false;
            }
            if (cmd.id == (ushort)MAVLink.MAV_CMD.DO_GO_AROUND)
            {
                // ArduPilot violates the spec and treats this as a positional bookmark
                // (like DO_LAND_START and DO_RETURN_PATH_START)
                return true;
            }
            var mavCmdType = typeof(MAVLink.MAV_CMD);
            if (!Enum.IsDefined(mavCmdType, cmd.id))
                return true; // unknown, assume positional if lat/lon present
            var name = ((MAVLink.MAV_CMD)cmd.id).ToString();
            var memberInfo = mavCmdType.GetMember(name).FirstOrDefault();
            if (memberInfo == null)
                return true; // should not be possible

            return memberInfo
                .GetCustomAttributes(false)
                .OfType<MAVLink.hasLocation>()
                .Any();
        }

        static bool HasLatLon(Locationwp cmd)
        {
            return !(cmd.lat == 0 && cmd.lng == 0) &&
                   !double.IsNaN(cmd.lat) &&
                   !double.IsNaN(cmd.lng);
        }

        public static bool IsTerminal(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.DO_RALLY_LAND ||
                   cmd == (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_FLIGHTTERMINATION ||
                   cmd == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM;
        }

        /// <summary>
        /// Whether a command is a connected point in a route (like nav commands in missions, or polygon fence points)
        /// </summary>
        public static bool IsNode(Locationwp cmd)
        {
            // Despite not being NAV commands, these fence polygon commands are connected nodes
            if (cmd.id == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION ||
                cmd.id == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION)
            {
                return true;
            }

            // The obsolete "ROI" command is in the "NAV" range, but is not actually a NAV command
            if (cmd.id == (ushort)MAVLink.MAV_CMD.ROI)
            {
                return false;
            }

            // These commands terminate the flight, so even though some do not have a location, they get a node
            if (IsTerminal(cmd.id))
            {
                return true;
            }

            // Takeoffs don't have a lat/lon set, but they are still nodes
            // (their location will be home or the immediately preceding land)
            if (IsTakeoff(cmd.id))
            {
                return true;
            }

            // Otherwise, anything in the NAV command range that has a location is a connected node
            if (cmd.id < (ushort)MAVLink.MAV_CMD.LAST)
            {
                return HasLocation(cmd);
            }

            return false;
        }

        public static bool IsBookmark(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.JUMP_TAG ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_LAND_START ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_RETURN_PATH_START ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_GO_AROUND;
        }

        public static bool IsRegionOfInterest(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.ROI ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_SET_ROI ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_SET_ROI_LOCATION;
        }

        public static bool IsFencePoint(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION ||
                   cmd == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION ||
                   cmd == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION ||
                   cmd == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION;
        }

        public static bool TryGetJumpTarget(Locationwp cmd, Dictionary<int, int> jumpTags, out int jumpTarget)
        {
            switch (cmd.id)
            {
            case (ushort)MAVLink.MAV_CMD.DO_JUMP:
                jumpTarget = (int)cmd.p1 - 1;
                return true;
            case (ushort)MAVLink.MAV_CMD.DO_JUMP_TAG:
                return jumpTags.TryGetValue((int)cmd.p1, out jumpTarget);
            default:
                throw new ArgumentException("Command is not a jump");
            }
        }

        public static bool IsJumpCommand(ushort cmd)
        {
            return cmd == (ushort)MAVLink.MAV_CMD.DO_JUMP ||
                   cmd == (ushort)MAVLink.MAV_CMD.DO_JUMP_TAG;
        }

        public static int GetJumpCount(Locationwp cmd)
        {
            if (IsJumpCommand(cmd.id))
            {
                return (int)cmd.p2;
            }
            else
            {
                throw new ArgumentException("Command is not a jump");
            }
        }

        public static bool IsSplineStoppedCopter(Locationwp cmd)
        {
            if (IsTakeoff(cmd.id) || IsLand(cmd.id))
            {
                return true;
            }
            switch (cmd.id)
            {
            case (ushort)MAVLink.MAV_CMD.WAYPOINT:
            case (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT:
            case (ushort)MAVLink.MAV_CMD.LOITER_TIME:
                return cmd.p1 > 0;
            case (ushort)MAVLink.MAV_CMD.LOITER_TURNS:
            case (ushort)MAVLink.MAV_CMD.PAYLOAD_PLACE:
                return true;
            default:
                return false;
            }
        }

        public static PointLatLngAlt GetTakeoffLocation(MissionNode node, PointLatLngAlt home)
        {
            if (!IsTakeoff(node.Command.id))
            {
                throw new ArgumentException("Command is not a takeoff");
            }
            var prev_node = node.IncomingEdges?.FirstOrDefault()?.FromNode;
            if (prev_node == null)
            {
                // Nothing leads to here, so just use home
                return home;
            }
            if (!IsLand(prev_node.Command.id))
            {
                // Placing a takeoff right after a non-land command is undefined behavior (usually skipped).
                // Null is better than home in this case.
                return null;
            }
            // Last node was a land. Takeoff from there
            if (HasLocation(prev_node.Command))
            {
                return new PointLatLngAlt(prev_node.Command);
            }
            // If the land has no location, backtrack until we find one
            var visited = new HashSet<int>();
            while (prev_node != null && !visited.Contains(prev_node.MissionIndex) && !HasLocation(prev_node.Command))
            {
                visited.Add(prev_node.MissionIndex);
                prev_node = prev_node.IncomingEdges?.FirstOrDefault()?.FromNode;
            }
            if (prev_node == null || !HasLocation(prev_node.Command))
            {
                return home;
            }
            return new PointLatLngAlt(prev_node.Command);
        }
    }
}
