using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using System.Drawing;

namespace TurbineStatus
{
    public partial class TurbineStatusUI : Form
    {
        PluginHost Host;
        public TurbineStatusUI(PluginHost host)
        {
            InitializeComponent();
            
            Host = host;

            ThemeManager.ApplyThemeTo(this);
        }

        // Force the window to minimize instead of closing
        private void TurbineStatusUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }

        public void RestoreSplitterDistance()
        {
            var value = Settings.Instance[Text.Replace(" ", "_") + "_SplitterDistance"];
            if (value != null)
            {
                try
                {
                    splitContainer1.SplitterDistance = int.Parse(value);
                }
                catch { }
            }
        }

        public void SaveSplitterDistance()
        {
            Settings.Instance[Text.Replace(" ", "_") + "_SplitterDistance"] = splitContainer1.SplitterDistance.ToString();
        }

        // On resizing, adjust the height of the table layout to always have a 3:2 ratio
        private void table_gauges_Resize(object sender, System.EventArgs e)
        {
            table_gauges.Height = table_gauges.Width * 2 / 3;
        }
    }
}
