using MissionPlanner.Controls;
using MissionPlanner.Plugin;

namespace TurbineStatus
{
    public class TurbineStatusPlugin : Plugin
    {
        private string _Name = "Turbine Status";
        private string _Version = "1.0";
        private string _Author = "Bob Long";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }

        private TurbineStatusUI uiform;

        public override bool Init() { return true; }

        public override bool Loaded()
        {
            // Open the TurbineStatus UI
            uiform = new TurbineStatusUI(Host);
            uiform.RestoreStartupLocation();
            uiform.Show();
            uiform.RestoreSplitterDistance();

            //loopratehz = 1;

            return true;
        }

        public override bool Exit()
        {
            uiform.SaveStartupLocation();
            uiform.SaveSplitterDistance();
            return true;
        }

        /*public override bool Loop()
        {
            // Print uiform location and size to console
            System.Console.WriteLine("uiform.Location: " + uiform.Location.ToString());
            System.Console.WriteLine("uiform.Size: " + uiform.Size.ToString());
            System.Console.WriteLine("uiform.WindowState: " + uiform.WindowState.ToString());
            System.Console.WriteLine("uiform.RestoreBounds: " + uiform.RestoreBounds.ToString());
            return true;
        }*/
    }
}