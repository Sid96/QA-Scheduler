namespace CalendarAppV4
{
    partial class ViewCalendar
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
            this.Undo = new System.Windows.Forms.Button();
            this.Reset = new System.Windows.Forms.Button();
            this.CreateNewTargVers = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Back = new System.Windows.Forms.Button();
            this.calendar1 = new Calendar.NET.Calendar();
            this.SuspendLayout();
            // 
            // Undo
            // 
            this.Undo.Location = new System.Drawing.Point(854, 934);
            this.Undo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Undo.Name = "Undo";
            this.Undo.Size = new System.Drawing.Size(284, 35);
            this.Undo.TabIndex = 14;
            this.Undo.Text = "Undo";
            this.Undo.UseVisualStyleBackColor = true;
            this.Undo.Click += new System.EventHandler(this.Undo_Click);
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(430, 934);
            this.Reset.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(282, 35);
            this.Reset.TabIndex = 13;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // CreateNewTargVers
            // 
            this.CreateNewTargVers.Location = new System.Drawing.Point(1278, 934);
            this.CreateNewTargVers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CreateNewTargVers.Name = "CreateNewTargVers";
            this.CreateNewTargVers.Size = new System.Drawing.Size(288, 35);
            this.CreateNewTargVers.TabIndex = 12;
            this.CreateNewTargVers.Text = "Create New Target Version";
            this.CreateNewTargVers.UseVisualStyleBackColor = true;
            this.CreateNewTargVers.Click += new System.EventHandler(this.CreateNewTargVers_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(1685, 934);
            this.Save.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(112, 35);
            this.Save.TabIndex = 11;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Back
            // 
            this.Back.Location = new System.Drawing.Point(63, 934);
            this.Back.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Back.Name = "Back";
            this.Back.Size = new System.Drawing.Size(112, 35);
            this.Back.TabIndex = 10;
            this.Back.Text = "Back";
            this.Back.UseVisualStyleBackColor = true;
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // calendar1
            // 
            this.calendar1.AllowEditingEvents = true;
            this.calendar1.CalendarDate = new System.DateTime(2015, 1, 19, 16, 13, 40, 397);
            this.calendar1.CalendarView = Calendar.NET.CalendarViews.Month;
            this.calendar1.DateHeaderFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.calendar1.DayOfWeekFont = new System.Drawing.Font("Arial", 10F);
            this.calendar1.DaysFont = new System.Drawing.Font("Arial", 10F);
            this.calendar1.DayViewTimeFont = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.calendar1.DimDisabledEvents = true;
            this.calendar1.HighlightCurrentDay = true;
            this.calendar1.LoadPresetHolidays = true;
            this.calendar1.Location = new System.Drawing.Point(13, 14);
            this.calendar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.calendar1.Name = "calendar1";
            this.calendar1.ShowArrowControls = true;
            this.calendar1.ShowDashedBorderOnDisabledEvents = true;
            this.calendar1.ShowDateInHeader = true;
            this.calendar1.ShowDisabledEvents = true;
            this.calendar1.ShowEventTooltips = false;
            this.calendar1.ShowTodayButton = true;
            this.calendar1.Size = new System.Drawing.Size(1896, 933);
            this.calendar1.TabIndex = 9;
            this.calendar1.TodayFont = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            // 
            // ViewCalendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1912, 983);
            this.Controls.Add(this.Undo);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.CreateNewTargVers);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Back);
            this.Controls.Add(this.calendar1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ViewCalendar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Workflow Calendar";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ViewCalendar_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Undo;
        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.Button CreateNewTargVers;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Back;
        private Calendar.NET.Calendar calendar1;
    }
}