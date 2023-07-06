namespace RadioLOS
{
    partial class RadioLOS_UI
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.num_stop_az = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.num_start_az = new System.Windows.Forms.NumericUpDown();
            this.lbl_meters_do_not_convert = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.num_dist_step = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbl_altunit3 = new System.Windows.Forms.Label();
            this.num_clear_terrain = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.num_clear_angle = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_altunit2 = new System.Windows.Forms.Label();
            this.num_mast_height = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.rad_ahl = new System.Windows.Forms.RadioButton();
            this.rad_msl = new System.Windows.Forms.RadioButton();
            this.lbl_altunit1 = new System.Windows.Forms.Label();
            this.lbl_distbigunit = new System.Windows.Forms.Label();
            this.num_altitude = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.num_range = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.num_tolerance = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.num_az_step = new System.Windows.Forms.NumericUpDown();
            this.but_generate = new MissionPlanner.Controls.MyButton();
            this.progressBar = new MissionPlanner.Controls.MyProgressBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_stop_az)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_start_az)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_dist_step)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_clear_terrain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_clear_angle)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_mast_height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_altitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_range)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_tolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_az_step)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.num_stop_az);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.num_start_az);
            this.groupBox1.Location = new System.Drawing.Point(241, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(125, 101);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Azimuth";
            this.toolTip1.SetToolTip(this.groupBox1, "Controls the area of interest to calculate.\r\nReducing this to only cover the area" +
        " of interest\r\ncan significantly increase calculation speed.");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(91, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "deg";
            this.toolTip1.SetToolTip(this.label7, "End of clockwise arc");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(91, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "deg";
            this.toolTip1.SetToolTip(this.label4, "Start angle of a clockwise arc");
            // 
            // num_stop_az
            // 
            this.num_stop_az.Location = new System.Drawing.Point(41, 49);
            this.num_stop_az.Maximum = new decimal(new int[] {
            370,
            0,
            0,
            0});
            this.num_stop_az.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.num_stop_az.Name = "num_stop_az";
            this.num_stop_az.Size = new System.Drawing.Size(44, 20);
            this.num_stop_az.TabIndex = 3;
            this.toolTip1.SetToolTip(this.num_stop_az, "End of clockwise arc");
            this.num_stop_az.ValueChanged += new System.EventHandler(this.num_az_startstop_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Stop";
            this.toolTip1.SetToolTip(this.label2, "End of clockwise arc");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start";
            this.toolTip1.SetToolTip(this.label1, "Start angle of a clockwise arc");
            // 
            // num_start_az
            // 
            this.num_start_az.Location = new System.Drawing.Point(41, 23);
            this.num_start_az.Maximum = new decimal(new int[] {
            370,
            0,
            0,
            0});
            this.num_start_az.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.num_start_az.Name = "num_start_az";
            this.num_start_az.Size = new System.Drawing.Size(44, 20);
            this.num_start_az.TabIndex = 0;
            this.toolTip1.SetToolTip(this.num_start_az, "Start angle of a clockwise arc");
            this.num_start_az.ValueChanged += new System.EventHandler(this.num_az_startstop_ValueChanged);
            // 
            // lbl_meters_do_not_convert
            // 
            this.lbl_meters_do_not_convert.AutoSize = true;
            this.lbl_meters_do_not_convert.Location = new System.Drawing.Point(316, 20);
            this.lbl_meters_do_not_convert.Name = "lbl_meters_do_not_convert";
            this.lbl_meters_do_not_convert.Size = new System.Drawing.Size(15, 13);
            this.lbl_meters_do_not_convert.TabIndex = 8;
            this.lbl_meters_do_not_convert.Text = "m";
            this.toolTip1.SetToolTip(this.lbl_meters_do_not_convert, "Linear distance step size for calculating terrain\r\nintersections. Increase this t" +
        "o speed up calculation\r\nat the cost of accuracy\r\n\r\nDefault 10m\r\n");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Distance Step";
            this.toolTip1.SetToolTip(this.label3, "Linear distance step size for calculating terrain\r\nintersections. Increase this t" +
        "o speed up calculation\r\nat the cost of accuracy\r\n\r\nDefault 10m\r\n");
            // 
            // num_dist_step
            // 
            this.num_dist_step.Location = new System.Drawing.Point(266, 18);
            this.num_dist_step.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_dist_step.Name = "num_dist_step";
            this.num_dist_step.Size = new System.Drawing.Size(44, 20);
            this.num_dist_step.TabIndex = 4;
            this.toolTip1.SetToolTip(this.num_dist_step, "Linear distance step size for calculating terrain\r\nintersections. Increase this t" +
        "o speed up calculation\r\nat the cost of accuracy\r\n\r\nDefault 10m\r\n");
            this.num_dist_step.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbl_altunit3);
            this.groupBox3.Controls.Add(this.num_clear_terrain);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.num_clear_angle);
            this.groupBox3.Location = new System.Drawing.Point(372, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(138, 101);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Clearance";
            this.toolTip1.SetToolTip(this.groupBox3, "Controls how clear of terrain the line of sight\r\nmust be and the minimum separati" +
        "on for\r\nthe aircraft to pass safely over obstacles.");
            // 
            // lbl_altunit3
            // 
            this.lbl_altunit3.AutoSize = true;
            this.lbl_altunit3.Location = new System.Drawing.Point(102, 51);
            this.lbl_altunit3.Name = "lbl_altunit3";
            this.lbl_altunit3.Size = new System.Drawing.Size(15, 13);
            this.lbl_altunit3.TabIndex = 10;
            this.lbl_altunit3.Text = "m";
            this.toolTip1.SetToolTip(this.lbl_altunit3, "The aircraft should pass at least this high over\r\nobstacles for safety.");
            // 
            // num_clear_terrain
            // 
            this.num_clear_terrain.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_clear_terrain.Location = new System.Drawing.Point(52, 49);
            this.num_clear_terrain.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.num_clear_terrain.Name = "num_clear_terrain";
            this.num_clear_terrain.Size = new System.Drawing.Size(44, 20);
            this.num_clear_terrain.TabIndex = 9;
            this.toolTip1.SetToolTip(this.num_clear_terrain, "The aircraft should pass at least this high over\r\nobstacles for safety.");
            this.num_clear_terrain.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Terrain";
            this.toolTip1.SetToolTip(this.label6, "The aircraft should pass at least this high over\r\nobstacles for safety.");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(102, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(25, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "deg";
            this.toolTip1.SetToolTip(this.label11, "The straight line between the base and the aircraft\r\nmust clear all obstacles by " +
        "a margin of this many\r\ndegrees. This is to account for signal degradation\r\ndue t" +
        "o reflections.\r\n\r\nDefault 1.5 degrees\r\n");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 25);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Angle";
            this.toolTip1.SetToolTip(this.label14, "The straight line between the base and the aircraft\r\nmust clear all obstacles by " +
        "a margin of this many\r\ndegrees. This is to account for signal degradation\r\ndue t" +
        "o reflections.\r\n\r\nDefault 1.5 degrees\r\n");
            // 
            // num_clear_angle
            // 
            this.num_clear_angle.DecimalPlaces = 1;
            this.num_clear_angle.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.num_clear_angle.Location = new System.Drawing.Point(52, 23);
            this.num_clear_angle.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.num_clear_angle.Name = "num_clear_angle";
            this.num_clear_angle.Size = new System.Drawing.Size(44, 20);
            this.num_clear_angle.TabIndex = 0;
            this.toolTip1.SetToolTip(this.num_clear_angle, "The straight line between the base and the aircraft\r\nmust clear all obstacles by " +
        "a margin of this many\r\ndegrees. This is to account for signal degradation\r\ndue t" +
        "o reflections.\r\n\r\nDefault 1.5 deg\r\n");
            this.num_clear_angle.Value = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbl_altunit2);
            this.groupBox2.Controls.Add(this.num_mast_height);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.rad_ahl);
            this.groupBox2.Controls.Add(this.rad_msl);
            this.groupBox2.Controls.Add(this.lbl_altunit1);
            this.groupBox2.Controls.Add(this.lbl_distbigunit);
            this.groupBox2.Controls.Add(this.num_altitude);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.num_range);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(223, 101);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Flight";
            this.toolTip1.SetToolTip(this.groupBox2, "Parameters related to this planned flight");
            // 
            // lbl_altunit2
            // 
            this.lbl_altunit2.AutoSize = true;
            this.lbl_altunit2.Location = new System.Drawing.Point(146, 77);
            this.lbl_altunit2.Name = "lbl_altunit2";
            this.lbl_altunit2.Size = new System.Drawing.Size(15, 13);
            this.lbl_altunit2.TabIndex = 12;
            this.lbl_altunit2.Text = "m";
            this.toolTip1.SetToolTip(this.lbl_altunit2, "Height above ground that the base radio is mounted.\r\nSet this to the height of yo" +
        "ur mast if using one.");
            // 
            // num_mast_height
            // 
            this.num_mast_height.Location = new System.Drawing.Point(83, 75);
            this.num_mast_height.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.num_mast_height.Name = "num_mast_height";
            this.num_mast_height.Size = new System.Drawing.Size(57, 20);
            this.num_mast_height.TabIndex = 11;
            this.toolTip1.SetToolTip(this.num_mast_height, "Height above ground that the base radio is mounted.\r\nSet this to the height of yo" +
        "ur mast if using one.");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Mast Height";
            this.toolTip1.SetToolTip(this.label9, "Height above ground that the base radio is mounted.\r\nSet this to the height of yo" +
        "ur mast if using one.");
            // 
            // rad_ahl
            // 
            this.rad_ahl.AutoSize = true;
            this.rad_ahl.Location = new System.Drawing.Point(170, 60);
            this.rad_ahl.Name = "rad_ahl";
            this.rad_ahl.Size = new System.Drawing.Size(46, 17);
            this.rad_ahl.TabIndex = 9;
            this.rad_ahl.TabStop = true;
            this.rad_ahl.Text = "AHL";
            this.toolTip1.SetToolTip(this.rad_ahl, "Flight flight_altitude is height above home");
            this.rad_ahl.UseVisualStyleBackColor = true;
            // 
            // rad_msl
            // 
            this.rad_msl.AutoSize = true;
            this.rad_msl.Location = new System.Drawing.Point(170, 40);
            this.rad_msl.Name = "rad_msl";
            this.rad_msl.Size = new System.Drawing.Size(47, 17);
            this.rad_msl.TabIndex = 8;
            this.rad_msl.TabStop = true;
            this.rad_msl.Text = "MSL";
            this.toolTip1.SetToolTip(this.rad_msl, "Flight flight_altitude is above Mean Sea Level");
            this.rad_msl.UseVisualStyleBackColor = true;
            // 
            // lbl_altunit1
            // 
            this.lbl_altunit1.AutoSize = true;
            this.lbl_altunit1.Location = new System.Drawing.Point(146, 51);
            this.lbl_altunit1.Name = "lbl_altunit1";
            this.lbl_altunit1.Size = new System.Drawing.Size(15, 13);
            this.lbl_altunit1.TabIndex = 7;
            this.lbl_altunit1.Text = "m";
            this.toolTip1.SetToolTip(this.lbl_altunit1, "Altitude of the intended flight, either measured\r\nabove home (AHL) or above mean " +
        "sea level (MSL)");
            // 
            // lbl_distbigunit
            // 
            this.lbl_distbigunit.AutoSize = true;
            this.lbl_distbigunit.Location = new System.Drawing.Point(146, 25);
            this.lbl_distbigunit.Name = "lbl_distbigunit";
            this.lbl_distbigunit.Size = new System.Drawing.Size(21, 13);
            this.lbl_distbigunit.TabIndex = 6;
            this.lbl_distbigunit.Text = "km";
            this.toolTip1.SetToolTip(this.lbl_distbigunit, "The best-case range of the radio, or the furthest\r\nintended flight distance. Larg" +
        "e distances can slow\r\ndown the calculation.");
            // 
            // num_altitude
            // 
            this.num_altitude.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_altitude.Location = new System.Drawing.Point(83, 49);
            this.num_altitude.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.num_altitude.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.num_altitude.Name = "num_altitude";
            this.num_altitude.Size = new System.Drawing.Size(57, 20);
            this.num_altitude.TabIndex = 3;
            this.toolTip1.SetToolTip(this.num_altitude, "Altitude of the intended flight, either measured\r\nabove home (AHL) or above mean " +
        "sea level (MSL)");
            this.num_altitude.Value = new decimal(new int[] {
            12000,
            0,
            0,
            0});
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 51);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 2;
            this.label17.Text = "Flight Altitude";
            this.toolTip1.SetToolTip(this.label17, "Altitude of the intended flight, either measured\r\nabove home (AHL) or above mean " +
        "sea level (MSL)");
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 25);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(39, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Range";
            this.toolTip1.SetToolTip(this.label18, "The best-case range of the radio, or the furthest\r\nintended flight distance. Larg" +
        "e distances can slow\r\ndown the calculation.");
            // 
            // num_range
            // 
            this.num_range.DecimalPlaces = 1;
            this.num_range.Location = new System.Drawing.Point(83, 23);
            this.num_range.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.num_range.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_range.Name = "num_range";
            this.num_range.Size = new System.Drawing.Size(57, 20);
            this.num_range.TabIndex = 0;
            this.toolTip1.SetToolTip(this.num_range, "The best-case range of the radio, or the furthest\r\nintended flight distance. Larg" +
        "e distances can slow\r\ndown the calculation.");
            this.num_range.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.num_tolerance);
            this.groupBox4.Controls.Add(this.lbl_meters_do_not_convert);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.num_az_step);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.num_dist_step);
            this.groupBox4.Location = new System.Drawing.Point(12, 119);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(498, 45);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Advanced";
            this.toolTip1.SetToolTip(this.groupBox4, "Settings that affect accuracy/runtime tradeoff. Leave\r\nthese default.");
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(471, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(25, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "deg";
            this.toolTip1.SetToolTip(this.label12, "Convergence tolerance for determining minimum\r\nelevation angle. Increase to speed" +
        " up calculation at\r\nthe cost of accuracty.\r\n\r\nDefault 0.05 deg");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(360, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Tolerance";
            this.toolTip1.SetToolTip(this.label13, "Convergence tolerance for determining minimum\r\nelevation angle. Increase to speed" +
        " up calculation at\r\nthe cost of accuracty.\r\n\r\nDefault 0.05 deg");
            // 
            // num_tolerance
            // 
            this.num_tolerance.DecimalPlaces = 2;
            this.num_tolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.num_tolerance.Location = new System.Drawing.Point(421, 18);
            this.num_tolerance.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_tolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.num_tolerance.Name = "num_tolerance";
            this.num_tolerance.Size = new System.Drawing.Size(44, 20);
            this.num_tolerance.TabIndex = 12;
            this.toolTip1.SetToolTip(this.num_tolerance, "Convergence tolerance for determining minimum\r\nelevation angle. Increase to speed" +
        " up calculation at\r\nthe cost of accuracty.\r\n\r\nDefault 0.05 deg");
            this.num_tolerance.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(131, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "deg";
            this.toolTip1.SetToolTip(this.label5, "Step size from azimuth start to stop. Increase to\r\nspeed up calculation, at cost " +
        "of accuracy.\r\n\r\nDefault 0.5 deg");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "Azimuth Step";
            this.toolTip1.SetToolTip(this.label10, "Step size from azimuth start to stop. Increase to\r\nspeed up calculation, at cost " +
        "of accuracy.\r\n\r\nDefault 0.5 deg");
            // 
            // num_az_step
            // 
            this.num_az_step.DecimalPlaces = 1;
            this.num_az_step.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.num_az_step.Location = new System.Drawing.Point(81, 18);
            this.num_az_step.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_az_step.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.num_az_step.Name = "num_az_step";
            this.num_az_step.Size = new System.Drawing.Size(44, 20);
            this.num_az_step.TabIndex = 9;
            this.toolTip1.SetToolTip(this.num_az_step, "Step size from azimuth start to stop. Increase to\r\nspeed up calculation, at cost " +
        "of accuracy.\r\n\r\nDefault 0.5 deg");
            this.num_az_step.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // but_generate
            // 
            this.but_generate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_generate.Location = new System.Drawing.Point(435, 173);
            this.but_generate.Name = "but_generate";
            this.but_generate.Size = new System.Drawing.Size(75, 23);
            this.but_generate.TabIndex = 12;
            this.but_generate.Text = "Generate";
            this.but_generate.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_generate.UseVisualStyleBackColor = true;
            this.but_generate.Click += new System.EventHandler(this.but_generate_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.Color.Transparent;
            this.progressBar.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(167)))), ((int)(((byte)(42)))));
            this.progressBar.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(139)))), ((int)(((byte)(26)))));
            this.progressBar.Location = new System.Drawing.Point(12, 173);
            this.progressBar.Name = "progressBar";
            this.progressBar.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(171)))), ((int)(((byte)(112)))));
            this.progressBar.Size = new System.Drawing.Size(417, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 13;
            this.progressBar.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(54)))), ((int)(((byte)(8)))));
            // 
            // RadioLOS_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 208);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.but_generate);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RadioLOS_UI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Radio Line-of-Sight Calculator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_stop_az)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_start_az)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_dist_step)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_clear_terrain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_clear_angle)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_mast_height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_altitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_range)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_tolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_az_step)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown num_dist_step;
        private System.Windows.Forms.NumericUpDown num_stop_az;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown num_start_az;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbl_meters_do_not_convert;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown num_clear_angle;
        private System.Windows.Forms.Label lbl_altunit3;
        private System.Windows.Forms.NumericUpDown num_clear_terrain;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbl_altunit2;
        private System.Windows.Forms.NumericUpDown num_mast_height;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rad_ahl;
        private System.Windows.Forms.RadioButton rad_msl;
        private System.Windows.Forms.Label lbl_altunit1;
        private System.Windows.Forms.Label lbl_distbigunit;
        private System.Windows.Forms.NumericUpDown num_altitude;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.NumericUpDown num_range;
        private MissionPlanner.Controls.MyButton but_generate;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown num_tolerance;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown num_az_step;
        private MissionPlanner.Controls.MyProgressBar progressBar;
    }
}