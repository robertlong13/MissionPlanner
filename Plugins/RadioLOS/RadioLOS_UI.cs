using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Controls;
using MissionPlanner.Plugin;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Utilities;

namespace RadioLOS
{
    public partial class RadioLOS_UI : Form
    {
        PluginHost Host;

        static GMapOverlay DataPageOverlay;
        static GMapOverlay PlanPageOverlay;

        double multiplierdistbig = 1/1000.0; // For converting ft to mi or m to km

        public RadioLOS_UI(PluginHost host)
        {
            Host = host;
            
            InitializeComponent();

            LoadSettings();

            if (DataPageOverlay == null)
            {
                DataPageOverlay = new GMapOverlay("RadioLOS");
                Host.FDGMapControl.Overlays.Insert(0, DataPageOverlay);
            }
            if (PlanPageOverlay == null)
            {
                PlanPageOverlay = new GMapOverlay("RadioLOS");
                Host.FPGMapControl.Overlays.Insert(0, PlanPageOverlay);
            }

            // Use reflection to force doublebuffering on the progress bar
            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(progressBar, true, null);

            // Set unit labels
            lbl_altunit1.Text = CurrentState.AltUnit;
            lbl_altunit2.Text = CurrentState.AltUnit;
            lbl_altunit3.Text = CurrentState.AltUnit;

            // It actually makes the most sense to use the speed unit to infer the big distance unit
            if (CurrentState.SpeedUnit == "kts")
            {
                lbl_distbigunit.Text = "nm";
                multiplierdistbig = 1 / 1852.0;
            }
            else if (CurrentState.SpeedUnit == "fps" || CurrentState.SpeedUnit == "mph")
            {
                lbl_distbigunit.Text = "mi";
                multiplierdistbig = 1 / 1609.344; 
            }
            else
            {
                lbl_distbigunit.Text = "km";
                multiplierdistbig = 1 / 1000.0;
            }
        }

        // Settings are saved and loaded without unit conversion. People don't
        // switch between units that frequently to justify the pain.
        void LoadSettings()
        {
            num_range.Value = GetDecimal("RadioLOS_UI.num_range", num_range.Value);
            num_mast_height.Value = GetDecimal("RadioLOS_UI.num_mast_height", num_mast_height.Value);
            num_start_az.Value = GetDecimal("RadioLOS_UI.num_start_az", num_start_az.Value);
            num_stop_az.Value = GetDecimal("RadioLOS_UI.num_stop_az", num_stop_az.Value);
            num_clear_angle.Value = GetDecimal("RadioLOS_UI.num_clear_angle", num_clear_angle.Value);
            num_clear_terrain.Value = GetDecimal("RadioLOS_UI.num_clear_terrain", num_clear_terrain.Value);
            num_az_step.Value = GetDecimal("RadioLOS_UI.num_az_step", num_az_step.Value);
            num_dist_step.Value = GetDecimal("RadioLOS_UI.num_dist_step", num_dist_step.Value);
            num_tolerance.Value = GetDecimal("RadioLOS_UI.num_tolerance", num_tolerance.Value);

            // Grab the default flight_altitude and flight_altitude mode from the FlightPlanner page
            if (decimal.TryParse(Host.MainForm.FlightPlanner.TXT_DefaultAlt.Text, out decimal altitude))
            {
                num_altitude.Value = altitude;
            }
            if ((MissionPlanner.GCSViews.FlightPlanner.altmode)Host.MainForm.FlightPlanner.CMB_altmode.SelectedValue == MissionPlanner.GCSViews.FlightPlanner.altmode.Absolute)
            {
                rad_msl.Checked = true;
            }
            else
            {
                rad_ahl.Checked = true;
            }
        }

        private decimal GetDecimal(string key, decimal defaultd = 0)
        {
            decimal result;
            if (Host.config.ContainsKey(key) && decimal.TryParse(Host.config[key], out result))
            {
                return result;
            }
            return defaultd;
        }

        void SaveSettings()
        {
            Host.config["RadioLOS_UI.num_range"] = num_range.Value.ToString();
            Host.config["RadioLOS_UI.num_mast_height"] = num_mast_height.Value.ToString();
            Host.config["RadioLOS_UI.num_start_az"] = num_start_az.Value.ToString();
            Host.config["RadioLOS_UI.num_stop_az"] = num_stop_az.Value.ToString();
            Host.config["RadioLOS_UI.num_clear_angle"] = num_clear_angle.Value.ToString();
            Host.config["RadioLOS_UI.num_clear_terrain"] = num_clear_terrain.Value.ToString();
            Host.config["RadioLOS_UI.num_az_step"] = num_az_step.Value.ToString();
            Host.config["RadioLOS_UI.num_dist_step"] = num_dist_step.Value.ToString();
            Host.config["RadioLOS_UI.num_tolerance"] = num_tolerance.Value.ToString();
        }

        private void num_az_startstop_ValueChanged(object sender, EventArgs e)
        {
            decimal value = ((NumericUpDown)sender).Value;
            if (value >= 360)
            {
                value -= 360;
            }
            else if (value < 0)
            {
                value += 360;
            }
            ((NumericUpDown)sender).Value = value;
        }

        private async void but_generate_Click(object sender, EventArgs e)
        {
            but_generate.Enabled = false;
            
            var progress = new Progress<int>(percentage => { progressBar.Value = percentage; });

            var options = new RadioLOSOptions()
            {
                base_height = (double)num_mast_height.Value / CurrentState.multiplieralt,
                clearance_angle = (double)num_clear_angle.Value,
                clearance_terrain = (double)num_clear_terrain.Value / CurrentState.multiplieralt,
                start_azimuth = (double)num_start_az.Value,
                stop_azimuth = (double)num_stop_az.Value,
                azimuth_step = (double)num_az_step.Value,
                distance_step = (double)num_dist_step.Value, // We don't convert the distance step unit.
                angle_tolerance = (double)num_tolerance.Value,
            };


            PointLatLngAlt home = new PointLatLngAlt(Host.cs.PlannedHomeLocation);
            home.Alt /= CurrentState.multiplieralt; // PlannedHomeLocation has displayunit altitude, unlike HomeLocation
            double flight_altitude = (double)num_altitude.Value / CurrentState.multiplieralt;
            if (rad_ahl.Checked) flight_altitude += home.Alt;

            double range = (double)num_range.Value / multiplierdistbig;

            var radioLOS = new RadioLOS();
            List<PointLatLng> allowableFlightZone = await radioLOS.CalculateAllowableFlightZoneAsync(
                range, flight_altitude, home, options, progress);

            GMapPolygon poly = new GMapPolygon(allowableFlightZone, "LOS")
            {
                Fill = Brushes.Transparent,
                Stroke = new Pen(Color.MediumOrchid, 3) { DashStyle = DashStyle.Dash }
            };

            Host.FDMenuMap.BeginInvokeIfRequired((Action) delegate
            {
                DataPageOverlay.Polygons.Clear();
                DataPageOverlay.Polygons.Add(poly);
            });

            Host.FPMenuMap.BeginInvokeIfRequired((Action)delegate
            {
                PlanPageOverlay.Polygons.Clear();
                PlanPageOverlay.Polygons.Add(poly);
            });

            SaveSettings();

            progressBar.Value = 0;

            but_generate.Enabled = true;
        }
    }
}
