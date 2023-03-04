namespace TurbineStatus
{
    partial class TurbineStatusUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.aGauge1 = new AGaugeApp.AGauge();
            this.aGauge3 = new AGaugeApp.AGauge();
            this.aGauge4 = new AGaugeApp.AGauge();
            this.aGauge6 = new AGaugeApp.AGauge();
            this.txt_messages = new System.Windows.Forms.TextBox();
            this.aGauge2 = new AGaugeApp.AGauge();
            this.aGauge5 = new AGaugeApp.AGauge();
            this.table_gauges = new System.Windows.Forms.TableLayoutPanel();
            this.table_main = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.table_control = new System.Windows.Forms.TableLayoutPanel();
            this.myButton5 = new MissionPlanner.Controls.MyButton();
            this.myButton8 = new MissionPlanner.Controls.MyButton();
            this.myButton6 = new MissionPlanner.Controls.MyButton();
            this.myButton2 = new MissionPlanner.Controls.MyButton();
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.myButton7 = new MissionPlanner.Controls.MyButton();
            this.myButton3 = new MissionPlanner.Controls.MyButton();
            this.myButton4 = new MissionPlanner.Controls.MyButton();
            this.label3 = new System.Windows.Forms.Label();
            this.myButton10 = new MissionPlanner.Controls.MyButton();
            this.myButton9 = new MissionPlanner.Controls.MyButton();
            this.myButton12 = new MissionPlanner.Controls.MyButton();
            this.myButton11 = new MissionPlanner.Controls.MyButton();
            this.label4 = new System.Windows.Forms.Label();
            this.myButton13 = new MissionPlanner.Controls.MyButton();
            this.ledBulb1 = new Bulb.LedBulb();
            this.ledBulb2 = new Bulb.LedBulb();
            this.ledBulb3 = new Bulb.LedBulb();
            this.ledBulb4 = new Bulb.LedBulb();
            this.ledBulb5 = new Bulb.LedBulb();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.table_gauges.SuspendLayout();
            this.table_main.SuspendLayout();
            this.table_control.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // aGauge1
            // 
            this.aGauge1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aGauge1.BackColor = System.Drawing.Color.Transparent;
            this.aGauge1.BackgroundImage = global::ACCTurbineStatus.Properties.Resources.Gaugebg;
            this.aGauge1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.aGauge1.BaseArcColor = System.Drawing.Color.Transparent;
            this.aGauge1.BaseArcRadius = 70;
            this.aGauge1.BaseArcStart = 135;
            this.aGauge1.BaseArcSweep = 270;
            this.aGauge1.BaseArcWidth = 2;
            this.aGauge1.Cap_Idx = ((byte)(0));
            this.aGauge1.CapColor = System.Drawing.Color.White;
            this.aGauge1.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge1.CapPosition = new System.Drawing.Point(67, 110);
            this.aGauge1.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(67, 110),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge1.CapsText = new string[] {
        "N1",
        "",
        "",
        "",
        ""};
            this.aGauge1.CapText = "N1";
            this.aGauge1.Center = new System.Drawing.Point(75, 75);
            this.aGauge1.Location = new System.Drawing.Point(0, 0);
            this.aGauge1.Margin = new System.Windows.Forms.Padding(0);
            this.aGauge1.MaxValue = 120F;
            this.aGauge1.MinValue = 0F;
            this.aGauge1.Name = "aGauge1";
            this.aGauge1.Need_Idx = ((byte)(3));
            this.aGauge1.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.aGauge1.NeedleColor2 = System.Drawing.Color.Brown;
            this.aGauge1.NeedleEnabled = false;
            this.aGauge1.NeedleRadius = 70;
            this.aGauge1.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.aGauge1.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.aGauge1.NeedlesEnabled = new bool[] {
        true,
        false,
        false,
        false};
            this.aGauge1.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.aGauge1.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.aGauge1.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.aGauge1.NeedleType = 0;
            this.aGauge1.NeedleWidth = 2;
            this.aGauge1.Range_Idx = ((byte)(2));
            this.aGauge1.RangeColor = System.Drawing.Color.Red;
            this.aGauge1.RangeEnabled = true;
            this.aGauge1.RangeEndValue = 120F;
            this.aGauge1.RangeInnerRadius = 55;
            this.aGauge1.RangeOuterRadius = 60;
            this.aGauge1.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Green,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Red,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge1.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge1.RangesEndValue = new float[] {
        120F,
        120F,
        120F,
        0F,
        0F};
            this.aGauge1.RangesInnerRadius = new int[] {
        55,
        55,
        55,
        70,
        70};
            this.aGauge1.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.aGauge1.RangesStartValue = new float[] {
        50F,
        102F,
        108F,
        0F,
        0F};
            this.aGauge1.RangeStartValue = 108F;
            this.aGauge1.ScaleLinesInterColor = System.Drawing.Color.White;
            this.aGauge1.ScaleLinesInterInnerRadius = 52;
            this.aGauge1.ScaleLinesInterOuterRadius = 60;
            this.aGauge1.ScaleLinesInterWidth = 1;
            this.aGauge1.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.aGauge1.ScaleLinesMajorInnerRadius = 50;
            this.aGauge1.ScaleLinesMajorOuterRadius = 60;
            this.aGauge1.ScaleLinesMajorStepValue = 20F;
            this.aGauge1.ScaleLinesMajorWidth = 2;
            this.aGauge1.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.aGauge1.ScaleLinesMinorInnerRadius = 55;
            this.aGauge1.ScaleLinesMinorNumOf = 9;
            this.aGauge1.ScaleLinesMinorOuterRadius = 60;
            this.aGauge1.ScaleLinesMinorWidth = 1;
            this.aGauge1.ScaleNumbersColor = System.Drawing.Color.White;
            this.aGauge1.ScaleNumbersFormat = null;
            this.aGauge1.ScaleNumbersRadius = 38;
            this.aGauge1.ScaleNumbersRotation = 0;
            this.aGauge1.ScaleNumbersStartScaleLine = 1;
            this.aGauge1.ScaleNumbersStepScaleLines = 1;
            this.aGauge1.Size = new System.Drawing.Size(252, 252);
            this.aGauge1.TabIndex = 0;
            this.aGauge1.Value = 0F;
            this.aGauge1.Value0 = 0F;
            this.aGauge1.Value1 = 0F;
            this.aGauge1.Value2 = 0F;
            this.aGauge1.Value3 = 0F;
            // 
            // aGauge3
            // 
            this.aGauge3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aGauge3.BackColor = System.Drawing.Color.Transparent;
            this.aGauge3.BackgroundImage = global::ACCTurbineStatus.Properties.Resources.Gaugebg;
            this.aGauge3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.aGauge3.BaseArcColor = System.Drawing.Color.Transparent;
            this.aGauge3.BaseArcRadius = 70;
            this.aGauge3.BaseArcStart = 135;
            this.aGauge3.BaseArcSweep = 270;
            this.aGauge3.BaseArcWidth = 2;
            this.aGauge3.Cap_Idx = ((byte)(0));
            this.aGauge3.CapColor = System.Drawing.Color.White;
            this.aGauge3.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge3.CapPosition = new System.Drawing.Point(58, 110);
            this.aGauge3.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(58, 110),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge3.CapsText = new string[] {
        "Torque",
        "",
        "",
        "",
        ""};
            this.aGauge3.CapText = "Torque";
            this.aGauge3.Center = new System.Drawing.Point(75, 75);
            this.aGauge3.Location = new System.Drawing.Point(479, 0);
            this.aGauge3.Margin = new System.Windows.Forms.Padding(0);
            this.aGauge3.MaxValue = 120F;
            this.aGauge3.MinValue = 0F;
            this.aGauge3.Name = "aGauge3";
            this.aGauge3.Need_Idx = ((byte)(3));
            this.aGauge3.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.aGauge3.NeedleColor2 = System.Drawing.Color.Brown;
            this.aGauge3.NeedleEnabled = false;
            this.aGauge3.NeedleRadius = 70;
            this.aGauge3.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.aGauge3.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.aGauge3.NeedlesEnabled = new bool[] {
        true,
        false,
        false,
        false};
            this.aGauge3.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.aGauge3.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.aGauge3.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.aGauge3.NeedleType = 0;
            this.aGauge3.NeedleWidth = 2;
            this.aGauge3.Range_Idx = ((byte)(2));
            this.aGauge3.RangeColor = System.Drawing.Color.Red;
            this.aGauge3.RangeEnabled = true;
            this.aGauge3.RangeEndValue = 120F;
            this.aGauge3.RangeInnerRadius = 55;
            this.aGauge3.RangeOuterRadius = 60;
            this.aGauge3.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Green,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Red,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge3.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge3.RangesEndValue = new float[] {
        120F,
        120F,
        120F,
        0F,
        0F};
            this.aGauge3.RangesInnerRadius = new int[] {
        55,
        55,
        55,
        70,
        70};
            this.aGauge3.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.aGauge3.RangesStartValue = new float[] {
        50F,
        102F,
        108F,
        0F,
        0F};
            this.aGauge3.RangeStartValue = 108F;
            this.aGauge3.ScaleLinesInterColor = System.Drawing.Color.White;
            this.aGauge3.ScaleLinesInterInnerRadius = 52;
            this.aGauge3.ScaleLinesInterOuterRadius = 60;
            this.aGauge3.ScaleLinesInterWidth = 1;
            this.aGauge3.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.aGauge3.ScaleLinesMajorInnerRadius = 50;
            this.aGauge3.ScaleLinesMajorOuterRadius = 60;
            this.aGauge3.ScaleLinesMajorStepValue = 20F;
            this.aGauge3.ScaleLinesMajorWidth = 2;
            this.aGauge3.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.aGauge3.ScaleLinesMinorInnerRadius = 55;
            this.aGauge3.ScaleLinesMinorNumOf = 9;
            this.aGauge3.ScaleLinesMinorOuterRadius = 60;
            this.aGauge3.ScaleLinesMinorWidth = 1;
            this.aGauge3.ScaleNumbersColor = System.Drawing.Color.White;
            this.aGauge3.ScaleNumbersFormat = null;
            this.aGauge3.ScaleNumbersRadius = 38;
            this.aGauge3.ScaleNumbersRotation = 0;
            this.aGauge3.ScaleNumbersStartScaleLine = 1;
            this.aGauge3.ScaleNumbersStepScaleLines = 1;
            this.aGauge3.Size = new System.Drawing.Size(252, 252);
            this.aGauge3.TabIndex = 1;
            this.aGauge3.Value = 0F;
            this.aGauge3.Value0 = 0F;
            this.aGauge3.Value1 = 0F;
            this.aGauge3.Value2 = 0F;
            this.aGauge3.Value3 = 0F;
            // 
            // aGauge4
            // 
            this.aGauge4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aGauge4.BackColor = System.Drawing.Color.Transparent;
            this.aGauge4.BackgroundImage = global::ACCTurbineStatus.Properties.Resources.Gaugebg;
            this.aGauge4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.aGauge4.BaseArcColor = System.Drawing.Color.Transparent;
            this.aGauge4.BaseArcRadius = 70;
            this.aGauge4.BaseArcStart = 135;
            this.aGauge4.BaseArcSweep = 270;
            this.aGauge4.BaseArcWidth = 2;
            this.aGauge4.Cap_Idx = ((byte)(0));
            this.aGauge4.CapColor = System.Drawing.Color.White;
            this.aGauge4.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge4.CapPosition = new System.Drawing.Point(63, 110);
            this.aGauge4.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(63, 110),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge4.CapsText = new string[] {
        "EGT",
        "",
        "",
        "",
        ""};
            this.aGauge4.CapText = "EGT";
            this.aGauge4.Center = new System.Drawing.Point(75, 75);
            this.aGauge4.Location = new System.Drawing.Point(0, 252);
            this.aGauge4.Margin = new System.Windows.Forms.Padding(0);
            this.aGauge4.MaxValue = 1200F;
            this.aGauge4.MinValue = 0F;
            this.aGauge4.Name = "aGauge4";
            this.aGauge4.Need_Idx = ((byte)(3));
            this.aGauge4.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.aGauge4.NeedleColor2 = System.Drawing.Color.Brown;
            this.aGauge4.NeedleEnabled = false;
            this.aGauge4.NeedleRadius = 70;
            this.aGauge4.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.aGauge4.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.aGauge4.NeedlesEnabled = new bool[] {
        true,
        false,
        false,
        false};
            this.aGauge4.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.aGauge4.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.aGauge4.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.aGauge4.NeedleType = 0;
            this.aGauge4.NeedleWidth = 2;
            this.aGauge4.Range_Idx = ((byte)(2));
            this.aGauge4.RangeColor = System.Drawing.Color.Red;
            this.aGauge4.RangeEnabled = true;
            this.aGauge4.RangeEndValue = 1200F;
            this.aGauge4.RangeInnerRadius = 55;
            this.aGauge4.RangeOuterRadius = 60;
            this.aGauge4.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Green,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Red,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge4.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge4.RangesEndValue = new float[] {
        1200F,
        1200F,
        1200F,
        0F,
        0F};
            this.aGauge4.RangesInnerRadius = new int[] {
        55,
        55,
        55,
        70,
        70};
            this.aGauge4.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.aGauge4.RangesStartValue = new float[] {
        150F,
        600F,
        650F,
        0F,
        0F};
            this.aGauge4.RangeStartValue = 650F;
            this.aGauge4.ScaleLinesInterColor = System.Drawing.Color.White;
            this.aGauge4.ScaleLinesInterInnerRadius = 52;
            this.aGauge4.ScaleLinesInterOuterRadius = 60;
            this.aGauge4.ScaleLinesInterWidth = 1;
            this.aGauge4.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.aGauge4.ScaleLinesMajorInnerRadius = 50;
            this.aGauge4.ScaleLinesMajorOuterRadius = 60;
            this.aGauge4.ScaleLinesMajorStepValue = 200F;
            this.aGauge4.ScaleLinesMajorWidth = 2;
            this.aGauge4.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.aGauge4.ScaleLinesMinorInnerRadius = 55;
            this.aGauge4.ScaleLinesMinorNumOf = 9;
            this.aGauge4.ScaleLinesMinorOuterRadius = 60;
            this.aGauge4.ScaleLinesMinorWidth = 1;
            this.aGauge4.ScaleNumbersColor = System.Drawing.Color.White;
            this.aGauge4.ScaleNumbersFormat = null;
            this.aGauge4.ScaleNumbersRadius = 34;
            this.aGauge4.ScaleNumbersRotation = 0;
            this.aGauge4.ScaleNumbersStartScaleLine = 1;
            this.aGauge4.ScaleNumbersStepScaleLines = 1;
            this.aGauge4.Size = new System.Drawing.Size(252, 252);
            this.aGauge4.TabIndex = 2;
            this.aGauge4.Value = 400F;
            this.aGauge4.Value0 = 0F;
            this.aGauge4.Value1 = 0F;
            this.aGauge4.Value2 = 0F;
            this.aGauge4.Value3 = 400F;
            // 
            // aGauge6
            // 
            this.aGauge6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aGauge6.BackColor = System.Drawing.Color.Transparent;
            this.aGauge6.BackgroundImage = global::ACCTurbineStatus.Properties.Resources.Gaugebg;
            this.aGauge6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.aGauge6.BaseArcColor = System.Drawing.Color.Transparent;
            this.aGauge6.BaseArcRadius = 70;
            this.aGauge6.BaseArcStart = 135;
            this.aGauge6.BaseArcSweep = 270;
            this.aGauge6.BaseArcWidth = 2;
            this.aGauge6.Cap_Idx = ((byte)(0));
            this.aGauge6.CapColor = System.Drawing.Color.White;
            this.aGauge6.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge6.CapPosition = new System.Drawing.Point(50, 110);
            this.aGauge6.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge6.CapsText = new string[] {
        "Fuel Flow",
        "",
        "",
        "",
        ""};
            this.aGauge6.CapText = "Fuel Flow";
            this.aGauge6.Center = new System.Drawing.Point(75, 75);
            this.aGauge6.Location = new System.Drawing.Point(479, 252);
            this.aGauge6.Margin = new System.Windows.Forms.Padding(0);
            this.aGauge6.MaxValue = 120F;
            this.aGauge6.MinValue = 0F;
            this.aGauge6.Name = "aGauge6";
            this.aGauge6.Need_Idx = ((byte)(3));
            this.aGauge6.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.aGauge6.NeedleColor2 = System.Drawing.Color.Brown;
            this.aGauge6.NeedleEnabled = false;
            this.aGauge6.NeedleRadius = 70;
            this.aGauge6.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.aGauge6.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.aGauge6.NeedlesEnabled = new bool[] {
        true,
        false,
        false,
        false};
            this.aGauge6.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.aGauge6.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.aGauge6.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.aGauge6.NeedleType = 0;
            this.aGauge6.NeedleWidth = 2;
            this.aGauge6.Range_Idx = ((byte)(2));
            this.aGauge6.RangeColor = System.Drawing.Color.Red;
            this.aGauge6.RangeEnabled = true;
            this.aGauge6.RangeEndValue = 120F;
            this.aGauge6.RangeInnerRadius = 55;
            this.aGauge6.RangeOuterRadius = 60;
            this.aGauge6.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Green,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Red,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge6.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge6.RangesEndValue = new float[] {
        120F,
        120F,
        120F,
        0F,
        0F};
            this.aGauge6.RangesInnerRadius = new int[] {
        55,
        55,
        55,
        70,
        70};
            this.aGauge6.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.aGauge6.RangesStartValue = new float[] {
        50F,
        102F,
        108F,
        0F,
        0F};
            this.aGauge6.RangeStartValue = 108F;
            this.aGauge6.ScaleLinesInterColor = System.Drawing.Color.White;
            this.aGauge6.ScaleLinesInterInnerRadius = 52;
            this.aGauge6.ScaleLinesInterOuterRadius = 60;
            this.aGauge6.ScaleLinesInterWidth = 1;
            this.aGauge6.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.aGauge6.ScaleLinesMajorInnerRadius = 50;
            this.aGauge6.ScaleLinesMajorOuterRadius = 60;
            this.aGauge6.ScaleLinesMajorStepValue = 20F;
            this.aGauge6.ScaleLinesMajorWidth = 2;
            this.aGauge6.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.aGauge6.ScaleLinesMinorInnerRadius = 55;
            this.aGauge6.ScaleLinesMinorNumOf = 9;
            this.aGauge6.ScaleLinesMinorOuterRadius = 60;
            this.aGauge6.ScaleLinesMinorWidth = 1;
            this.aGauge6.ScaleNumbersColor = System.Drawing.Color.White;
            this.aGauge6.ScaleNumbersFormat = null;
            this.aGauge6.ScaleNumbersRadius = 38;
            this.aGauge6.ScaleNumbersRotation = 0;
            this.aGauge6.ScaleNumbersStartScaleLine = 1;
            this.aGauge6.ScaleNumbersStepScaleLines = 1;
            this.aGauge6.Size = new System.Drawing.Size(252, 252);
            this.aGauge6.TabIndex = 3;
            this.aGauge6.Value = 0F;
            this.aGauge6.Value0 = 0F;
            this.aGauge6.Value1 = 0F;
            this.aGauge6.Value2 = 0F;
            this.aGauge6.Value3 = 0F;
            // 
            // txt_messages
            // 
            this.txt_messages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_messages.Location = new System.Drawing.Point(3, 513);
            this.txt_messages.Multiline = true;
            this.txt_messages.Name = "txt_messages";
            this.txt_messages.ReadOnly = true;
            this.txt_messages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_messages.Size = new System.Drawing.Size(720, 118);
            this.txt_messages.TabIndex = 9;
            // 
            // aGauge2
            // 
            this.aGauge2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aGauge2.BackColor = System.Drawing.Color.Transparent;
            this.aGauge2.BackgroundImage = global::ACCTurbineStatus.Properties.Resources.Gaugebg;
            this.aGauge2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.aGauge2.BaseArcColor = System.Drawing.Color.Transparent;
            this.aGauge2.BaseArcRadius = 70;
            this.aGauge2.BaseArcStart = 135;
            this.aGauge2.BaseArcSweep = 270;
            this.aGauge2.BaseArcWidth = 2;
            this.aGauge2.Cap_Idx = ((byte)(0));
            this.aGauge2.CapColor = System.Drawing.Color.White;
            this.aGauge2.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge2.CapPosition = new System.Drawing.Point(67, 110);
            this.aGauge2.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(67, 110),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge2.CapsText = new string[] {
        "N2",
        "",
        "",
        "",
        ""};
            this.aGauge2.CapText = "N2";
            this.aGauge2.Center = new System.Drawing.Point(75, 75);
            this.aGauge2.Location = new System.Drawing.Point(239, 0);
            this.aGauge2.Margin = new System.Windows.Forms.Padding(0);
            this.aGauge2.MaxValue = 120F;
            this.aGauge2.MinValue = 0F;
            this.aGauge2.Name = "aGauge2";
            this.aGauge2.Need_Idx = ((byte)(3));
            this.aGauge2.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.aGauge2.NeedleColor2 = System.Drawing.Color.Brown;
            this.aGauge2.NeedleEnabled = false;
            this.aGauge2.NeedleRadius = 70;
            this.aGauge2.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.aGauge2.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.aGauge2.NeedlesEnabled = new bool[] {
        true,
        false,
        false,
        false};
            this.aGauge2.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.aGauge2.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.aGauge2.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.aGauge2.NeedleType = 0;
            this.aGauge2.NeedleWidth = 2;
            this.aGauge2.Range_Idx = ((byte)(2));
            this.aGauge2.RangeColor = System.Drawing.Color.Red;
            this.aGauge2.RangeEnabled = true;
            this.aGauge2.RangeEndValue = 120F;
            this.aGauge2.RangeInnerRadius = 55;
            this.aGauge2.RangeOuterRadius = 60;
            this.aGauge2.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Green,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Red,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge2.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge2.RangesEndValue = new float[] {
        120F,
        120F,
        120F,
        0F,
        0F};
            this.aGauge2.RangesInnerRadius = new int[] {
        55,
        55,
        55,
        70,
        70};
            this.aGauge2.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.aGauge2.RangesStartValue = new float[] {
        50F,
        102F,
        108F,
        0F,
        0F};
            this.aGauge2.RangeStartValue = 108F;
            this.aGauge2.ScaleLinesInterColor = System.Drawing.Color.White;
            this.aGauge2.ScaleLinesInterInnerRadius = 52;
            this.aGauge2.ScaleLinesInterOuterRadius = 60;
            this.aGauge2.ScaleLinesInterWidth = 1;
            this.aGauge2.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.aGauge2.ScaleLinesMajorInnerRadius = 50;
            this.aGauge2.ScaleLinesMajorOuterRadius = 60;
            this.aGauge2.ScaleLinesMajorStepValue = 20F;
            this.aGauge2.ScaleLinesMajorWidth = 2;
            this.aGauge2.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.aGauge2.ScaleLinesMinorInnerRadius = 55;
            this.aGauge2.ScaleLinesMinorNumOf = 9;
            this.aGauge2.ScaleLinesMinorOuterRadius = 60;
            this.aGauge2.ScaleLinesMinorWidth = 1;
            this.aGauge2.ScaleNumbersColor = System.Drawing.Color.White;
            this.aGauge2.ScaleNumbersFormat = null;
            this.aGauge2.ScaleNumbersRadius = 38;
            this.aGauge2.ScaleNumbersRotation = 0;
            this.aGauge2.ScaleNumbersStartScaleLine = 1;
            this.aGauge2.ScaleNumbersStepScaleLines = 1;
            this.aGauge2.Size = new System.Drawing.Size(252, 252);
            this.aGauge2.TabIndex = 12;
            this.aGauge2.Value = 0F;
            this.aGauge2.Value0 = 0F;
            this.aGauge2.Value1 = 0F;
            this.aGauge2.Value2 = 0F;
            this.aGauge2.Value3 = 0F;
            // 
            // aGauge5
            // 
            this.aGauge5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aGauge5.BackColor = System.Drawing.Color.Transparent;
            this.aGauge5.BackgroundImage = global::ACCTurbineStatus.Properties.Resources.Gaugebg;
            this.aGauge5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.aGauge5.BaseArcColor = System.Drawing.Color.Transparent;
            this.aGauge5.BaseArcRadius = 70;
            this.aGauge5.BaseArcStart = 135;
            this.aGauge5.BaseArcSweep = 270;
            this.aGauge5.BaseArcWidth = 2;
            this.aGauge5.Cap_Idx = ((byte)(0));
            this.aGauge5.CapColor = System.Drawing.Color.White;
            this.aGauge5.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge5.CapPosition = new System.Drawing.Point(50, 110);
            this.aGauge5.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge5.CapsText = new string[] {
        "Oil Press.",
        "",
        "",
        "",
        ""};
            this.aGauge5.CapText = "Oil Press.";
            this.aGauge5.Center = new System.Drawing.Point(75, 75);
            this.aGauge5.Location = new System.Drawing.Point(239, 252);
            this.aGauge5.Margin = new System.Windows.Forms.Padding(0);
            this.aGauge5.MaxValue = 120F;
            this.aGauge5.MinValue = 0F;
            this.aGauge5.Name = "aGauge5";
            this.aGauge5.Need_Idx = ((byte)(3));
            this.aGauge5.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.aGauge5.NeedleColor2 = System.Drawing.Color.Brown;
            this.aGauge5.NeedleEnabled = false;
            this.aGauge5.NeedleRadius = 70;
            this.aGauge5.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.aGauge5.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.aGauge5.NeedlesEnabled = new bool[] {
        true,
        false,
        false,
        false};
            this.aGauge5.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.aGauge5.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.aGauge5.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.aGauge5.NeedleType = 0;
            this.aGauge5.NeedleWidth = 2;
            this.aGauge5.Range_Idx = ((byte)(2));
            this.aGauge5.RangeColor = System.Drawing.Color.Red;
            this.aGauge5.RangeEnabled = true;
            this.aGauge5.RangeEndValue = 120F;
            this.aGauge5.RangeInnerRadius = 55;
            this.aGauge5.RangeOuterRadius = 60;
            this.aGauge5.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Green,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Red,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge5.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge5.RangesEndValue = new float[] {
        120F,
        120F,
        120F,
        0F,
        0F};
            this.aGauge5.RangesInnerRadius = new int[] {
        55,
        55,
        55,
        70,
        70};
            this.aGauge5.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.aGauge5.RangesStartValue = new float[] {
        50F,
        102F,
        108F,
        0F,
        0F};
            this.aGauge5.RangeStartValue = 108F;
            this.aGauge5.ScaleLinesInterColor = System.Drawing.Color.White;
            this.aGauge5.ScaleLinesInterInnerRadius = 52;
            this.aGauge5.ScaleLinesInterOuterRadius = 60;
            this.aGauge5.ScaleLinesInterWidth = 1;
            this.aGauge5.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.aGauge5.ScaleLinesMajorInnerRadius = 50;
            this.aGauge5.ScaleLinesMajorOuterRadius = 60;
            this.aGauge5.ScaleLinesMajorStepValue = 20F;
            this.aGauge5.ScaleLinesMajorWidth = 2;
            this.aGauge5.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.aGauge5.ScaleLinesMinorInnerRadius = 55;
            this.aGauge5.ScaleLinesMinorNumOf = 9;
            this.aGauge5.ScaleLinesMinorOuterRadius = 60;
            this.aGauge5.ScaleLinesMinorWidth = 1;
            this.aGauge5.ScaleNumbersColor = System.Drawing.Color.White;
            this.aGauge5.ScaleNumbersFormat = null;
            this.aGauge5.ScaleNumbersRadius = 38;
            this.aGauge5.ScaleNumbersRotation = 0;
            this.aGauge5.ScaleNumbersStartScaleLine = 1;
            this.aGauge5.ScaleNumbersStepScaleLines = 1;
            this.aGauge5.Size = new System.Drawing.Size(252, 252);
            this.aGauge5.TabIndex = 13;
            this.aGauge5.Value = 0F;
            this.aGauge5.Value0 = 0F;
            this.aGauge5.Value1 = 0F;
            this.aGauge5.Value2 = 0F;
            this.aGauge5.Value3 = 0F;
            // 
            // table_gauges
            // 
            this.table_gauges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.table_gauges.ColumnCount = 3;
            this.table_gauges.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.table_gauges.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.table_gauges.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.table_gauges.Controls.Add(this.aGauge1, 0, 0);
            this.table_gauges.Controls.Add(this.aGauge2, 1, 0);
            this.table_gauges.Controls.Add(this.aGauge5, 1, 1);
            this.table_gauges.Controls.Add(this.aGauge3, 2, 0);
            this.table_gauges.Controls.Add(this.aGauge4, 0, 1);
            this.table_gauges.Controls.Add(this.aGauge6, 2, 1);
            this.table_gauges.Location = new System.Drawing.Point(3, 3);
            this.table_gauges.Name = "table_gauges";
            this.table_gauges.RowCount = 2;
            this.table_gauges.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.table_gauges.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table_gauges.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.table_gauges.Size = new System.Drawing.Size(720, 504);
            this.table_gauges.TabIndex = 15;
            // 
            // table_main
            // 
            this.table_main.ColumnCount = 1;
            this.table_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.table_main.Controls.Add(this.table_gauges, 0, 0);
            this.table_main.Controls.Add(this.txt_messages, 0, 1);
            this.table_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_main.Location = new System.Drawing.Point(0, 0);
            this.table_main.Name = "table_main";
            this.table_main.RowCount = 2;
            this.table_main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table_main.Size = new System.Drawing.Size(726, 634);
            this.table_main.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.table_control.SetColumnSpan(this.label1, 2);
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "ECU: False Start";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.table_control.SetColumnSpan(this.label2, 2);
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 31);
            this.label2.TabIndex = 1;
            this.label2.Text = "Turbine: Flight";
            // 
            // table_control
            // 
            this.table_control.ColumnCount = 2;
            this.table_control.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table_control.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.table_control.Controls.Add(this.label2, 0, 1);
            this.table_control.Controls.Add(this.label1, 0, 0);
            this.table_control.Controls.Add(this.myButton5, 0, 11);
            this.table_control.Controls.Add(this.myButton8, 0, 10);
            this.table_control.Controls.Add(this.myButton6, 0, 9);
            this.table_control.Controls.Add(this.myButton2, 0, 8);
            this.table_control.Controls.Add(this.myButton1, 0, 7);
            this.table_control.Controls.Add(this.myButton7, 0, 6);
            this.table_control.Controls.Add(this.myButton3, 0, 5);
            this.table_control.Controls.Add(this.myButton4, 0, 4);
            this.table_control.Controls.Add(this.label3, 0, 3);
            this.table_control.Controls.Add(this.myButton10, 0, 17);
            this.table_control.Controls.Add(this.myButton9, 0, 16);
            this.table_control.Controls.Add(this.myButton12, 0, 15);
            this.table_control.Controls.Add(this.myButton11, 0, 14);
            this.table_control.Controls.Add(this.label4, 0, 13);
            this.table_control.Controls.Add(this.myButton13, 0, 19);
            this.table_control.Controls.Add(this.ledBulb1, 1, 14);
            this.table_control.Controls.Add(this.ledBulb2, 1, 15);
            this.table_control.Controls.Add(this.ledBulb3, 1, 16);
            this.table_control.Controls.Add(this.ledBulb4, 1, 17);
            this.table_control.Controls.Add(this.ledBulb5, 1, 19);
            this.table_control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_control.Location = new System.Drawing.Point(0, 0);
            this.table_control.Name = "table_control";
            this.table_control.RowCount = 21;
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142857F));
            this.table_control.Size = new System.Drawing.Size(282, 634);
            this.table_control.TabIndex = 16;
            // 
            // myButton5
            // 
            this.myButton5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton5.Location = new System.Drawing.Point(3, 347);
            this.myButton5.Name = "myButton5";
            this.myButton5.Size = new System.Drawing.Size(247, 27);
            this.myButton5.TabIndex = 6;
            this.myButton5.Text = "Flight";
            this.myButton5.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton5.UseVisualStyleBackColor = true;
            // 
            // myButton8
            // 
            this.myButton8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton8.Location = new System.Drawing.Point(3, 314);
            this.myButton8.Name = "myButton8";
            this.myButton8.Size = new System.Drawing.Size(247, 27);
            this.myButton8.TabIndex = 9;
            this.myButton8.Text = "Idle 2";
            this.myButton8.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton8.UseVisualStyleBackColor = true;
            // 
            // myButton6
            // 
            this.myButton6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton6.Location = new System.Drawing.Point(3, 281);
            this.myButton6.Name = "myButton6";
            this.myButton6.Size = new System.Drawing.Size(247, 27);
            this.myButton6.TabIndex = 7;
            this.myButton6.Text = "Idle 1";
            this.myButton6.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton6.UseVisualStyleBackColor = true;
            // 
            // myButton2
            // 
            this.myButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton2.Location = new System.Drawing.Point(3, 248);
            this.myButton2.Name = "myButton2";
            this.myButton2.Size = new System.Drawing.Size(247, 27);
            this.myButton2.TabIndex = 3;
            this.myButton2.Text = "Cool";
            this.myButton2.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton2.UseVisualStyleBackColor = true;
            // 
            // myButton1
            // 
            this.myButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton1.Location = new System.Drawing.Point(3, 215);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new System.Drawing.Size(247, 27);
            this.myButton1.TabIndex = 2;
            this.myButton1.Text = "Stop";
            this.myButton1.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton1.UseVisualStyleBackColor = true;
            // 
            // myButton7
            // 
            this.myButton7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton7.Location = new System.Drawing.Point(3, 182);
            this.myButton7.Name = "myButton7";
            this.myButton7.Size = new System.Drawing.Size(247, 27);
            this.myButton7.TabIndex = 8;
            this.myButton7.Text = "Start";
            this.myButton7.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton7.UseVisualStyleBackColor = true;
            // 
            // myButton3
            // 
            this.myButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton3.Location = new System.Drawing.Point(3, 149);
            this.myButton3.Name = "myButton3";
            this.myButton3.Size = new System.Drawing.Size(247, 27);
            this.myButton3.TabIndex = 4;
            this.myButton3.Text = "Cold Start";
            this.myButton3.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton3.UseVisualStyleBackColor = true;
            // 
            // myButton4
            // 
            this.myButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton4.Location = new System.Drawing.Point(3, 116);
            this.myButton4.Name = "myButton4";
            this.myButton4.Size = new System.Drawing.Size(247, 27);
            this.myButton4.TabIndex = 5;
            this.myButton4.Text = "False Start";
            this.myButton4.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton4.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Set Mode";
            // 
            // myButton10
            // 
            this.myButton10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton10.Location = new System.Drawing.Point(3, 512);
            this.myButton10.Name = "myButton10";
            this.myButton10.Size = new System.Drawing.Size(247, 27);
            this.myButton10.TabIndex = 13;
            this.myButton10.Text = "Alternator";
            this.myButton10.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton10.UseVisualStyleBackColor = true;
            // 
            // myButton9
            // 
            this.myButton9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton9.Location = new System.Drawing.Point(3, 479);
            this.myButton9.Name = "myButton9";
            this.myButton9.Size = new System.Drawing.Size(247, 27);
            this.myButton9.TabIndex = 12;
            this.myButton9.Text = "Alter. Conn.";
            this.myButton9.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton9.UseVisualStyleBackColor = true;
            // 
            // myButton12
            // 
            this.myButton12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton12.Location = new System.Drawing.Point(3, 446);
            this.myButton12.Name = "myButton12";
            this.myButton12.Size = new System.Drawing.Size(247, 27);
            this.myButton12.TabIndex = 15;
            this.myButton12.Text = "Em.Pump";
            this.myButton12.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton12.UseVisualStyleBackColor = true;
            // 
            // myButton11
            // 
            this.myButton11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton11.Location = new System.Drawing.Point(3, 413);
            this.myButton11.Name = "myButton11";
            this.myButton11.Size = new System.Drawing.Size(247, 27);
            this.myButton11.TabIndex = 14;
            this.myButton11.Text = "Main Pump";
            this.myButton11.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton11.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 397);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Relays";
            // 
            // myButton13
            // 
            this.myButton13.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton13.Location = new System.Drawing.Point(3, 565);
            this.myButton13.Name = "myButton13";
            this.myButton13.Size = new System.Drawing.Size(247, 27);
            this.myButton13.TabIndex = 16;
            this.myButton13.Text = "Total Stop";
            this.myButton13.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton13.UseVisualStyleBackColor = true;
            // 
            // ledBulb1
            // 
            this.ledBulb1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ledBulb1.Location = new System.Drawing.Point(256, 415);
            this.ledBulb1.Name = "ledBulb1";
            this.ledBulb1.On = true;
            this.ledBulb1.Size = new System.Drawing.Size(23, 23);
            this.ledBulb1.TabIndex = 17;
            this.ledBulb1.Text = "ledBulb1";
            // 
            // ledBulb2
            // 
            this.ledBulb2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ledBulb2.Location = new System.Drawing.Point(256, 448);
            this.ledBulb2.Name = "ledBulb2";
            this.ledBulb2.On = true;
            this.ledBulb2.Size = new System.Drawing.Size(23, 23);
            this.ledBulb2.TabIndex = 18;
            this.ledBulb2.Text = "ledBulb2";
            // 
            // ledBulb3
            // 
            this.ledBulb3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ledBulb3.Location = new System.Drawing.Point(256, 481);
            this.ledBulb3.Name = "ledBulb3";
            this.ledBulb3.On = true;
            this.ledBulb3.Size = new System.Drawing.Size(23, 23);
            this.ledBulb3.TabIndex = 19;
            this.ledBulb3.Text = "ledBulb3";
            // 
            // ledBulb4
            // 
            this.ledBulb4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ledBulb4.Location = new System.Drawing.Point(256, 514);
            this.ledBulb4.Name = "ledBulb4";
            this.ledBulb4.On = true;
            this.ledBulb4.Size = new System.Drawing.Size(23, 23);
            this.ledBulb4.TabIndex = 20;
            this.ledBulb4.Text = "ledBulb4";
            // 
            // ledBulb5
            // 
            this.ledBulb5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ledBulb5.Location = new System.Drawing.Point(256, 567);
            this.ledBulb5.Name = "ledBulb5";
            this.ledBulb5.On = true;
            this.ledBulb5.Size = new System.Drawing.Size(23, 23);
            this.ledBulb5.TabIndex = 21;
            this.ledBulb5.Text = "ledBulb5";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.table_main);
            this.splitContainer1.Panel1.Resize += new System.EventHandler(this.table_gauges_Resize);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.table_control);
            this.splitContainer1.Size = new System.Drawing.Size(1012, 634);
            this.splitContainer1.SplitterDistance = 726;
            this.splitContainer1.TabIndex = 16;
            // 
            // TurbineStatusUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 634);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TurbineStatusUI";
            this.ShowIcon = false;
            this.Text = "TurbineStatusUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TurbineStatusUI_FormClosing);
            this.Resize += new System.EventHandler(this.table_gauges_Resize);
            this.table_gauges.ResumeLayout(false);
            this.table_main.ResumeLayout(false);
            this.table_main.PerformLayout();
            this.table_control.ResumeLayout(false);
            this.table_control.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AGaugeApp.AGauge aGauge1;
        private AGaugeApp.AGauge aGauge3;
        private AGaugeApp.AGauge aGauge4;
        private AGaugeApp.AGauge aGauge6;
        private System.Windows.Forms.TextBox txt_messages;
        private AGaugeApp.AGauge aGauge2;
        private AGaugeApp.AGauge aGauge5;
        private System.Windows.Forms.TableLayoutPanel table_gauges;
        private System.Windows.Forms.TableLayoutPanel table_main;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel table_control;
        private MissionPlanner.Controls.MyButton myButton6;
        private MissionPlanner.Controls.MyButton myButton4;
        private MissionPlanner.Controls.MyButton myButton2;
        private MissionPlanner.Controls.MyButton myButton7;
        private MissionPlanner.Controls.MyButton myButton3;
        private MissionPlanner.Controls.MyButton myButton1;
        private MissionPlanner.Controls.MyButton myButton8;
        private MissionPlanner.Controls.MyButton myButton5;
        private System.Windows.Forms.Label label3;
        private MissionPlanner.Controls.MyButton myButton10;
        private MissionPlanner.Controls.MyButton myButton9;
        private MissionPlanner.Controls.MyButton myButton12;
        private MissionPlanner.Controls.MyButton myButton11;
        private System.Windows.Forms.Label label4;
        private MissionPlanner.Controls.MyButton myButton13;
        private Bulb.LedBulb ledBulb1;
        private Bulb.LedBulb ledBulb2;
        private Bulb.LedBulb ledBulb3;
        private Bulb.LedBulb ledBulb4;
        private Bulb.LedBulb ledBulb5;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}