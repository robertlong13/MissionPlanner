using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml.Serialization;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities.Mission
{
    public sealed class SegmentStyle
    {
        public Color StrokeColor { get; set; } = Color.Yellow;
        public float StrokeWidth { get; set; } = 3f;
        public DashStyle DashStyle { get; set; } = DashStyle.Solid;
        public bool ShowArrow { get; set; } = true;
    }

    public sealed class SegmentStyleRuleConfig
    {
        public string Description { get; set; }
        public SegmentKind? KindFilter { get; set; }
        public SegmentFlags? RequiredFlags { get; set; }
        public SegmentFlags? ExcludedFlags { get; set; }

        public Color? StrokeColor { get; set; }
        public float? StrokeWidth { get; set; }
        public DashStyle? DashStyle { get; set; }
        public bool? ShowArrow { get; set; }
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

    public sealed class MarkerStyle
    {
        public GMarkerGoogleType MarkerType { get; set; } = GMarkerGoogleType.green;
        public Color CircleColor { get; set; } = Color.White;
    }

    public sealed class MarkerStyleRuleConfig
    {
        public string Description { get; set; }
        public ushort[] RawCommandIds { get; set; }
        public bool AllLoiters { get; set; }
        public bool AllTakeoffs { get; set; }
        public bool AllLandings { get; set; }
        public bool AllBookmarks { get; set; }
        public bool AllFencePoints { get; set; }
        public bool AllRegionsOfInterest { get; set; }

        public GMarkerGoogleType? MarkerType { get; set; }
        public Color? CircleColor { get; set; }
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
            bool applies = true;
            if (_config.RawCommandIds != null && _config.RawCommandIds.Length > 0)
            {
                applies &= Array.Exists(_config.RawCommandIds, id => id == cmd);
            }
            if (_config.AllLoiters)
            {
                applies &= IsLoiter(cmd);
            }
            if (_config.AllTakeoffs)
            {
                applies &= IsTakeoff(cmd);
            }
            if (_config.AllLandings)
            {
                applies &= IsLand(cmd);
            }
            if (_config.AllBookmarks)
            {
                applies &= IsBookmark(cmd);
            }
            if (_config.AllFencePoints)
            {
                applies &= IsFencePoint(cmd);
            }
            if (_config.AllRegionsOfInterest)
            {
                applies &= IsRegionOfInterest(cmd);
            }
            return applies;
        }

        public void Apply(MarkerStyle style)
        {
            if (_config.MarkerType.HasValue) style.MarkerType = _config.MarkerType.Value;
            if (_config.CircleColor.HasValue) style.CircleColor = _config.CircleColor.Value;
        }
    }

    [XmlRoot("MissionStyle")]
    public class MissionStyleConfig
    {
        public List<SegmentStyleRuleConfig> SegmentRules { get; set; } = new List<SegmentStyleRuleConfig>();
        public List<MarkerStyleRuleConfig> MarkerRules { get; set; } = new List<MarkerStyleRuleConfig>();
    }

    public class MissionStyle
    {
        static readonly MissionStyleConfig DefaultConfig = new MissionStyleConfig()
        {
            SegmentRules = new List<SegmentStyleRuleConfig>()
            {
                new SegmentStyleRuleConfig
                {
                    Description  = "Base defaults",
                    KindFilter   = null,
                    StrokeColor  = Color.FromArgb(180, 255, 255, 0),
                    StrokeWidth  = 3f,
                    DashStyle    = DashStyle.Solid,
                    ShowArrow    = true,
                },
                new SegmentStyleRuleConfig
                {
                    Description   = "Jumps/Alternates",
                    RequiredFlags = SegmentFlags.Alternate,
                    StrokeWidth   = 2f,
                    DashStyle     = DashStyle.Dash
                },
                new SegmentStyleRuleConfig
                {
                    Description   = "Takeoff segments",
                    RequiredFlags = SegmentFlags.FromTakeoff,
                    StrokeWidth   = 2f,
                    DashStyle     = DashStyle.Dash
                },
                new SegmentStyleRuleConfig
                {
                    Description   = "Bookmark lines",
                    RequiredFlags = SegmentFlags.FromBookmark,
                    StrokeColor   = Color.Orange
                },
                new SegmentStyleRuleConfig
                {
                    Description   = "Landing sequence",
                    RequiredFlags = SegmentFlags.LandSequence,
                    StrokeColor = Color.FromArgb(128, 180, 255, 0),
                },
            },
            MarkerRules = new List<MarkerStyleRuleConfig>()
            {
                new MarkerStyleRuleConfig
                {
                    Description = "Base defaults",
                    MarkerType = GMarkerGoogleType.green,
                    CircleColor = Color.FromArgb(64, 255, 255, 255)
                },
                new MarkerStyleRuleConfig
                {
                    Description = "Loiters",
                    AllLoiters = true,
                    CircleColor = Color.Transparent
                },
                new MarkerStyleRuleConfig
                {
                    Description = "Landings",
                    AllLandings = true,
                    CircleColor = Color.Transparent
                },
                new MarkerStyleRuleConfig
                {
                    Description = "Bookmarks",
                    AllBookmarks = true,
                    MarkerType = GMarkerGoogleType.orange,
                    CircleColor = Color.Transparent
                },
                new MarkerStyleRuleConfig
                {
                    Description = "ROI",
                    AllRegionsOfInterest = true,
                    MarkerType = GMarkerGoogleType.purple,
                    CircleColor = Color.Transparent
                },
            }
        };

        public MissionStyleConfig config { get; private set; }

        // Runtime rule arrays built from configs
        List<SegmentStyleRule> SegmentRules = new List<SegmentStyleRule>();
        List<MarkerStyleRule> MarkerRules = new List<MarkerStyleRule>();

        public MissionStyle(MissionStyleConfig config = null)
        {
            this.config = config ?? DefaultConfig;
            RefreshConfig();
        }

        private void RefreshConfig()
        {
            foreach (var segmentStyleRuleConfig in config.SegmentRules)
            {
                SegmentRules.Add(new SegmentStyleRule(segmentStyleRuleConfig));
            }
            foreach (var markerStyleRuleConfig in config.MarkerRules)
            {
                MarkerRules.Add(new MarkerStyleRule(markerStyleRuleConfig));
            }
        }

        public SegmentStyle GetSegmentStyle(MissionSegmentizer.Segment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            var style = new SegmentStyle();
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

        public MarkerStyle GetMarkerStyle(ushort cmd)
        {
            var style = new MarkerStyle();
            foreach (var rule in MarkerRules)
            {
                if (rule.Applies(cmd))
                {
                    rule.Apply(style);
                }
            }
            return style;
        }

        public static MissionStyle LoadFromConfig(string styleFilePath)
        {
            if (string.IsNullOrEmpty(styleFilePath) || !File.Exists(styleFilePath))
                return new MissionStyle();

            try
            {
                var serializer = new XmlSerializer(typeof(MissionStyleConfig));
                using (var stream = File.OpenRead(styleFilePath))
                {
                    var cfg = (MissionStyleConfig)serializer.Deserialize(stream);
                    return new MissionStyle(cfg);
                }
            }
            catch
            {
                return new MissionStyle();
            }
        }

        public static void SaveToConfig(string styleFilePath, MissionStyleConfig config)
        {
            if (string.IsNullOrEmpty(styleFilePath))
                throw new ArgumentNullException(nameof(styleFilePath));

            var serializer = new XmlSerializer(typeof(MissionStyleConfig));
            using (var stream = File.Create(styleFilePath))
            {
                serializer.Serialize(stream, config);
            }
        }
    }
}
