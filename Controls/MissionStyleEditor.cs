using log4net;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Mission;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class MissionStyleEditor : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        BindingList<SegmentStyleRuleConfig> segmentRules;
        BindingList<MarkerStyleRuleConfig> markerRules;

        Dictionary<string, Uri> presetStyles = new Dictionary<string, Uri>()
        {
            { "Default", null },
        };

        public MissionStyleEditor(MissionStyle activeStyle, Action applyStyle)
        {
            InitializeComponent();

            scanForPresets();

            foreach (var preset in presetStyles)
            {
                styleBox.Items.Add(preset.Key);
            }
            var presetFilename = Path.GetFileNameWithoutExtension(Settings.Instance["missionstyle", "Default"]);
            if (styleBox.Items.Contains(presetFilename))
            {
                styleBox.SelectedItem = presetFilename;
            }
            else
            {
                styleBox.SelectedIndex = 0;
            }

            segmentRules = new BindingList<SegmentStyleRuleConfig>(activeStyle.config.SegmentRules);
            markerRules = new BindingList<MarkerStyleRuleConfig>(activeStyle.config.MarkerRules);
            segmentRuleListBox.DataSource = segmentRules;
            segmentRuleListBox.DisplayMember = "Description";
            segmentRuleListBox.SelectedIndex = -1;
            markerRuleListBox.DataSource = markerRules;
            markerRuleListBox.DisplayMember = "Description";
            markerRuleListBox.SelectedIndex = -1;

            rulePropertyEditor.SelectedObject = null;
        }

        private void scanForPresets()
        {
            var presetDir = Settings.GetUserDataDirectory();
            if (Directory.Exists(presetDir))
            {
                var files = Directory.GetFiles(presetDir, "*.mpmissionstyle");
                foreach (var file in files)
                {
                    var presetName = Path.GetFileNameWithoutExtension(file);
                    if (presetName == "Default")
                    {
                        log.Warn("Ignoring preset named 'Default' as it is reserved.");
                        continue;
                    }
                    presetStyles[Path.GetFileNameWithoutExtension(file)] = new Uri(file);
                }
            }
        }

        private void markerRuleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            rulePropertyEditor.SelectedObject = markerRuleListBox.SelectedItem;
        }

        private void segmentRuleListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            rulePropertyEditor.SelectedObject = segmentRuleListBox.SelectedItem;
        }
    }
}
