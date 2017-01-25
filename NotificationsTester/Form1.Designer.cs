namespace NotificationsTester
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIOSToken = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAndroidToken = new System.Windows.Forms.TextBox();
            this.buttonSendIOSMessage = new System.Windows.Forms.Button();
            this.buttonSendAndroidMessage = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "iOS device token:";
            // 
            // textBoxIOSToken
            // 
            this.textBoxIOSToken.Location = new System.Drawing.Point(163, 61);
            this.textBoxIOSToken.Name = "textBoxIOSToken";
            this.textBoxIOSToken.Size = new System.Drawing.Size(417, 20);
            this.textBoxIOSToken.TabIndex = 1;
            this.textBoxIOSToken.Text = "9388adc2 1f093caa 12dd1469 a671d28e 88d2e87f 3088ee80 1b28b258 55bc175d";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Android device token:";
            // 
            // textBoxAndroidToken
            // 
            this.textBoxAndroidToken.Location = new System.Drawing.Point(163, 92);
            this.textBoxAndroidToken.Name = "textBoxAndroidToken";
            this.textBoxAndroidToken.Size = new System.Drawing.Size(417, 20);
            this.textBoxAndroidToken.TabIndex = 3;
            this.textBoxAndroidToken.Text = "ff5oAFrKE88:APA91bHRE1lPXOtVhrmsHZEV6X0WwM1pZAi7kST7cF4xux-A3oeTfFdr09z_7r2d7kTZ5" +
    "abPbfZFdvgeX2E9Cpy14e0OjfWFo1BcODgYLgXuVJpoiLyJ85rIsUSkf9IX1oyLTaFKFxaC";
            // 
            // buttonSendIOSMessage
            // 
            this.buttonSendIOSMessage.Location = new System.Drawing.Point(595, 60);
            this.buttonSendIOSMessage.Name = "buttonSendIOSMessage";
            this.buttonSendIOSMessage.Size = new System.Drawing.Size(141, 23);
            this.buttonSendIOSMessage.TabIndex = 4;
            this.buttonSendIOSMessage.Text = "Send IOS message";
            this.buttonSendIOSMessage.UseVisualStyleBackColor = true;
            this.buttonSendIOSMessage.Click += new System.EventHandler(this.buttonSendIOSMessage_Click);
            // 
            // buttonSendAndroidMessage
            // 
            this.buttonSendAndroidMessage.Location = new System.Drawing.Point(595, 90);
            this.buttonSendAndroidMessage.Name = "buttonSendAndroidMessage";
            this.buttonSendAndroidMessage.Size = new System.Drawing.Size(141, 23);
            this.buttonSendAndroidMessage.TabIndex = 5;
            this.buttonSendAndroidMessage.Text = "Send Android message";
            this.buttonSendAndroidMessage.UseVisualStyleBackColor = true;
            this.buttonSendAndroidMessage.Click += new System.EventHandler(this.buttonSendAndroidMessage_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Message to send:";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(163, 26);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(417, 20);
            this.textBoxMessage.TabIndex = 7;
            this.textBoxMessage.Text = "test from test";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 256);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonSendAndroidMessage);
            this.Controls.Add(this.buttonSendIOSMessage);
            this.Controls.Add(this.textBoxAndroidToken);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIOSToken);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIOSToken;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAndroidToken;
        private System.Windows.Forms.Button buttonSendIOSMessage;
        private System.Windows.Forms.Button buttonSendAndroidMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxMessage;
    }
}

