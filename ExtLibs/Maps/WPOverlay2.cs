using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Mission;
using static MissionPlanner.Utilities.Mission.CommandUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MissionPlanner.Maps
{
    public class WPOverlay2
    {
        public GMapOverlay overlay = new GMapOverlay("WPOverlay2");

        /// <summary>
        /// List of points as per the mission (non-expanded, skips DO_JUMP repeats),
        /// compatible with WPOverlay.pointlist semantics.
        /// </summary>
        public List<PointLatLngAlt> pointlist = new List<PointLatLngAlt>();

        public VehicleClass VehicleClass = VehicleClass.Copter;
        public bool ShowPlusMarkers = true;

        public void CreateOverlay(
            PointLatLngAlt home,
            List<Locationwp> missionitems,
            double wpradius,
            double loiterradius,
            double altunitmultiplier)
        {
            overlay.Clear();
            
            // Only planes should have a default loiter radius
            if (VehicleClass != VehicleClass.Plane)
            {
                loiterradius = wpradius;
            }

            // 1) Rebuild pointlist with legacy semantics
            BuildPointList(home, missionitems);

            // 2) Build a mission graph (no jump expansion)
            var graph = MissionGraphBuilder.Build(home, missionitems);

            // 3) Generate segments from the graph
            var segments = MissionSegmentizer.BuildSegments(graph, VehicleClass, loiterradius);

            // 4) Render markers and segments to overlay
            MissionRenderer.RenderMarkers(overlay, graph, wpradius, loiterradius, altunitmultiplier);
            MissionRenderer.RenderSegments(overlay, segments, ShowPlusMarkers);
        }

        public sealed class MissionRenderer
        {
            public static void RenderMarkers(
                GMapOverlay overlay,
                MissionGraph graph,
                double wpradius,
                double loiterradius,
                double altunitmultiplier)
            {
                foreach (var node in graph.Nodes)
                {
                    if (!HasLatLon(node.Command))
                    {
                        continue;
                    }
                    var point = new PointLatLng(node.Command.lat, node.Command.lng);
                    var tag = PointTag(node.MissionIndex);
                    var alt = node.Command.alt * altunitmultiplier;
                    // TODO: vary marker style based on command type
                    var marker = new GMapMarkerWP(point, tag)
                    {
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        ToolTipText = "Alt: " + alt.ToString("0"),
                        Tag = tag
                    };
                    GMapMarkerRect mBorders = new GMapMarkerRect(point)
                    {
                        InnerMarker = marker,
                        Tag = tag,
                        wprad = MarkerRadius(node.Command, loiterradius, wpradius),
                    };
                    overlay.Markers.Add(marker);
                    overlay.Markers.Add(mBorders);
                }
            }

            public static void RenderSegments(
                GMapOverlay overlay,
                List<MissionSegmentizer.Segment> segments,
                bool showPlusMarkers)
            {
                foreach (var segment in segments)
                {
                    var points = new List<PointLatLng>();
                    foreach (var pt in segment.Path)
                    {
                        points.Add(new PointLatLng(pt.Lat, pt.Lng));
                    }
                    // TODO: vary color/style based on segment kind
                    var route = new GMapRoute(points, "segment")
                    {
                        Stroke = new Pen(Color.Yellow, segment.IsPrimary ? 4 : 2)
                        {
                            DashStyle = segment.IsPrimary ? DashStyle.Solid : DashStyle.Dash,
                        },
                        ArrowMode = GMapRoute.ArrowDrawMode.SinglePerRoute,
                    };
                    overlay.Routes.Add(route);

                    // These markers are handled FlightPlanner to insert waypoints
                    if (showPlusMarkers && segment.IsPrimary && segment.Midpoint != null)
                    {
                        var plusMarker = new GMapMarkerPlus(segment.Midpoint)
                        {
                            Tag = MakeMidlineObject(segment),
                        };
                        overlay.Markers.Add(plusMarker);
                    }
                }
            }

            static midline MakeMidlineObject(MissionSegmentizer.Segment segment)
            {
                var startNode = segment.StartNode;
                var endNode = segment.EndNode;
                return new midline
                {
                    now = new PointLatLngAlt(
                        startNode.Command.lat,
                        startNode.Command.lng,
                        startNode.Command.alt,
                        PointTag(startNode.MissionIndex)
                    ),
                    next = new PointLatLngAlt(
                        endNode.Command.lat,
                        endNode.Command.lng,
                        endNode.Command.alt,
                        PointTag(endNode.MissionIndex)
                    )
                };
            }

            static string PointTag(int index)
            {
                return (index < 0) ? "H" : (index + 1).ToString();
            }
        }

        /// <summary>
        /// Build the public `pointlist` from the mission items exactly the same way as the old WPOverlay did.
        /// The use of pointlist will be phased out in the future.
        /// </summary>
        /// <param name="home"></param>
        /// <param name="missionitems"></param>
        void BuildPointList(
            PointLatLngAlt home,
            List<Locationwp> missionitems)
        {
            pointlist.Clear();

            double gethomealt(MAVLink.MAV_FRAME altmode, double lat, double lng) =>
                GetHomeAlt(altmode, home.Alt, lat, lng);

            if (home != PointLatLngAlt.Zero)
            {
                var homeCopy = new PointLatLngAlt(home.Lat, home.Lng, home.Alt, "H");
                pointlist.Add(homeCopy);
            }

            for (int a = 0; a < missionitems.Count; a++)
            {
                var item = missionitems[a];

                ushort command = item.id;

                if (command == 0)
                {
                    pointlist.Add(null);
                    continue;
                }

                if (command < (ushort)MAVLink.MAV_CMD.LAST &&
                    command != (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH &&
                    command != (ushort)MAVLink.MAV_CMD.CONTINUE_AND_CHANGE_ALT &&
                    command != (ushort)MAVLink.MAV_CMD.DELAY &&
                    command != (ushort)MAVLink.MAV_CMD.GUIDED_ENABLE
                    || command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI
                    || command == (ushort)MAVLink.MAV_CMD.DO_LAND_START)
                {
                    if ((command == (ushort)MAVLink.MAV_CMD.LAND ||
                         command == (ushort)MAVLink.MAV_CMD.VTOL_LAND) &&
                        item.lat == 0 && item.lng == 0)
                    {
                        continue;
                    }

                    if (command == (ushort)MAVLink.MAV_CMD.DO_LAND_START &&
                        item.lat != 0 && item.lng != 0)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            (a + 1).ToString()));
                    }
                    else if ((command == (ushort)MAVLink.MAV_CMD.LAND ||
                              command == (ushort)MAVLink.MAV_CMD.VTOL_LAND) &&
                             item.lat != 0 && item.lng != 0)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            (a + 1).ToString()));
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.DO_SET_ROI)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            "ROI" + (a + 1))
                        { color = Color.Red });
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.LOITER_TIME ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_TURNS ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_TO_ALT ||
                             command == (ushort)MAVLink.MAV_CMD.LOITER_UNLIM)
                    {
                        if (item.lat == 0 && item.lng == 0)
                        {
                            // loiter at current location -> null entry (matches old)
                            pointlist.Add(null);
                        }
                        else
                        {
                            pointlist.Add(new PointLatLngAlt(
                                item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                                (a + 1).ToString())
                            { color = Color.LightBlue });
                        }
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.SPLINE_WAYPOINT)
                    {
                        pointlist.Add(new PointLatLngAlt(
                            item.lat, item.lng,
                            item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                            (a + 1).ToString())
                        { Tag2 = "spline" });
                    }
                    else if (command == (ushort)MAVLink.MAV_CMD.WAYPOINT &&
                             item.lat == 0 && item.lng == 0)
                    {
                        pointlist.Add(null);
                    }
                    else
                    {
                        if (item.lat != 0 && item.lng != 0)
                        {
                            pointlist.Add(new PointLatLngAlt(
                                item.lat, item.lng,
                                item.alt + gethomealt((MAVLink.MAV_FRAME)item.frame, item.lat, item.lng),
                                (a + 1).ToString()));
                        }
                        else
                        {
                            pointlist.Add(null);
                        }
                    }
                }
                else if (command == (ushort)MAVLink.MAV_CMD.DO_JUMP)
                {
                    pointlist.Add(null);
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.FENCE_RETURN_POINT)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else if (command == (ushort)MAVLink.MAV_CMD.RALLY_POINT)
                {
                    pointlist.Add(new PointLatLngAlt(
                        item.lat, item.lng, 0, (a + 1).ToString()));
                }
                else
                {
                    pointlist.Add(null);
                }
            }
        }

        private double GetHomeAlt(
            MAVLink.MAV_FRAME altmode,
            double homealt,
            double lat,
            double lng)
        {
            if (altmode == MAVLink.MAV_FRAME.GLOBAL_INT ||
                altmode == MAVLink.MAV_FRAME.GLOBAL)
            {
                // absolute: don't add home alt
                return 0;
            }

            if (altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT_INT ||
                altmode == MAVLink.MAV_FRAME.GLOBAL_TERRAIN_ALT)
            {
                var sralt = srtm.getAltitude(lat, lng);
                if (sralt.currenttype == srtm.tiletype.invalid)
                    return -999;
                return sralt.alt;
            }

            return homealt;
        }
    }
}
