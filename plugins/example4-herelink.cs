using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using MissionPlanner.Controls;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using System.Drawing;
using GMap.NET.WindowsForms;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;


namespace CameraControl
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        private int sub;
        private int sub1;
        private int sub2;
        private int sub3;
        private int sub4;

        public override string Name
        {
            get { return "Camera Control"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            return true;
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}