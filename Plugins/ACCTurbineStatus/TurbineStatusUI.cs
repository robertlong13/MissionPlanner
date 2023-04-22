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
using MissionPlanner.Controls;

namespace TurbineStatus
{
    public partial class TurbineStatusUI : Form
    {
        PluginHost Host;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Settings from the config file of all 8 gauges
        List<GaugeSettings> gauge_settings;

        PropertyInfo[] gauge_value_sources = new PropertyInfo[8];
        AGaugeApp.AGauge[] gauges = new AGaugeApp.AGauge[8];

        PropertyInfo relay_status;
        PropertyInfo ecu_mode;
        PropertyInfo turbine_mode;

        HashSet<string> critical_errors = new HashSet<string>(new string[] {
            "Error of internal SPI interface",
            "Error of internal I2C interface",
            "Error of internal UART interface",
            "Error during writing into internal EEPROM",
            "Error of internal SPI interface of measuring processor",
            "Reset of communication processor caused by watchdog",
            "Reset of measuring processor caused by watchdog",
            "Production information are not set correctly",
            "HW overspeed limit of free turbine exceeded (speed sensor A)",
            "HW overspeed limit of free turbine exceeded (speed sensor B)",
            "HW overspeed limit of gas turbine exceeded",
            "Underspeed of the engine not reached in time limit. If measured air intake temperature is lower than -10°C, time limit is automatically increased by 5 sec",
            "Loss of control signals",
            "Internal SW error - unknown command",
            "Internal SW error - task list is full",
            "Internal SW error - stored parameters are inconsistent",
        });
        
        HashSet<string> degraded_errors = new HashSet<string>(new string[] {
            "Error of fuel valve control output",
            "Error of ignition control output",
            "Overload of output for fuel valve",
            "Overload of output for ignition",
            "Error in connection of free turbine speed sensor A or B",
            "Range error of free turbine speed sensor A or B",
            "Error of both free turbine speed sensors",
            "Error of exhaust gas temperature sensor",
            "Error of air intake temperature sensor",
            "Error of driver for fuel pump A",
            "Error of driver for fuel pump B",
            "Initial test of HW overspeed protection of free turbine speed sensor A failed",
            "Initial test of HW overspeed protection of free turbine speed sensor B failed",
            "Initial test of HW overspeed protection of gas turbine failed",
            "Error in connection of starter-generator",
            "Exceeding of exhaust gas temperature limit for a prolonged period",
            "Error of starter-generator",
            "Minimal engine speed not reached in time limit",
            "Permanent loss of oil pressure in gas turbine",
            "Loss of engine speed signal or speed decreased below underspeed",
            "Exceeding of power elements temperature limit",
            "Exceeding of free turbine SW overspeed limit (speed sensor A)",
            "Exceeding of free turbine SW overspeed limit (speed sensor B)",
            "Exceeding SW overspeed limit of gas turbine",
            "Error of torque sensor A",
            "Error of torque sensor B",
            "Error of internal power converter",
            "Permanent loss of oil pressure in reducer",
            "Exceeding of exhaust gas temperature limit during starting proc. for a prolonged period",
            "Exceeding of torque limit for a prolonged period",
            "Control voltage is out of range",
            "Activation of degraded mode",
            "Oil filter clogged in gas turbine",
            "Oil filter clogged in engine reducer",
            "Fuel filter clogged",
            "Presence of chips in oil",
            "Low fuel pressure",
            "Service maintenance of the engine is required",
            "The difference of torque between two sensors is above limit value",
        });

        HashSet<string> warnings = new HashSet<string>(new string[] {
            "Exceeding of exhaust gas temperature limit",
            "Loss of oil pressure in gas turbine",
            "Loss of oil pressure in reducer",
            "Exceeding of exhaust gas temperature limit during starting procedure",
            "Exceeding of torque limit",
            "Loss of actual air data on CAN bus",
            "Command is not allowed",
            "Exceeding current limit in low transistors of driver for starter-generator",
            "Synchronization loss of driver for starter-generator",
            "Zero cross loss of driver for starter-generator",
            "Supply voltage of driver for starter-generator decreased under limit",
            "Supply voltage of driver for starter-generator exceeded limit",
            "Too low current during starting procedure (engine or driver error)",
            "Command not accepted due to hardware error",
            "Command not accepted due to STOP signal from HW stop switch",
            "Measured speeds from free turbine speed sensors are different",
            "Access denied - writing to the register without appropriate access rights",
            "Exceeding speed change of free turbine",
            "Loss of actual collective lever position on CAN bus",
            "Collective lever position is out of range (via CAN bus)",
            "Collective lever position is out of range (via voltage on analog input)",
            "Loss of collective lever position (via CAN bus and analog voltage)",
            "Air intake temperature is out of range",
            "Exceeding of fuel pump speed limit",
            "Repeated start of fuel pump A",
            "Repeated start of fuel pump B",
            "Request for password reset was accepted",
            "Request for password reset failed - wrong password for reset",
            "Internal SW error - EEPROM address is out of range",
        });

        enum Severity
        {
            OK,
            Warning,
            Degraded,
            Critical
        }

        public TurbineStatusUI(PluginHost host)
        {
            InitializeComponent();
            
            Host = host;

            ThemeManager.ApplyThemeTo(this);

            // Select the Stop mode by default
            cmb_mode.SelectedIndex = cmb_mode.FindStringExact("Stop");

            // Find or reserve customfields for relay_status, ecu_mode, and turbine_mode
            relay_status = get_customfield("MAV_TS100RELAY");
            ecu_mode = get_customfield("MAV_TS100_ECU");
            turbine_mode = get_customfield("MAV_TS100_TURB");
            
            LoadSettings();

            // Set up the gauges
            for (int i = 0; i < 8; i++)
            {
                GaugeSettings gs = gauge_settings[i];
                // Look up the gauge control by name
                AGaugeApp.AGauge g = (AGaugeApp.AGauge)Controls.Find($"aGauge{i+1}", true)[0];

                g.Cap_Idx = 0;
                g.CapText = gs.desc;
                g.CapPosition = new Point(gs.desc_pos, 110);

                g.Cap_Idx = 1;
                g.CapText = gs.min.ToString(gs.val_format);
                g.CapPosition = new Point(gs.val_pos, 125);

                g.MinValue = gs.min;
                g.MaxValue = gs.max;
                g.ScaleLinesMajorStepValue = gs.step;
                g.ScaleLinesMinorNumOf = gs.minor;
                g.ScaleNumbersRadius = gs.number_radius;

                g.RangesEnabled[0] = !(gs.green_min is null) || !(gs.green_max is null);
                g.RangesStartValue[0] = (gs.green_min is null) ? gs.min : (float)gs.green_min;
                g.RangesEndValue[0] = (gs.green_max is null) ? gs.max : (float)gs.green_max;

                g.RangesEnabled[1] = !(gs.yellow_min is null) || !(gs.yellow_max is null);
                g.RangesStartValue[1] = (gs.yellow_min is null) ? gs.min : (float)gs.yellow_min;
                g.RangesEndValue[1] = (gs.yellow_max is null) ? gs.max : (float)gs.yellow_max;

                g.RangesEnabled[2] = !(gs.red_min is null) || !(gs.red_max is null);
                g.RangesStartValue[2] = (gs.red_min is null) ? gs.min : (float)gs.red_min;
                g.RangesEndValue[2] = (gs.red_max is null) ? gs.max : (float)gs.red_max;

                // Assign the variable if it exists in CurrentState (otherwise, it will remain null)4
                gauge_value_sources[i] = Host.cs.GetType().GetProperty(gs.variable);
                if (gauge_value_sources[i] is null)
                {
                    gauge_value_sources[i] = get_customfield(gs.variable);
                }

                gauges[i] = g;

                gs.variable_offset = (gs.variable_offset is null) ? 0 : gs.variable_offset;
                gs.variable_scale = (gs.variable_scale is null) ? 1 : gs.variable_scale;
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

        // Searches if a customfield has already been assigned to this name.
        // If it has, it returns the customfield object
        // If not, it reserves one, and returns the newly-reserved customfield object
        private PropertyInfo get_customfield(string name)
        {
            string cfname = "";
            if (CurrentState.custom_field_names.ContainsValue(name))
            {
                cfname = CurrentState.custom_field_names.First(x => x.Value == name).Key;
            }
            else
            {
                for (int j = 0; j < 20; j++)
                {
                    if (!CurrentState.custom_field_names.ContainsKey($"customfield{j}"))
                    {
                        cfname = $"customfield{j}";
                        CurrentState.custom_field_names[cfname] = name;
                        break;
                    }
                }
            }
            
            return Host.cs.GetType().GetProperty(cfname);

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
        DateTime clearerrorstime = DateTime.MinValue;
        Severity highest_severity = Severity.OK;
        bool freeze_scroll = false;
        
        // Updates the UI with the latest data
        private void ui_timer_Tick(object sender, EventArgs e)
        {
            // Don't bother doing anything unless we are connected
            if ((Host.comPort.BaseStream == null || !Host.comPort.BaseStream.IsOpen))
            {
                return;
            }

            var messagetime = Host.cs.messages.LastOrDefault().time;
            if (lastmessagetime != messagetime)
            {
                highest_severity = Severity.OK;
                bool overflow = false;
                string msg = "";
                try
                {
                    StringBuilder message = new StringBuilder();
                    Host.cs.messages.ForEach(x =>
                    {
                        if (overflow || x.message.StartsWith("TS100: "))
                        {
                            if (overflow)
                            {
                                msg += x.message;
                            }
                            else
                            {
                                msg = x.message.Substring(7);
                            }
                            if (x.message.Length == 50)
                            {
                                overflow = true;
                            }
                            else
                            {
                                overflow = false;
                                message.Append(x.time + " : " + msg + "\r\n");
                                if (x.time > clearerrorstime && highest_severity < Severity.Critical)
                                {
                                    if (critical_errors.Contains(msg))
                                    {
                                        highest_severity = Severity.Critical;
                                    }
                                    else if (highest_severity < Severity.Degraded && degraded_errors.Contains(msg))
                                    {
                                        highest_severity = Severity.Degraded;
                                    }
                                    else if (highest_severity < Severity.Warning && warnings.Contains(msg))
                                    {
                                        highest_severity = Severity.Warning;
                                    }
                                }
                            }
                        }
                    });

                    switch(highest_severity)
                    {
                        case Severity.OK:
                            led_errors.On = false;
                            break;
                        case Severity.Warning:
                            led_errors.Color = Color.Yellow;
                            led_errors.On = true;
                            break;
                        case Severity.Degraded:
                            led_errors.Color = Color.Orange;
                            led_errors.On = true;
                            break;
                        case Severity.Critical:
                        default:
                            led_errors.Color = Color.Red;
                            led_errors.On = true;
                            break;
                    }

                    txt_messages.Text = message.ToString();

                    // Autoscroll to the end unless the user is trying to select text
                    if(txt_messages.SelectionLength == 0)
                    {
                        txt_messages.SelectionStart = txt_messages.Text.Length;
                        txt_messages.ScrollToCaret();
                    }

                    lastmessagetime = messagetime;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            // Update states sent in custom fields
            // Update the relay LEDs
            UInt16 flags = (UInt16)((float)relay_status.GetValue(Host.cs));

            led_alternatorconn.On = ((flags) & 0x1) > 0;
            led_oilcooler.On = ((flags >> 1) & 0x1) > 0;
            led_empump.On = ((flags >> 2) & 0x1) > 0;
            led_mainpump.On = ((flags >> 3) & 0x1) > 0;
            led_alternator.On = ((flags >> 4) & 0x1) > 0;
            led_totalstop.On = ((flags >> 5) & 0x3) > 0; // Either of the two bits can be set

            // Update the ECU Mode
            string mode = "UNKNOWN";
            int n = (int)((float)ecu_mode.GetValue(Host.cs));
            if (disp_ecu_modes.ContainsKey(n))
            {
                mode = disp_ecu_modes[n];
            }
            lbl_ecumode.Text = "ECU: " + mode;

            // Update the Turbine Mode
            mode = "UNKNOWN";
            n = (int)((float)turbine_mode.GetValue(Host.cs));
            if (disp_turbine_modes.ContainsKey(n))
            {
                mode = disp_turbine_modes[n];
            }
            lbl_turbinemode.Text = "Turbine: " + mode;

            // Update the gauges
            for (int i = 0; i < gauges.Length; i++)
            {
                if (gauge_value_sources[i] != null)
                {
                    float val = (float)gauge_value_sources[i].GetValue(Host.cs) * (float)gauge_settings[i].variable_scale + (float)gauge_settings[i].variable_offset;

                    gauges[i].Cap_Idx = 1;
                    gauges[i].CapText = val.ToString(gauge_settings[i].val_format);

                    // Constrain the value to the min/max
                    val = Math.Max(val, (float)gauge_settings[i].min);
                    val = Math.Min(val, (float)gauge_settings[i].max);
                    gauges[i].Value0 = val;
                }
            }

            // Disable controls when armed
            cmb_mode.Enabled = !Host.cs.armed;
            but_setmode.Enabled = !Host.cs.armed;
            but_mainpump.Enabled = !led_mainpump.On || !Host.cs.armed;
            but_empump.Enabled = !led_empump.On || !Host.cs.armed;

        }

        // Force the window to minimize instead of closing
        private void TurbineStatusUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
        
        private static bool RectVisible(Rectangle rectangle)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                // Add a 50px buffer to height and width
                var screenbounds = screen.Bounds;
                screenbounds.Inflate(50, 50);
                if (screenbounds.Contains(rectangle)) return true;
            }
            return false;
        }
        
        public void RestoreStartupLocation()
        {
            var value = Settings.Instance[Text.Replace(" ", "_") + "_StartLocation"];

            if (value != null)
            {
                try
                {
                    var fsl = value.FromJSON<ControlHelpers.FormStartLocation>();
                    Location = fsl.Location;
                    Size = fsl.Size;
                    StartPosition = RectVisible(Bounds) ? FormStartPosition.Manual : FormStartPosition.WindowsDefaultLocation;
                    WindowState = fsl.State;
                } catch {}
            }
        }

        public void SaveStartupLocation()
        {
            Rectangle bounds;
            FormWindowState state = WindowState;
            if (state == FormWindowState.Normal)
            {
                bounds = Bounds;
            }
            else
            {
                bounds = RestoreBounds;
                state = FormWindowState.Maximized;
            }
            Settings.Instance[Text.Replace(" ", "_") + "_StartLocation"] = new ControlHelpers.FormStartLocation { Location = bounds.Location, Size = bounds.Size, State = state }.ToJSON();
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
            // Create a pop-up dialog form from scratch
            Form form = new Form();
            form.Text = "Emergency Stop";
            form.Width = 330;
            form.Height = 120;
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.ShowIcon = false;
            form.ShowInTaskbar = false;

            // Create a table layout panel to hold the label and buttons
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.RowCount = 2;
            panel.ColumnCount = 3;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            form.Controls.Add(panel);


            // Add text in center
            Label label = new Label();
            label.Text = "Are you sure you want to " + (led_totalstop.On ? "enable" : "DISABLE") + " the engine?";
            label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            label.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(label, 0, 0);
            panel.SetColumnSpan(label, 3);

            // Add Yes/No buttons on the bottom-right
            Button buttonNo = new MyButton()
            {
                Text = "No",
                Anchor = AnchorStyles.Right,
                DialogResult = DialogResult.No
            };
            panel.Controls.Add(buttonNo, 2, 1);

            Button buttonYes = new MyButton()
            {
                Text = "Yes",
                Anchor = AnchorStyles.Right,
                DialogResult = DialogResult.Yes
            };
            panel.Controls.Add(buttonYes, 1, 1);

            // Make "No" the default choice
            form.AcceptButton = buttonNo;

            // Apply theme
            ThemeManager.ApplyThemeTo(form);

            // Show the dialog and wait for the user to click Yes/No
            if (form.ShowDialog() == DialogResult.Yes)
            {
                send_scripting(6, led_totalstop.On ? 0 : 2);
            }
        }

        private void but_clearerrors_Click(object sender, EventArgs e)
        {
            clearerrorstime = lastmessagetime;
            led_errors.On = false;
        }

        private void txt_messages_MouseDown(object sender, MouseEventArgs e)
        {
            freeze_scroll = true;
        }

        private void txt_messages_MouseUp(object sender, EventArgs e)
        {
            freeze_scroll = false;
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
        public string val_format;
        public int val_pos;
        public int min;
        public int max;
        public int step;
        public int number_radius;
        public int minor;
        public float? green_min;
        public float? green_max;
        public float? yellow_min;
        public float? yellow_max;
        public float? red_min;
        public float? red_max;
    }

}
