namespace musicLED
{
    partial class musicLED
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartFft = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.displayMode = new System.Windows.Forms.ComboBox();
            this.ledMode = new System.Windows.Forms.ComboBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.serialSelect = new System.Windows.Forms.ComboBox();
            this.smoothingCheckBox = new System.Windows.Forms.CheckBox();
            this.displayCheckBox = new System.Windows.Forms.CheckBox();
            this.redSlider = new System.Windows.Forms.TrackBar();
            this.greenSlider = new System.Windows.Forms.TrackBar();
            this.blueSlider = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.brightnessSlider = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartFft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // chartFft
            // 
            chartArea4.Name = "ChartArea1";
            this.chartFft.ChartAreas.Add(chartArea4);
            this.chartFft.Location = new System.Drawing.Point(12, 209);
            this.chartFft.Name = "chartFft";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series4.Name = "Series1";
            this.chartFft.Series.Add(series4);
            this.chartFft.Size = new System.Drawing.Size(600, 300);
            this.chartFft.TabIndex = 0;
            this.chartFft.Text = "FFT Chart";
            this.chartFft.Visible = false;
            // 
            // displayMode
            // 
            this.displayMode.FormattingEnabled = true;
            this.displayMode.Location = new System.Drawing.Point(12, 182);
            this.displayMode.Name = "displayMode";
            this.displayMode.Size = new System.Drawing.Size(121, 21);
            this.displayMode.TabIndex = 1;
            // 
            // ledMode
            // 
            this.ledMode.FormattingEnabled = true;
            this.ledMode.Location = new System.Drawing.Point(301, 12);
            this.ledMode.Name = "ledMode";
            this.ledMode.Size = new System.Drawing.Size(175, 21);
            this.ledMode.TabIndex = 2;
            this.ledMode.SelectedValueChanged += new System.EventHandler(this.onLEDModeChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(139, 10);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.onRefresh);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(220, 10);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 4;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.onConnect);
            // 
            // serialSelect
            // 
            this.serialSelect.FormattingEnabled = true;
            this.serialSelect.Location = new System.Drawing.Point(12, 12);
            this.serialSelect.Name = "serialSelect";
            this.serialSelect.Size = new System.Drawing.Size(121, 21);
            this.serialSelect.TabIndex = 5;
            // 
            // smoothingCheckBox
            // 
            this.smoothingCheckBox.AutoSize = true;
            this.smoothingCheckBox.Checked = true;
            this.smoothingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.smoothingCheckBox.Location = new System.Drawing.Point(482, 14);
            this.smoothingCheckBox.Name = "smoothingCheckBox";
            this.smoothingCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.smoothingCheckBox.Size = new System.Drawing.Size(76, 17);
            this.smoothingCheckBox.TabIndex = 8;
            this.smoothingCheckBox.Text = "Smoothing";
            this.smoothingCheckBox.UseMnemonic = false;
            this.smoothingCheckBox.UseVisualStyleBackColor = true;
            // 
            // displayCheckBox
            // 
            this.displayCheckBox.AutoSize = true;
            this.displayCheckBox.Location = new System.Drawing.Point(139, 186);
            this.displayCheckBox.Name = "displayCheckBox";
            this.displayCheckBox.Size = new System.Drawing.Size(60, 17);
            this.displayCheckBox.TabIndex = 9;
            this.displayCheckBox.Text = "Display";
            this.displayCheckBox.UseVisualStyleBackColor = true;
            this.displayCheckBox.CheckedChanged += new System.EventHandler(this.onDisplayChanged);
            // 
            // redSlider
            // 
            this.redSlider.Location = new System.Drawing.Point(12, 40);
            this.redSlider.Maximum = 100;
            this.redSlider.Name = "redSlider";
            this.redSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.redSlider.Size = new System.Drawing.Size(45, 120);
            this.redSlider.TabIndex = 10;
            this.redSlider.TickFrequency = 10;
            this.redSlider.Value = 100;
            this.redSlider.ValueChanged += new System.EventHandler(this.OnRedSliderChanged);
            // 
            // greenSlider
            // 
            this.greenSlider.Location = new System.Drawing.Point(63, 40);
            this.greenSlider.Maximum = 100;
            this.greenSlider.Name = "greenSlider";
            this.greenSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.greenSlider.Size = new System.Drawing.Size(45, 120);
            this.greenSlider.TabIndex = 11;
            this.greenSlider.TickFrequency = 10;
            this.greenSlider.Value = 100;
            this.greenSlider.ValueChanged += new System.EventHandler(this.OnGreenSliderChanged);
            // 
            // blueSlider
            // 
            this.blueSlider.Location = new System.Drawing.Point(114, 40);
            this.blueSlider.Maximum = 100;
            this.blueSlider.Name = "blueSlider";
            this.blueSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.blueSlider.Size = new System.Drawing.Size(45, 120);
            this.blueSlider.TabIndex = 12;
            this.blueSlider.TickFrequency = 10;
            this.blueSlider.Value = 100;
            this.blueSlider.ValueChanged += new System.EventHandler(this.OnBlueSliderChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(114, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Blue";
            // 
            // brightnessSlider
            // 
            this.brightnessSlider.Location = new System.Drawing.Point(165, 40);
            this.brightnessSlider.Maximum = 100;
            this.brightnessSlider.Name = "brightnessSlider";
            this.brightnessSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.brightnessSlider.Size = new System.Drawing.Size(45, 120);
            this.brightnessSlider.TabIndex = 16;
            this.brightnessSlider.TickFrequency = 10;
            this.brightnessSlider.Value = 50;
            this.brightnessSlider.ValueChanged += new System.EventHandler(this.OnBrightnessSliderChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(165, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Brightness";
            // 
            // musicLED
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 221);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.brightnessSlider);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.blueSlider);
            this.Controls.Add(this.greenSlider);
            this.Controls.Add(this.redSlider);
            this.Controls.Add(this.displayCheckBox);
            this.Controls.Add(this.smoothingCheckBox);
            this.Controls.Add(this.serialSelect);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.ledMode);
            this.Controls.Add(this.displayMode);
            this.Controls.Add(this.chartFft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "musicLED";
            this.Text = "musicLED";
            ((System.ComponentModel.ISupportInitialize)(this.chartFft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartFft;
        private System.Windows.Forms.ComboBox displayMode;
        private System.Windows.Forms.ComboBox ledMode;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ComboBox serialSelect;
        private System.Windows.Forms.CheckBox smoothingCheckBox;
        private System.Windows.Forms.CheckBox displayCheckBox;
        private System.Windows.Forms.TrackBar redSlider;
        private System.Windows.Forms.TrackBar greenSlider;
        private System.Windows.Forms.TrackBar blueSlider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar brightnessSlider;
        private System.Windows.Forms.Label label4;
    }
}

