using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using log4net;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace TurbineStatus
{
    public partial class TurbineStatusUI : Form
    {
        PluginHost Host;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Settings from the config file of all 8 gauges
        public static List<GaugeSettings> gauge_settings;

        public List<object> gauge_variables;

        public TurbineStatusUI(PluginHost host)
        {
            InitializeComponent();
            
            Host = host;

            ThemeManager.ApplyThemeTo(this);

            // Select the Stop mode by default
            cmb_mode.SelectedIndex = cmb_mode.FindStringExact("Stop");

            LoadSettings();

            // Set up the gauges
            for (int i = 0; i < 8; i++)
            {
                GaugeSettings gs = gauge_settings[i];
                // Look up the gauge control by name
                AGaugeApp.AGauge g = (AGaugeApp.AGauge)Controls.Find($"aGauge{i+1}", true)[0];
                g.Cap_Idx = 0;
                g.CapText = gs.desc;
                g.CapPosition = new Point(gs.desc_pos, g.CapPosition.Y);
                
                g.MinValue = gs.min;
                g.MaxValue = gs.max;
                g.ScaleLinesMajorStepValue = gs.step;
                g.ScaleLinesMinorNumOf = gs.minor;

                g.RangesEnabled[0] = !(gs.green_min is null) || !(gs.green_max is null);
                g.RangesStartValue[0] = (gs.green_min is null) ? gs.min : (float)gs.green_min;
                g.RangesEndValue[0] = (gs.green_max is null) ? gs.max : (float)gs.green_max;

                g.RangesEnabled[1] = !(gs.yellow_min is null) || !(gs.yellow_max is null);
                g.RangesStartValue[1] = (gs.yellow_min is null) ? gs.min : (float)gs.yellow_min;
                g.RangesEndValue[1] = (gs.yellow_max is null) ? gs.max : (float)gs.yellow_max;

                g.RangesEnabled[2] = !(gs.red_min is null) || !(gs.red_max is null);
                g.RangesStartValue[2] = (gs.red_min is null) ? gs.min : (float)gs.red_min;
                g.RangesEndValue[2] = (gs.red_max is null) ? gs.max : (float)gs.red_max;
            }
        }

        private void LoadSettings()
        {
            string settings_file = Path.Combine(Settings.GetUserDataDirectory(), "ACCTurbineStatus.json");
            bool needs_new_file = !File.Exists(settings_file);

#if DEBUG
            // For debugging, always load settings from embedded file
            // This is so the json file in the git repo always matches what I am testing
            needs_new_file = true;
#endif

            if (needs_new_file)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("ACCTurbineStatus.ACCTurbineStatus.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string s = reader.ReadToEnd();
                    File.WriteAllText(settings_file, s);
                }
            }

            gauge_settings = JsonConvert.DeserializeObject<List<GaugeSettings>>(File.ReadAllText(settings_file));

        }

        private static readonly Dictionary<int, string> disp_ecu_modes = new Dictionary<int, string>
        {
            { 0, "Stop" },
            { 1, "Cool" },
            { 2, "Cold Start" },
            { 3, "False Start" },
            { 4, "Run" },
            { 5, "Degraded N1" },
            { 6, "Start" },
            { 7, "Degraded N2" },
            { 255, "Initializing" }
        };

        private static readonly Dictionary<int, string> disp_turbine_modes = new Dictionary<int, string>
        {
            { 0, "Idle 1" },
            { 1, "Flight" },
            { 2, "Idle 2" },
            { 6, "Run Up" },
        };

        DateTime lastmessagetime;
        private void timer1_Tick(object sender, EventArgs e)
        {
            var messagetime = Host.cs.messages.LastOrDefault().time;
            if (lastmessagetime != messagetime)
            {
                try
                {
                    StringBuilder message = new StringBuilder();
                    Host.cs.messages.ForEach(x =>
                    {
                        if (x.Item2.StartsWith("TS100: "))
                        {
                            message.Insert(0, x.Item1 + " : " + x.Item2.Substring(7) + "\r\n");
                        }
                    });
                    txt_messages.Text = message.ToString();

                    lastmessagetime = messagetime;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            // Update states sent in custom fields
            foreach (KeyValuePair<string, string> kvp in CurrentState.custom_field_names)
            {
                // Update the relay LEDs
                if (kvp.Value == "MAV_TS100RELAY")
                {
                    // Get the value of the custom field by name
                    UInt16 flags = (UInt16)((float)Host.cs.GetPropertyOrField(kvp.Key));

                    led_alternatorconn.On = ((flags) & 0x1) > 0;
                    led_oilcooler.On = ((flags >> 1) & 0x1) > 0;
                    led_empump.On = ((flags >> 2) & 0x1) > 0;
                    led_mainpump.On = ((flags >> 3) & 0x1) > 0;
                    led_alternator.On = ((flags >> 4) & 0x1) > 0;
                    led_totalstop.On = ((flags >> 5) & 0x3) > 0; // Either of the two bits can be set
                }
                else if (kvp.Value == "MAV_TS100_ECU")
                {
                    string mode = "UNKNOWN";
                    int n = (int)((float)Host.cs.GetPropertyOrField(kvp.Key));
                    if (disp_ecu_modes.ContainsKey(n))
                    {
                        mode = disp_ecu_modes[n];
                    }
                    lbl_ecumode.Text = "ECU: " + mode;
                }
                else if (kvp.Value == "MAV_TS100_TURB")
                {
                    string mode = "UNKNOWN";
                    int n = (int)((float)Host.cs.GetPropertyOrField(kvp.Key));
                    if (disp_turbine_modes.ContainsKey(n))
                    {
                        mode = disp_turbine_modes[n];
                    }
                    // Set the mode lable across thread
                    lbl_turbinemode.Text = "Turbine: " + mode;
                }
            }
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

    // Create gauge settings struct for config file
    public class GaugeSettings
    {
        public string variable;
        public float? variable_offset;
        public float? variable_scale;
        public string desc;
        public int desc_pos;
        public int min;
        public int max;
        public int step;
        public int minor;
        public float? green_min;
        public float? green_max;
        public float? yellow_min;
        public float? yellow_max;
        public float? red_min;
        public float? red_max;
    }

}
