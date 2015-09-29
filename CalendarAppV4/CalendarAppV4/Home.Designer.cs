namespace CalendarAppV4
{
    partial class Home
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
            this.Replace = new System.Windows.Forms.Button();
            this.Copy = new System.Windows.Forms.Button();
            this.userInput = new System.Windows.Forms.ComboBox();
            this.viewCalendar = new System.Windows.Forms.Button();
            this.OpenTxt = new System.Windows.Forms.Button();
            this.newTargetVersion = new System.Windows.Forms.Button();
            this.Done = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Append = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // Replace
            // 
            this.Replace.Location = new System.Drawing.Point(83, 213);
            this.Replace.Name = "Replace";
            this.Replace.Size = new System.Drawing.Size(267, 23);
            this.Replace.TabIndex = 8;
            this.Replace.Text = "Replace With New Database";
            this.Replace.UseVisualStyleBackColor = true;
            this.Replace.Click += new System.EventHandler(this.Replace_Click);
            // 
            // Copy
            // 
            this.Copy.Location = new System.Drawing.Point(83, 149);
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(267, 23);
            this.Copy.TabIndex = 6;
            this.Copy.Text = "Copy Database to Desktop";
            this.Copy.UseVisualStyleBackColor = true;
            this.Copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // userInput
            // 
            this.userInput.FormattingEnabled = true;
            this.userInput.Location = new System.Drawing.Point(83, 15);
            this.userInput.Name = "userInput";
            this.userInput.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.userInput.Size = new System.Drawing.Size(267, 21);
            this.userInput.TabIndex = 1;
            this.userInput.Click += new System.EventHandler(this.userInput_Click);
            // 
            // viewCalendar
            // 
            this.viewCalendar.Location = new System.Drawing.Point(83, 116);
            this.viewCalendar.Name = "viewCalendar";
            this.viewCalendar.Size = new System.Drawing.Size(267, 23);
            this.viewCalendar.TabIndex = 5;
            this.viewCalendar.Text = "View Calendar";
            this.viewCalendar.UseVisualStyleBackColor = true;
            this.viewCalendar.Click += new System.EventHandler(this.viewCalendar_Click);
            // 
            // OpenTxt
            // 
            this.OpenTxt.Location = new System.Drawing.Point(83, 86);
            this.OpenTxt.Name = "OpenTxt";
            this.OpenTxt.Size = new System.Drawing.Size(267, 23);
            this.OpenTxt.TabIndex = 4;
            this.OpenTxt.Text = "Show All Target Versions";
            this.OpenTxt.UseVisualStyleBackColor = true;
            this.OpenTxt.Click += new System.EventHandler(this.OpenTxt_Click);
            // 
            // newTargetVersion
            // 
            this.newTargetVersion.Location = new System.Drawing.Point(83, 54);
            this.newTargetVersion.Name = "newTargetVersion";
            this.newTargetVersion.Size = new System.Drawing.Size(267, 23);
            this.newTargetVersion.TabIndex = 3;
            this.newTargetVersion.Text = "Create New Target Version";
            this.newTargetVersion.UseVisualStyleBackColor = true;
            this.newTargetVersion.Click += new System.EventHandler(this.newTargetVersion_Click);
            // 
            // Done
            // 
            this.Done.Location = new System.Drawing.Point(356, 15);
            this.Done.Name = "Done";
            this.Done.Size = new System.Drawing.Size(51, 21);
            this.Done.TabIndex = 2;
            this.Done.Text = "Search";
            this.Done.UseVisualStyleBackColor = true;
            this.Done.Click += new System.EventHandler(this.Done_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Version:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Show Target";
            // 
            // Append
            // 
            this.Append.Location = new System.Drawing.Point(83, 180);
            this.Append.Name = "Append";
            this.Append.Size = new System.Drawing.Size(267, 23);
            this.Append.TabIndex = 7;
            this.Append.Text = "Add to Old Database";
            this.Append.UseVisualStyleBackColor = true;
            this.Append.Click += new System.EventHandler(this.Append_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Home
            // 
            this.AcceptButton = this.Done;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 247);
            this.Controls.Add(this.Replace);
            this.Controls.Add(this.Copy);
            this.Controls.Add(this.userInput);
            this.Controls.Add(this.viewCalendar);
            this.Controls.Add(this.OpenTxt);
            this.Controls.Add(this.newTargetVersion);
            this.Controls.Add(this.Done);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Append);
            this.Name = "Home";
            this.Text = "Home";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Home_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Replace;
        private System.Windows.Forms.Button Copy;
        private System.Windows.Forms.ComboBox userInput;
        private System.Windows.Forms.Button viewCalendar;
        private System.Windows.Forms.Button OpenTxt;
        internal System.Windows.Forms.Button newTargetVersion;
        private System.Windows.Forms.Button Done;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Append;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}