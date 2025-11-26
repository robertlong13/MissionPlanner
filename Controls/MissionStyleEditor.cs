using MissionPlanner.Utilities.Mission;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class MissionStyleEditor : Form
    {
        BindingList<SegmentStyleRuleConfig> segmentRules;
        BindingList<MarkerStyleRuleConfig> markerRules;

        public MissionStyleEditor()
        {
            InitializeComponent();
        }
    }
}
