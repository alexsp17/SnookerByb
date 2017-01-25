namespace DatabaseSeeder
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.buttonVenues = new System.Windows.Forms.Button();
            this.buttonMetros = new System.Windows.Forms.Button();
            this.buttonApplyMetrosToVenues = new System.Windows.Forms.Button();
            this.buttonSeedVerifications = new System.Windows.Forms.Button();
            this.buttonTestNotifications = new System.Windows.Forms.Button();
            this.buttonTemp1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(533, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(156, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Seed sports and result types";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(533, 61);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(156, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Seed quotes";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // buttonVenues
            // 
            this.buttonVenues.Location = new System.Drawing.Point(28, 61);
            this.buttonVenues.Name = "buttonVenues";
            this.buttonVenues.Size = new System.Drawing.Size(156, 23);
            this.buttonVenues.TabIndex = 3;
            this.buttonVenues.Text = "Seed venues";
            this.buttonVenues.UseVisualStyleBackColor = true;
            this.buttonVenues.Click += new System.EventHandler(this.buttonVenues_Click);
            // 
            // buttonMetros
            // 
            this.buttonMetros.Location = new System.Drawing.Point(28, 22);
            this.buttonMetros.Name = "buttonMetros";
            this.buttonMetros.Size = new System.Drawing.Size(156, 23);
            this.buttonMetros.TabIndex = 4;
            this.buttonMetros.Text = "Seed metros";
            this.buttonMetros.UseVisualStyleBackColor = true;
            this.buttonMetros.Click += new System.EventHandler(this.buttonMetros_Click);
            // 
            // buttonApplyMetrosToVenues
            // 
            this.buttonApplyMetrosToVenues.Location = new System.Drawing.Point(28, 102);
            this.buttonApplyMetrosToVenues.Name = "buttonApplyMetrosToVenues";
            this.buttonApplyMetrosToVenues.Size = new System.Drawing.Size(156, 23);
            this.buttonApplyMetrosToVenues.TabIndex = 5;
            this.buttonApplyMetrosToVenues.Text = "Apply metros to venues";
            this.buttonApplyMetrosToVenues.UseVisualStyleBackColor = true;
            this.buttonApplyMetrosToVenues.Click += new System.EventHandler(this.buttonApplyMetrosToVenues_Click);
            // 
            // buttonSeedVerifications
            // 
            this.buttonSeedVerifications.Location = new System.Drawing.Point(533, 102);
            this.buttonSeedVerifications.Name = "buttonSeedVerifications";
            this.buttonSeedVerifications.Size = new System.Drawing.Size(156, 23);
            this.buttonSeedVerifications.TabIndex = 6;
            this.buttonSeedVerifications.Text = "Seed verifications";
            this.buttonSeedVerifications.UseVisualStyleBackColor = true;
            this.buttonSeedVerifications.Click += new System.EventHandler(this.buttonSeedVerifications_Click);
            // 
            // buttonTestNotifications
            // 
            this.buttonTestNotifications.Location = new System.Drawing.Point(533, 238);
            this.buttonTestNotifications.Name = "buttonTestNotifications";
            this.buttonTestNotifications.Size = new System.Drawing.Size(156, 23);
            this.buttonTestNotifications.TabIndex = 7;
            this.buttonTestNotifications.Text = "Test notifications";
            this.buttonTestNotifications.UseVisualStyleBackColor = true;
            this.buttonTestNotifications.Click += new System.EventHandler(this.buttonTestNotifications_Click);
            // 
            // buttonTemp1
            // 
            this.buttonTemp1.Location = new System.Drawing.Point(357, 238);
            this.buttonTemp1.Name = "buttonTemp1";
            this.buttonTemp1.Size = new System.Drawing.Size(156, 23);
            this.buttonTemp1.TabIndex = 8;
            this.buttonTemp1.Text = "Temp 1";
            this.buttonTemp1.UseVisualStyleBackColor = true;
            this.buttonTemp1.Click += new System.EventHandler(this.buttonTemp1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 283);
            this.Controls.Add(this.buttonTemp1);
            this.Controls.Add(this.buttonTestNotifications);
            this.Controls.Add(this.buttonSeedVerifications);
            this.Controls.Add(this.buttonApplyMetrosToVenues);
            this.Controls.Add(this.buttonMetros);
            this.Controls.Add(this.buttonVenues);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button buttonVenues;
        private System.Windows.Forms.Button buttonMetros;
        private System.Windows.Forms.Button buttonApplyMetrosToVenues;
        private System.Windows.Forms.Button buttonSeedVerifications;
        private System.Windows.Forms.Button buttonTestNotifications;
        private System.Windows.Forms.Button buttonTemp1;
    }
}

