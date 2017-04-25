namespace RouteChecker
{
    partial class Options
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Roles_TB = new System.Windows.Forms.TextBox();
            this.Cancel_BTN = new System.Windows.Forms.Button();
            this.Save_BTN = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Mfa_TB = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.EC2Items_TB = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Tags_TB = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Roles_TB);
            this.groupBox1.Location = new System.Drawing.Point(11, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(907, 194);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Roles";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(308, 34);
            this.label1.TabIndex = 1;
            this.label1.Text = "List of roles that can be assumed.\r\nFormat: FriendlyName|arn of the role to assum" +
    "e";
            // 
            // Roles_TB
            // 
            this.Roles_TB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Roles_TB.Location = new System.Drawing.Point(12, 57);
            this.Roles_TB.Multiline = true;
            this.Roles_TB.Name = "Roles_TB";
            this.Roles_TB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Roles_TB.Size = new System.Drawing.Size(880, 118);
            this.Roles_TB.TabIndex = 0;
            this.Roles_TB.WordWrap = false;
            // 
            // Cancel_BTN
            // 
            this.Cancel_BTN.Location = new System.Drawing.Point(14, 805);
            this.Cancel_BTN.Name = "Cancel_BTN";
            this.Cancel_BTN.Size = new System.Drawing.Size(75, 25);
            this.Cancel_BTN.TabIndex = 6;
            this.Cancel_BTN.Text = "Cancel";
            this.Cancel_BTN.UseVisualStyleBackColor = true;
            this.Cancel_BTN.Click += new System.EventHandler(this.Cancel_BTN_Click);
            // 
            // Save_BTN
            // 
            this.Save_BTN.Location = new System.Drawing.Point(95, 805);
            this.Save_BTN.Name = "Save_BTN";
            this.Save_BTN.Size = new System.Drawing.Size(75, 25);
            this.Save_BTN.TabIndex = 5;
            this.Save_BTN.Text = "Save";
            this.Save_BTN.UseVisualStyleBackColor = true;
            this.Save_BTN.Click += new System.EventHandler(this.Save_BTN_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.Mfa_TB);
            this.groupBox2.Location = new System.Drawing.Point(14, 227);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(907, 232);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MFA Devices";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(542, 85);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // Mfa_TB
            // 
            this.Mfa_TB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Mfa_TB.Location = new System.Drawing.Point(9, 121);
            this.Mfa_TB.Multiline = true;
            this.Mfa_TB.Name = "Mfa_TB";
            this.Mfa_TB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Mfa_TB.Size = new System.Drawing.Size(880, 94);
            this.Mfa_TB.TabIndex = 0;
            this.Mfa_TB.WordWrap = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.EC2Items_TB);
            this.groupBox3.Location = new System.Drawing.Point(14, 703);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(907, 75);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "EC2 Items";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(426, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Comma seperated list of EC2 instance default properties to display";
            // 
            // EC2Items_TB
            // 
            this.EC2Items_TB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EC2Items_TB.Location = new System.Drawing.Point(12, 43);
            this.EC2Items_TB.Multiline = true;
            this.EC2Items_TB.Name = "EC2Items_TB";
            this.EC2Items_TB.Size = new System.Drawing.Size(880, 24);
            this.EC2Items_TB.TabIndex = 1;
            this.EC2Items_TB.WordWrap = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.Tags_TB);
            this.groupBox4.Location = new System.Drawing.Point(14, 480);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(907, 206);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Tags";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(548, 34);
            this.label4.TabIndex = 2;
            this.label4.Text = "List of tags that can be displayed. This list will appear in the list of choosabl" +
    "e columns.\r\nUse the format Tag=tagName";
            // 
            // Tags_TB
            // 
            this.Tags_TB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Tags_TB.Location = new System.Drawing.Point(12, 58);
            this.Tags_TB.Multiline = true;
            this.Tags_TB.Name = "Tags_TB";
            this.Tags_TB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Tags_TB.Size = new System.Drawing.Size(880, 133);
            this.Tags_TB.TabIndex = 0;
            this.Tags_TB.WordWrap = false;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 845);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Cancel_BTN);
            this.Controls.Add(this.Save_BTN);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Name = "Options";
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Roles_TB;
        private System.Windows.Forms.Button Cancel_BTN;
        private System.Windows.Forms.Button Save_BTN;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Mfa_TB;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox EC2Items_TB;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Tags_TB;
    }
}