using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

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

            // Select the Stop mode by default
            cmb_mode.SelectedIndex = cmb_mode.FindStringExact("Stop");

            // Bind event to incoming MAVLink messages
            //Host.comPort.OnPacketReceived += MavlinkMessageReceived;
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

        // On resizing, adjust the height of the gauges table to always have a 8:15 ratio
        private void table_gauges_Resize(object sender, EventArgs e)
        {
            table_gauges.Height = table_gauges.Width * 8 / 15;

            table_control_Resize(sender, e);
        }

        // Adjust the font size of the mode labels to fit their new width
        private void table_control_Resize(object sender, EventArgs e)
        {
            // Use 24pt font for table_control.Width = 280 and scale up/down from there
            float fontSize = 24 * table_control.Width / 280;
            // Prevent 0 font size
            fontSize = fontSize > 0 ? fontSize : 24;
            
            lbl_ecumode.Font = new Font(lbl_ecumode.Font.FontFamily, fontSize);
            lbl_turbinemode.Font = new Font(lbl_turbinemode.Font.FontFamily, fontSize);
        }

        private void send_scripting(int n, int val)
        {
            // Send do_aux message
            try
            {
                if (!Host.comPort.doCommand(
                    (byte)Host.comPort.sysidcurrent,
                    (byte)Host.comPort.compidcurrent,
                    MAVLink.MAV_CMD.DO_AUX_FUNCTION,
                    299 + n, // 300 is the Scripting1 function
                    val,
                    0, 0, 0, 0, 0))
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        // Set Modes enum (different from the set_modes reported by the ECU)
        private static readonly Dictionary<string, int> set_modes = new Dictionary<string, int>
        {
            { "False Start", 7 },
            { "Start", 8 },
            { "Stop", 9 },
            { "Cool", 10 },
            { "Cold Start", 11 },
            { "Idle 1", 12 },
            { "Flight", 13 },
            { "Idle 2", 14 }
        };

        private void but_setmode_Click(object sender, EventArgs e)
        {
            Console.WriteLine(cmb_mode.Text + " " + set_modes[cmb_mode.Text].ToString());
            send_scripting(set_modes[cmb_mode.Text], 2);
        }

        private void but_mainpump_Click(object sender, EventArgs e)
        {
            send_scripting(4, led_mainpump.On ? 0 : 2);
        }

        private void but_empump_Click(object sender, EventArgs e)
        {
            send_scripting(3, led_empump.On ? 0 : 2);
        }

        private void but_alternatorconn_Click(object sender, EventArgs e)
        {
            send_scripting(1, led_alternatorconn.On ? 0 : 2);
        }

        private void but_alternator_Click(object sender, EventArgs e)
        {
            send_scripting(5, led_alternator.On ? 0 : 2);
        }

        private void but_oilcooler_Click(object sender, EventArgs e)
        {
            send_scripting(2, led_oilcooler.On ? 0 : 2);
        }

        private void but_totalstop_Click(object sender, EventArgs e)
        {
            send_scripting(6, led_totalstop.On ? 0 : 2);
        }
    }
}
