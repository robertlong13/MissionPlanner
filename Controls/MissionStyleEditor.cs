using log4net;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Mission;
using System;
using System.Collections;
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

        MissionStyle backupStyle;
        MissionStyle activeStyle;
        Action<MissionStyle> applyStyle;
        bool isLoading = true;

        public MissionStyleEditor(MissionStyle activeStyle, Action<MissionStyle> applyStyle)
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

            this.backupStyle = activeStyle;
            this.activeStyle = new MissionStyle(activeStyle.config);
            this.applyStyle = applyStyle;
            UpdateRulesLists();

            isLoading = false;
        }

        private void UpdateRulesLists()
        {
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

        private void ruleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var senderBox = sender as ListBox;
            rulePropertyEditor.SelectedObject = senderBox.SelectedItem;
        }

        private void styleBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            var selectedPreset = styleBox.SelectedItem.ToString();
            if (presetStyles.ContainsKey(selectedPreset))
            {
                var presetUri = presetStyles[selectedPreset];
                activeStyle = MissionStyle.LoadFromConfig(presetUri?.LocalPath);
                applyStyle.Invoke(activeStyle);
                saveButton.Enabled = (presetUri != null);
            }
            else
            {
                saveButton.Enabled = false;
                log.Error($"Selected preset '{selectedPreset}' not found in preset styles.");
            }
            UpdateRulesLists();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var selectedPreset = styleBox.SelectedItem.ToString();
            if (presetStyles.ContainsKey(selectedPreset))
            {
                var presetUri = presetStyles[selectedPreset];
                if (presetUri == null)
                {
                    log.Error("Cannot save over the default preset.");
                    return;
                }
                MissionStyle.SaveToConfig(presetUri.LocalPath, activeStyle.config);
            }
            else
            {
                log.Error($"Selected preset '{selectedPreset}' not found in preset styles.");
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Mission Style Files (*.mpmissionstyle)|*.mpmissionstyle";
            saveFileDialog.InitialDirectory = Settings.GetUserDataDirectory();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                MissionStyle.SaveToConfig(saveFileDialog.FileName, activeStyle.config);
                // Refresh the presets list
                styleBox.Items.Clear();
                presetStyles.Clear();
                scanForPresets();
                foreach (var preset in presetStyles)
                {
                    styleBox.Items.Add(preset.Key);
                }
                var presetFilename = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                styleBox.SelectedItem = presetFilename;
            }
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            applyStyle.Invoke(activeStyle);
        }

        private ListBox getActiveListBox()
        {
            if (ruleTabs.SelectedTab == segmentRuleTab)
            {
                return segmentRuleListBox;
            }
            else if (ruleTabs.SelectedTab == markerRuleTab)
            {
                return markerRuleListBox;
            }
            return null;
        }

        private void ruleMoveUpButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(-1);
        }

        private void ruleMoveDownButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(1);
        }

        private void MoveSelectedItem(int direction)
        {
            var listBox = getActiveListBox();
            if (listBox == null)
                return;

            var list = listBox.DataSource as IList;
            if (list == null)
                return;

            int index = listBox.SelectedIndex;
            int newIndex = index + direction;

            if (index < 0 || newIndex < 0 || newIndex >= list.Count)
                return;

            var item = list[index];
            list.RemoveAt(index);
            list.Insert(newIndex, item);
            listBox.SelectedIndex = newIndex;
        }
    }
}
