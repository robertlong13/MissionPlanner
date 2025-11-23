using GMap.NET.WindowsForms.Markers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities.Mission
{
    public sealed class SegmentStyle
    {
        public Color StrokeColor { get; set; }
        public float StrokeWidth { get; set; }
        public DashStyle DashStyle { get; set; }
        public bool ShowArrow { get; set; }
    }

    public sealed class SegmentStyleRuleConfig
    {
        public SegmentKind? KindFilter { get; set; }
        public SegmentFlags? RequiredFlags { get; set; }
        public SegmentFlags? ExcludedFlags { get; set; }

        public Color? StrokeColor { get; set; }
        public float? StrokeWidth { get; set; }
        public DashStyle? DashStyle { get; set; }
        public bool? ShowArrow { get; set; }
    }

    public sealed class MarkerStyle
    {
        public GMarkerGoogleType MarkerType { get; set; }
        public Color CircleColor { get; set; }
    }

    public sealed class MarkerStyleRuleConfig
    {
        public ushort[] RawCommandIds { get; set; }
        public bool? AllLoiters { get; set; }
        public bool? AllTakeoffs { get; set; }
        public bool? AllLandings { get; set; }
        public bool? AllBookmarks { get; set; }
        public bool? AllFencePoints { get; set; }
        public bool? AllRegionsOfInterest { get; set; }

        public GMarkerGoogleType? MarkerType { get; set; }
        public Color? CircleColor { get; set; }
    }

    public sealed class SegmentStyleRule
    {
        readonly SegmentStyleRuleConfig _config;

        public SegmentStyleRule(SegmentStyleRuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool Applies(SegmentKind kind, SegmentFlags flags)
        {
            if (_config.KindFilter.HasValue && kind != _config.KindFilter.Value)
            {
                return false;
            }

            if (_config.RequiredFlags.HasValue &&
                (flags & _config.RequiredFlags.Value) != _config.RequiredFlags.Value)
            {
                return false;
            }

            if (_config.ExcludedFlags.HasValue && (flags & _config.ExcludedFlags.Value) != 0)
            {
                return false;
            }

            return true;
        }

        public void Apply(SegmentStyle style)
        {
            if (_config.StrokeColor.HasValue) style.StrokeColor = _config.StrokeColor.Value;
            if (_config.StrokeWidth.HasValue) style.StrokeWidth = _config.StrokeWidth.Value;
            if (_config.DashStyle.HasValue) style.DashStyle = _config.DashStyle.Value;
            if (_config.ShowArrow.HasValue) style.ShowArrow = _config.ShowArrow.Value;
        }
    }

    public sealed class MarkerStyleRule
    {
        readonly MarkerStyleRuleConfig _config;

        public MarkerStyleRule(MarkerStyleRuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool Applies(ushort cmd)
        {
            bool? applies = null;
            if (_config.RawCommandIds != null && _config.RawCommandIds.Length > 0)
            {
                applies &= Array.Exists(_config.RawCommandIds, id => id == cmd);
            }
            if (_config.AllLoiters.HasValue)
            {
                applies &= IsLoiter(cmd);
            }
            if (_config.AllTakeoffs.HasValue)
            {
                applies &= IsTakeoff(cmd);
            }
            if (_config.AllLandings.HasValue)
            {
                applies &= IsLand(cmd);
            }
            if (_config.AllBookmarks.HasValue)
            {
                applies &= IsBookmark(cmd);
            }
            if (_config.AllFencePoints.HasValue)
            {
                applies &= IsFencePoint(cmd);
            }
            if (_config.AllRegionsOfInterest.HasValue)
            {
                applies &= IsRegionOfInterest(cmd);
            }
            return applies ?? true;
        }

        public void Apply(MarkerStyle style)
        {
            if (_config.MarkerType.HasValue) style.MarkerType = _config.MarkerType.Value;
            if (_config.CircleColor.HasValue) style.CircleColor = _config.CircleColor.Value;
        }
    }

    public static class MissionStyle
    {
        static readonly SegmentStyleRuleConfig[] DefaultSegmentRuleConfigs =
        {
            // Base default: any straight primary leg
            new SegmentStyleRuleConfig
            {
                KindFilter   = null,
                StrokeColor  = Color.Yellow,
                StrokeWidth  = 4f,
                DashStyle    = DashStyle.Solid,
                ShowArrow    = true,
            },

            // Splines
            new SegmentStyleRuleConfig
            {
                KindFilter  = SegmentKind.Spline,
                StrokeColor = Color.LimeGreen
            },

            // Loiter arcs
            new SegmentStyleRuleConfig
            {
                KindFilter  = SegmentKind.LoiterArc,
                StrokeColor = Color.LightCoral
            },

            // Arc turns: purple-ish
            new SegmentStyleRuleConfig
            {
                KindFilter  = SegmentKind.ArcTurn,
                StrokeColor = Color.MediumVioletRed,
            },

            // Alternate / jump legs: thinner, dashed
            new SegmentStyleRuleConfig
            {
                RequiredFlags = SegmentFlags.Alternate,
                StrokeWidth   = 2f,
                DashStyle     = DashStyle.Dash
            },

            // Takeoff-origin legs: blue
            new SegmentStyleRuleConfig
            {
                RequiredFlags = SegmentFlags.FromTakeoff,
                StrokeColor   = Color.Blue
            },
        };

        static readonly MarkerStyleRuleConfig[] DefaultMarkerRuleConfigs =
        {
            // Base default
            new MarkerStyleRuleConfig
            {
                MarkerType = GMarkerGoogleType.green,
                CircleColor = Color.White
            },

            // Loiter commands: transparent circle
            new MarkerStyleRuleConfig
            {
                AllLoiters = true,
                CircleColor = Color.Transparent
            },

            // Land commands: light green
            new MarkerStyleRuleConfig
            {
                AllLandings = true,
                MarkerType = GMarkerGoogleType.red,
                CircleColor = Color.Transparent
            },

            // Bookmark commands: orange
            new MarkerStyleRuleConfig
            {
                AllBookmarks = true,
                MarkerType = GMarkerGoogleType.orange,
                CircleColor = Color.Transparent
            },

            // ROI commands: purple
            new MarkerStyleRuleConfig
            {
                AllRegionsOfInterest = true,
                MarkerType = GMarkerGoogleType.purple,
                CircleColor = Color.Transparent
            },
        };

        // Runtime rule arrays built from configs
        static readonly SegmentStyleRule[] SegmentRules;
        static readonly MarkerStyleRule[] MarkerRules;

        static MissionStyle()
        {
            SegmentRules = new SegmentStyleRule[DefaultSegmentRuleConfigs.Length];
            for (int i = 0; i < DefaultSegmentRuleConfigs.Length; i++)
                SegmentRules[i] = new SegmentStyleRule(DefaultSegmentRuleConfigs[i]);

            MarkerRules = new MarkerStyleRule[DefaultMarkerRuleConfigs.Length];
            for (int i = 0; i < DefaultMarkerRuleConfigs.Length; i++)
                MarkerRules[i] = new MarkerStyleRule(DefaultMarkerRuleConfigs[i]);
        }

        public static SegmentStyle GetSegmentStyle(MissionSegmentizer.Segment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));

            // Start from neutral defaults; rules layer on top
            var style = new SegmentStyle
            {
                StrokeColor = Color.Yellow,
                StrokeWidth = 3f,
                DashStyle = DashStyle.Solid,
                ShowArrow = true
            };

            var kind = segment.Kind;
            var flags = segment.Flags;

            foreach (var rule in SegmentRules)
            {
                if (rule.Applies(kind, flags))
                {
                    rule.Apply(style);
                }
            }

            return style;
        }

        public static MarkerStyle GetMarkerStyle(ushort cmd)
        {
            var style = new MarkerStyle
            {
                MarkerType = GMarkerGoogleType.green,
                CircleColor = Color.White
            };

            foreach (var rule in MarkerRules)
            {
                if (rule.Applies(cmd))
                {
                    rule.Apply(style);
                }
            }

            return style;
        }
    }
}
