using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Mission;
using static MissionPlanner.Utilities.Mission.CommandUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using GMap.NET.WindowsForms.Markers;

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
            MissionRenderer.RenderSegments(overlay, segments, wpradius, loiterradius, ShowPlusMarkers);
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
                if(graph.Home != PointLatLngAlt.Zero)
                {
                    var point = new PointLatLng(graph.Home.Lat, graph.Home.Lng);
                    var tag = PointTag(-1);
                    var alt = graph.Home.Alt * altunitmultiplier;
                    var marker = new GMapMarkerWP(point, tag)
                    {
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        ToolTipText = "Alt: " + alt.ToString("0"),
                        Tag = tag
                    };
                    overlay.Markers.Add(marker);
                }

                foreach (var node in graph.Nodes)
                {
                    if (!HasLocation(node.Command))
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

                foreach (var bookmark in graph.Bookmarks)
                {
                    PointLatLng point = GetBookmarkLocation(bookmark);
                    if (point.IsEmpty)
                    {
                        continue;
                    }
                    string label = GetBookmarkLabel(bookmark);
                    var marker = new GMapMarkerWP(point, label, type: GMarkerGoogleType.orange)
                    {
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        ToolTipText = "",
                        // "Tag" lets FlightPlanner identify which command to update during drag
                        Tag = HasLocation(bookmark.Command) ? PointTag(bookmark.MissionIndex) : PointTag(bookmark.Target.MissionIndex)
                    };
                    overlay.Markers.Add(marker);
                    if (HasLocation(bookmark.Command))
                    {
                        var mBorders = new GMapMarkerRect(point)
                        {
                            InnerMarker = marker,
                            Tag = marker.Tag,
                            wprad = 0, // no radius for bookmark markers
                        };
                        overlay.Markers.Add(mBorders);
                    }
                    // Render a thin segment to the target marker
                    if (bookmark.Target != null && HasLocation(bookmark.Command) && HasLocation(bookmark.Target.Command))
                    {
                        var route = new GMapRoute(new List<PointLatLng>
                        {
                            new PointLatLng(bookmark.Command.lat, bookmark.Command.lng),
                            new PointLatLng(bookmark.Target.Command.lat, bookmark.Target.Command.lng),
                        }, "bookmark-segment")
                        {
                            Stroke = new Pen(Color.Orange, 2)
                            {
                                DashStyle = DashStyle.Dash,
                            },
                        };
                        overlay.Routes.Add(route);
                    }
                }
            }

            public static void RenderSegments(
                GMapOverlay overlay,
                List<MissionSegmentizer.Segment> segments,
                double wpradius,
                double loiterradius,
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
                    bool isAlternate = segment.Flags.HasFlag(SegmentFlags.Alternate);
                    bool isTakeoff = segment.Flags.HasFlag(SegmentFlags.FromTakeoff);
                    var route = new GMapRoute(points, "segment")
                    {
                        Stroke = new Pen(isTakeoff ? Color.Blue : Color.Yellow, isAlternate ? 2 : 4)
                        {
                            DashStyle = isAlternate ? DashStyle.Dash : DashStyle.Solid,
                        },
                        ArrowMode = GMapRoute.ArrowDrawMode.SinglePerRoute,
                    };
                    overlay.Routes.Add(route);

                    // These markers are handled by FlightPlanner.cs to insert waypoints
                    if (showPlusMarkers && 
                        !isAlternate &&
                        segment.StartNode != null &&
                        segment.EndNode != null &&
                        segment.Midpoint != null)
                    {
                        // Skip the insert marker if the segment is too short
                        var markerRadius = Math.Max(
                            MarkerRadius(segment.StartNode.Command, loiterradius, wpradius),
                            MarkerRadius(segment.EndNode.Command, loiterradius, wpradius));
                        if (1000 * route.Distance < markerRadius)
                        {
                            continue;
                        }
                        var midLine = MakeMidlineObject(segment);
                        var plusMarker = new GMapMarkerPlus(segment.Midpoint)
                        {
                            Tag = midLine,
                        };
                        overlay.Markers.Add(plusMarker);
                    }
                }
            }

            static midline MakeMidlineObject(MissionSegmentizer.Segment segment)
            {
                var startNode = segment.StartNode;
                var endNode = segment.EndNode;
                int endIndex = endNode.MissionIndex;
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

        static PointLatLng GetBookmarkLocation(MissionBookmark bookmark)
        {
            if (HasLocation(bookmark.Command))
            {
                return new PointLatLng(bookmark.Command.lat, bookmark.Command.lng);
            }
            else if (HasLocation(bookmark.Target.Command))
            {
                return new PointLatLng(bookmark.Target.Command.lat, bookmark.Target.Command.lng);
            }
            return PointLatLng.Empty;
        }

        static string GetBookmarkLabel(MissionBookmark bookmark)
        {
            switch (bookmark.Command.id)
            {
            case (ushort)MAVLink.MAV_CMD.JUMP_TAG:
                return "T" + ((int)bookmark.Command.p1).ToString();
            case (ushort)MAVLink.MAV_CMD.DO_LAND_START:
                return "LS";
            case (ushort)MAVLink.MAV_CMD.DO_RETURN_PATH_START:
                return "RP";
            case (ushort)MAVLink.MAV_CMD.DO_GO_AROUND:
                return "GA";
            default:
                return "";
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
