using System;
using MissionPlanner.Plugin;
using System.Windows.Forms;
using System.Reflection;
using log4net;

namespace RadioLOS
{
    public class RadioLOS_Plugin : Plugin
    {
        private string _Name = "Radio LOS Calculator";
        private string _Version = "0.4";
        private string _Author = "Bob Long";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override bool Init() { return true; }

        public override bool Loaded()
        {
            // Add a menu item under Map Tool menu in the FlightPlanner page
            ((ToolStripMenuItem)Host.FPMenuMap.Items["mapToolToolStripMenuItem"])
                .DropDownItems.Add("Radio LOS Calculator", null, new EventHandler(
                    delegate (object sender, EventArgs e)
                    {
                        // Create a new instance of the form,
                        RadioLOS_UI form = new RadioLOS_UI(Host)
                        {
                            StartPosition = FormStartPosition.Manual,
                        };
                        // Set the start position to be over the center of MainForm
                        var loc = new System.Drawing.Point(
                            Host.MainForm.Location.X + Host.MainForm.Width / 2 - form.Width / 2,
                            Host.MainForm.Location.Y + Host.MainForm.Height / 2 - form.Height / 2
                        );
                        form.Location = loc;

                        // Show the form
                        form.Show();
                    }
                ));

            return true;
        }

        public override bool Exit() { return true; }
    }
}