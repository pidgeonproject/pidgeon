namespace Client
{
    partial class Main
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripCurrent = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusNetwork = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusChannel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripCo = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.shutDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelList1 = new Client.ChannelList();
            this.Scrollback = new Client.Scrollback();
            this.listView = new System.Windows.Forms.ListView();
            this.MessageLine = new Client.TextBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCurrent,
            this.toolStripInfo,
            this.toolStripStatusNetwork,
            this.toolStripStatusChannel,
            this.toolStripCo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 388);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(874, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripCurrent
            // 
            this.toolStripCurrent.Name = "toolStripCurrent";
            this.toolStripCurrent.Size = new System.Drawing.Size(92, 17);
            this.toolStripCurrent.Text = "toolStripCurrent";
            // 
            // toolStripInfo
            // 
            this.toolStripInfo.Name = "toolStripInfo";
            this.toolStripInfo.Size = new System.Drawing.Size(73, 17);
            this.toolStripInfo.Text = "toolStripInfo";
            // 
            // toolStripStatusNetwork
            // 
            this.toolStripStatusNetwork.Name = "toolStripStatusNetwork";
            this.toolStripStatusNetwork.Size = new System.Drawing.Size(129, 17);
            this.toolStripStatusNetwork.Text = "toolStripStatusNetwork";
            // 
            // toolStripStatusChannel
            // 
            this.toolStripStatusChannel.Name = "toolStripStatusChannel";
            this.toolStripStatusChannel.Size = new System.Drawing.Size(128, 17);
            this.toolStripStatusChannel.Text = "toolStripStatusChannel";
            // 
            // toolStripCo
            // 
            this.toolStripCo.Name = "toolStripCo";
            this.toolStripCo.Size = new System.Drawing.Size(67, 17);
            this.toolStripCo.Text = "toolStripCo";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.userToolStripMenuItem,
            this.helToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(874, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "ms";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newConnectionToolStripMenuItem,
            this.toolStripMenuItem1,
            this.preferencesToolStripMenuItem,
            this.toolStripMenuItem2,
            this.shutDownToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newConnectionToolStripMenuItem
            // 
            this.newConnectionToolStripMenuItem.Name = "newConnectionToolStripMenuItem";
            this.newConnectionToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.newConnectionToolStripMenuItem.Text = "New connection";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
            // 
            // shutDownToolStripMenuItem
            // 
            this.shutDownToolStripMenuItem.Name = "shutDownToolStripMenuItem";
            this.shutDownToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.shutDownToolStripMenuItem.Text = "Shut down";
            this.shutDownToolStripMenuItem.Click += new System.EventHandler(this.shutDownToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // userToolStripMenuItem
            // 
            this.userToolStripMenuItem.Name = "userToolStripMenuItem";
            this.userToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.userToolStripMenuItem.Text = "User";
            // 
            // helToolStripMenuItem
            // 
            this.helToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helToolStripMenuItem.Name = "helToolStripMenuItem";
            this.helToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helToolStripMenuItem.Text = "Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.contentsToolStripMenuItem.Text = "Contents";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // channelList1
            // 
            this.channelList1.Location = new System.Drawing.Point(12, 27);
            this.channelList1.Name = "channelList1";
            this.channelList1.Size = new System.Drawing.Size(152, 332);
            this.channelList1.TabIndex = 2;
            this.channelList1.Load += new System.EventHandler(this.channelList1_Load);
            // 
            // Scrollback
            // 
            this.Scrollback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Scrollback.Location = new System.Drawing.Point(189, 27);
            this.Scrollback.Name = "Scrollback";
            this.Scrollback.Size = new System.Drawing.Size(465, 195);
            this.Scrollback.TabIndex = 9;
            // 
            // listView
            // 
            this.listView.Location = new System.Drawing.Point(669, 27);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(155, 229);
            this.listView.TabIndex = 8;
            this.listView.UseCompatibleStateImageBehavior = false;
            // 
            // MessageLine
            // 
            this.MessageLine.Location = new System.Drawing.Point(189, 278);
            this.MessageLine.Name = "MessageLine";
            this.MessageLine.Size = new System.Drawing.Size(448, 29);
            this.MessageLine.TabIndex = 7;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 410);
            this.Controls.Add(this.Scrollback);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.MessageLine);
            this.Controls.Add(this.channelList1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "Pidgeon Client v 1.0";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Resize += new System.EventHandler(this.ResizeMe);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripCurrent;
        private System.Windows.Forms.ToolStripStatusLabel toolStripInfo;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusNetwork;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusChannel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripCo;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem shutDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        public Scrollback Scrollback;
        public System.Windows.Forms.ListView listView;
        public ChannelList channelList1;
        public TextBox MessageLine;
    }
}