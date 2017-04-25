namespace RouteChecker
{
    partial class App
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AWSRegion_CBB = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.AssumeRole_CBB = new System.Windows.Forms.ComboBox();
            this.Profile_CBB = new System.Windows.Forms.ComboBox();
            this.Submit_BTN = new System.Windows.Forms.Button();
            this.Inbound_DataGrid = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.Status_LB = new System.Windows.Forms.ToolStripStatusLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.Source_TB = new System.Windows.Forms.TextBox();
            this.Dest_TB = new System.Windows.Forms.TextBox();
            this.Port_TB = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.InboundTAB = new System.Windows.Forms.TabPage();
            this.OutboundTAB = new System.Windows.Forms.TabPage();
            this.Outbound_DataGrid = new System.Windows.Forms.DataGridView();
            this.Port_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Source_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceIP_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dest_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestIP_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestSG_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestProtocol_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestPortRange_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestCIDR_In = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Port_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Source_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceIP_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceSG_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceProtocol_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourcePortRange_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceCidr_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dest_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestIP_Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Inbound_DataGrid)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.InboundTAB.SuspendLayout();
            this.OutboundTAB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Outbound_DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1180, 28);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(130, 24);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AWSRegion_CBB);
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 71);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Region:";
            // 
            // AWSRegion_CBB
            // 
            this.AWSRegion_CBB.FormattingEnabled = true;
            this.AWSRegion_CBB.Location = new System.Drawing.Point(17, 26);
            this.AWSRegion_CBB.Name = "AWSRegion_CBB";
            this.AWSRegion_CBB.Size = new System.Drawing.Size(197, 24);
            this.AWSRegion_CBB.Sorted = true;
            this.AWSRegion_CBB.TabIndex = 7;
            this.AWSRegion_CBB.Text = "select";
            this.AWSRegion_CBB.SelectedIndexChanged += new System.EventHandler(this.AWSRegion_CBB_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.AssumeRole_CBB);
            this.groupBox2.Controls.Add(this.Profile_CBB);
            this.groupBox2.Location = new System.Drawing.Point(261, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(907, 71);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Authentication:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(441, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Assume Role:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Profile:";
            // 
            // AssumeRole_CBB
            // 
            this.AssumeRole_CBB.FormattingEnabled = true;
            this.AssumeRole_CBB.Location = new System.Drawing.Point(542, 27);
            this.AssumeRole_CBB.Name = "AssumeRole_CBB";
            this.AssumeRole_CBB.Size = new System.Drawing.Size(351, 24);
            this.AssumeRole_CBB.TabIndex = 5;
            this.AssumeRole_CBB.Text = "none";
            this.AssumeRole_CBB.SelectedIndexChanged += new System.EventHandler(this.AssumeRole_CBB_SelectedIndexChanged);
            // 
            // Profile_CBB
            // 
            this.Profile_CBB.FormattingEnabled = true;
            this.Profile_CBB.Location = new System.Drawing.Point(76, 26);
            this.Profile_CBB.Name = "Profile_CBB";
            this.Profile_CBB.Size = new System.Drawing.Size(322, 24);
            this.Profile_CBB.Sorted = true;
            this.Profile_CBB.TabIndex = 2;
            this.Profile_CBB.Text = "select a profile";
            // 
            // Submit_BTN
            // 
            this.Submit_BTN.Location = new System.Drawing.Point(12, 126);
            this.Submit_BTN.Name = "Submit_BTN";
            this.Submit_BTN.Size = new System.Drawing.Size(75, 25);
            this.Submit_BTN.TabIndex = 16;
            this.Submit_BTN.Text = "Submit";
            this.Submit_BTN.UseVisualStyleBackColor = true;
            this.Submit_BTN.Click += new System.EventHandler(this.Submit_BTN_Click);
            // 
            // Inbound_DataGrid
            // 
            this.Inbound_DataGrid.AllowUserToOrderColumns = true;
            this.Inbound_DataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Inbound_DataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.Inbound_DataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Inbound_DataGrid.BackgroundColor = System.Drawing.Color.White;
            this.Inbound_DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Inbound_DataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Port_In,
            this.Source_In,
            this.SourceIP_In,
            this.Dest_In,
            this.DestIP_In,
            this.DestSG_In,
            this.DestProtocol_In,
            this.DestPortRange_In,
            this.DestCIDR_In});
            this.Inbound_DataGrid.Location = new System.Drawing.Point(3, 3);
            this.Inbound_DataGrid.Name = "Inbound_DataGrid";
            this.Inbound_DataGrid.RowTemplate.Height = 24;
            this.Inbound_DataGrid.Size = new System.Drawing.Size(1141, 281);
            this.Inbound_DataGrid.TabIndex = 17;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar1,
            this.Status_LB});
            this.statusStrip1.Location = new System.Drawing.Point(0, 727);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1180, 25);
            this.statusStrip1.TabIndex = 18;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(200, 19);
            // 
            // Status_LB
            // 
            this.Status_LB.Name = "Status_LB";
            this.Status_LB.Size = new System.Drawing.Size(49, 20);
            this.Status_LB.Text = "Status";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.WorkerReportsProgress = true;
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // Source_TB
            // 
            this.Source_TB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Source_TB.Location = new System.Drawing.Point(16, 30);
            this.Source_TB.Multiline = true;
            this.Source_TB.Name = "Source_TB";
            this.Source_TB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Source_TB.Size = new System.Drawing.Size(184, 152);
            this.Source_TB.TabIndex = 19;
            this.Source_TB.Text = "172.23.36.46";
            this.Source_TB.WordWrap = false;
            // 
            // Dest_TB
            // 
            this.Dest_TB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Dest_TB.Location = new System.Drawing.Point(19, 30);
            this.Dest_TB.Multiline = true;
            this.Dest_TB.Name = "Dest_TB";
            this.Dest_TB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Dest_TB.Size = new System.Drawing.Size(178, 152);
            this.Dest_TB.TabIndex = 20;
            this.Dest_TB.Text = "172.23.36.82";
            this.Dest_TB.WordWrap = false;
            // 
            // Port_TB
            // 
            this.Port_TB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Port_TB.Location = new System.Drawing.Point(16, 30);
            this.Port_TB.Multiline = true;
            this.Port_TB.Name = "Port_TB";
            this.Port_TB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Port_TB.Size = new System.Drawing.Size(182, 152);
            this.Port_TB.TabIndex = 21;
            this.Port_TB.Text = "16163";
            this.Port_TB.WordWrap = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Source_TB);
            this.groupBox3.Location = new System.Drawing.Point(13, 169);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(213, 194);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Source";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Port_TB);
            this.groupBox4.Location = new System.Drawing.Point(584, 169);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(213, 194);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Port";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Dest_TB);
            this.groupBox5.Location = new System.Drawing.Point(300, 169);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(213, 194);
            this.groupBox5.TabIndex = 24;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Destination";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.OutboundTAB);
            this.tabControl1.Controls.Add(this.InboundTAB);
            this.tabControl1.Location = new System.Drawing.Point(13, 391);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1155, 319);
            this.tabControl1.TabIndex = 25;
            // 
            // InboundTAB
            // 
            this.InboundTAB.Controls.Add(this.Inbound_DataGrid);
            this.InboundTAB.Location = new System.Drawing.Point(4, 25);
            this.InboundTAB.Name = "InboundTAB";
            this.InboundTAB.Padding = new System.Windows.Forms.Padding(3);
            this.InboundTAB.Size = new System.Drawing.Size(1147, 290);
            this.InboundTAB.TabIndex = 0;
            this.InboundTAB.Text = "Destination Inbound Rules";
            this.InboundTAB.UseVisualStyleBackColor = true;
            // 
            // OutboundTAB
            // 
            this.OutboundTAB.Controls.Add(this.Outbound_DataGrid);
            this.OutboundTAB.Location = new System.Drawing.Point(4, 25);
            this.OutboundTAB.Name = "OutboundTAB";
            this.OutboundTAB.Padding = new System.Windows.Forms.Padding(3);
            this.OutboundTAB.Size = new System.Drawing.Size(1147, 290);
            this.OutboundTAB.TabIndex = 1;
            this.OutboundTAB.Text = "Source Outbound Rules";
            this.OutboundTAB.UseVisualStyleBackColor = true;
            // 
            // Outbound_DataGrid
            // 
            this.Outbound_DataGrid.AllowUserToOrderColumns = true;
            this.Outbound_DataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Outbound_DataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.Outbound_DataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Outbound_DataGrid.BackgroundColor = System.Drawing.Color.White;
            this.Outbound_DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Outbound_DataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Port_Out,
            this.Source_Out,
            this.SourceIP_Out,
            this.SourceSG_Out,
            this.SourceProtocol_Out,
            this.SourcePortRange_Out,
            this.SourceCidr_Out,
            this.Dest_Out,
            this.DestIP_Out});
            this.Outbound_DataGrid.Location = new System.Drawing.Point(3, 5);
            this.Outbound_DataGrid.Name = "Outbound_DataGrid";
            this.Outbound_DataGrid.RowTemplate.Height = 24;
            this.Outbound_DataGrid.Size = new System.Drawing.Size(1141, 281);
            this.Outbound_DataGrid.TabIndex = 18;
            // 
            // Port_In
            // 
            this.Port_In.HeaderText = "Port";
            this.Port_In.Name = "Port_In";
            // 
            // Source_In
            // 
            this.Source_In.HeaderText = "Source";
            this.Source_In.Name = "Source_In";
            // 
            // SourceIP_In
            // 
            this.SourceIP_In.HeaderText = "Source IP";
            this.SourceIP_In.Name = "SourceIP_In";
            // 
            // Dest_In
            // 
            this.Dest_In.HeaderText = "Destination";
            this.Dest_In.Name = "Dest_In";
            // 
            // DestIP_In
            // 
            this.DestIP_In.HeaderText = "Destination IP";
            this.DestIP_In.Name = "DestIP_In";
            // 
            // DestSG_In
            // 
            this.DestSG_In.HeaderText = "Destination SG";
            this.DestSG_In.Name = "DestSG_In";
            // 
            // DestProtocol_In
            // 
            this.DestProtocol_In.HeaderText = "Protocol";
            this.DestProtocol_In.Name = "DestProtocol_In";
            // 
            // DestPortRange_In
            // 
            this.DestPortRange_In.HeaderText = "Port Range";
            this.DestPortRange_In.Name = "DestPortRange_In";
            // 
            // DestCIDR_In
            // 
            this.DestCIDR_In.HeaderText = "CIDR/Group";
            this.DestCIDR_In.Name = "DestCIDR_In";
            // 
            // Port_Out
            // 
            this.Port_Out.HeaderText = "Port";
            this.Port_Out.Name = "Port_Out";
            // 
            // Source_Out
            // 
            this.Source_Out.HeaderText = "Source";
            this.Source_Out.Name = "Source_Out";
            // 
            // SourceIP_Out
            // 
            this.SourceIP_Out.HeaderText = "Source IP";
            this.SourceIP_Out.Name = "SourceIP_Out";
            // 
            // SourceSG_Out
            // 
            this.SourceSG_Out.HeaderText = "Source SG";
            this.SourceSG_Out.Name = "SourceSG_Out";
            // 
            // SourceProtocol_Out
            // 
            this.SourceProtocol_Out.HeaderText = "Protcol";
            this.SourceProtocol_Out.Name = "SourceProtocol_Out";
            // 
            // SourcePortRange_Out
            // 
            this.SourcePortRange_Out.HeaderText = "Port Range";
            this.SourcePortRange_Out.Name = "SourcePortRange_Out";
            // 
            // SourceCidr_Out
            // 
            this.SourceCidr_Out.HeaderText = "CIDR/Group";
            this.SourceCidr_Out.Name = "SourceCidr_Out";
            // 
            // Dest_Out
            // 
            this.Dest_Out.HeaderText = "Destination";
            this.Dest_Out.Name = "Dest_Out";
            // 
            // DestIP_Out
            // 
            this.DestIP_Out.HeaderText = "Destination IP";
            this.DestIP_Out.Name = "DestIP_Out";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(381, 382);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 26;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // App
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 752);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Submit_BTN);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Name = "App";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.App_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Inbound_DataGrid)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.InboundTAB.ResumeLayout(false);
            this.OutboundTAB.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Outbound_DataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox AWSRegion_CBB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox AssumeRole_CBB;
        private System.Windows.Forms.ComboBox Profile_CBB;
        private System.Windows.Forms.Button Submit_BTN;
        private System.Windows.Forms.DataGridView Inbound_DataGrid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel Status_LB;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.Windows.Forms.TextBox Source_TB;
        private System.Windows.Forms.TextBox Dest_TB;
        private System.Windows.Forms.TextBox Port_TB;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestCidr;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage InboundTAB;
        private System.Windows.Forms.TabPage OutboundTAB;
        private System.Windows.Forms.DataGridView Outbound_DataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Port_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceIP_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dest_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestIP_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestSG_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestProtocol_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestPortRange_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestCIDR_In;
        private System.Windows.Forms.DataGridViewTextBoxColumn Port_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceIP_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceSG_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceProtocol_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourcePortRange_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceCidr_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dest_Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestIP_Out;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button1;
    }
}

