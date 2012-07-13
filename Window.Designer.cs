/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/

namespace Client
{
    partial class Window
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.xContainer1 = new System.Windows.Forms.SplitContainer();
            this.xContainer4 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listView = new System.Windows.Forms.ListView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.whoisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickBanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dCCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewd = new System.Windows.Forms.ListView();
            this.textbox = new Client.TextBox();
            this.qToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.qToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.oToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.hToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.vToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.vERSIONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pINGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pAGEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tIMEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xContainer1.Panel1.SuspendLayout();
            this.xContainer1.Panel2.SuspendLayout();
            this.xContainer1.SuspendLayout();
			this.xContainer4.Panel1.SuspendLayout();
            this.xContainer4.Panel2.SuspendLayout();
            this.xContainer4.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // xContainer1
            // 
            this.xContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xContainer1.Location = new System.Drawing.Point(0, 0);
            this.xContainer1.Name = "xContainer1";
            this.xContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // xContainer1.Panel1
            // 
            this.xContainer1.Panel1.Controls.Add(this.xContainer4);
            // 
            // xContainer1.Panel2
            // 
            this.xContainer1.Panel2.Controls.Add(this.textbox);
            this.xContainer1.Size = new System.Drawing.Size(749, 333);
            this.xContainer1.SplitterDistance = 166;
            this.xContainer1.TabIndex = 0;
            this.xContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.Changed);
            // 
            // xContainer4
            // 
            this.xContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xContainer4.Location = new System.Drawing.Point(0, 0);
            this.xContainer4.Name = "xContainer4";
            // 
            // xContainer4.Panel1
            // 
            this.xContainer4.Panel1.Controls.Add(this.scrollback);
            
            // 
            // xContainer4.Panel2
            // 
            this.xContainer4.Panel2.Controls.Add(this.listViewd);
            this.xContainer4.Panel2.Controls.Add(this.listView);
            this.xContainer4.Size = new System.Drawing.Size(749, 166);
            this.xContainer4.SplitterDistance = 249;
            this.xContainer4.TabIndex = 0;
            this.xContainer4.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.Changed);
            // 
            // scrollback
            // 
            this.scrollback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scrollback.ContextMenuStrip = this.contextMenuStrip1;
            this.scrollback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollback.Location = new System.Drawing.Point(0, 0);
            this.scrollback.Name = "scrollback";
            this.scrollback.Size = new System.Drawing.Size(249, 166);
            this.scrollback.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // listView
            // 
            this.listView.ContextMenuStrip = this.contextMenuStrip2;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(496, 166);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whoisToolStripMenuItem,
            this.modeToolStripMenuItem,
            this.ctToolStripMenuItem,
            this.messageToolStripMenuItem,
            this.toolStripMenuItem2,
            this.kickToolStripMenuItem,
            this.banToolStripMenuItem,
            this.kickBanToolStripMenuItem,
            this.kickrToolStripMenuItem,
            this.kbToolStripMenuItem,
            this.toolStripMenuItem3,
            this.refreshToolStripMenuItem,
            this.dCCToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(131, 258);
            // 
            // whoisToolStripMenuItem
            // 
            this.whoisToolStripMenuItem.Name = "whoisToolStripMenuItem";
            this.whoisToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.whoisToolStripMenuItem.Text = "Whois";
            this.whoisToolStripMenuItem.Click += new System.EventHandler(this.whoisToolStripMenuItem_Click);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.qToolStripMenuItem,
                this.aToolStripMenuItem,
                this.oToolStripMenuItem,
                this.hToolStripMenuItem,
                this.vToolStripMenuItem,
                this.toolStripMenuItem3,
                this.qToolStripMenuItem1,
                this.aToolStripMenuItem1,
                this.oToolStripMenuItem1,
                this.hToolStripMenuItem1,
                this.vToolStripMenuItem1,
            });
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // ctToolStripMenuItem
            // 
            this.ctToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.vERSIONToolStripMenuItem,
                this.tIMEToolStripMenuItem,
            });
            this.ctToolStripMenuItem.Name = "ctToolStripMenuItem";
            this.ctToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ctToolStripMenuItem.Text = "ctcp";
            // 
            // messageToolStripMenuItem
            // 
            this.messageToolStripMenuItem.Name = "messageToolStripMenuItem";
            this.messageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.messageToolStripMenuItem.Text = "Message";
            this.messageToolStripMenuItem.Click += new System.EventHandler(this.messageToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kickToolStripMenuItem.Text = "Kick";
            this.kickToolStripMenuItem.Click += new System.EventHandler(this.kickToolStripMenuItem_Click);
            // 
            // banToolStripMenuItem
            // 
            this.banToolStripMenuItem.Name = "banToolStripMenuItem";
            this.banToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.banToolStripMenuItem.Text = "Ban";
            this.banToolStripMenuItem.Click += new System.EventHandler(this.banToolStripMenuItem_Click);
            // 
            // kickBanToolStripMenuItem
            // 
            this.kickBanToolStripMenuItem.Name = "kickBanToolStripMenuItem";
            this.kickBanToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kickBanToolStripMenuItem.Text = "Kick + Ban";
            this.kickBanToolStripMenuItem.Click += new System.EventHandler(this.kickBanToolStripMenuItem_Click);
            // 
            // kickrToolStripMenuItem
            // 
            this.kickrToolStripMenuItem.Name = "kickrToolStripMenuItem";
            this.kickrToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kickrToolStripMenuItem.Text = "Kick rs";
            this.kickrToolStripMenuItem.Click += new System.EventHandler(this.kickrToolStripMenuItem_Click);
            // 
            // kbToolStripMenuItem
            // 
            this.kbToolStripMenuItem.Name = "kbToolStripMenuItem";
            this.kbToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kbToolStripMenuItem.Text = "kr";
            this.kbToolStripMenuItem.Click += new System.EventHandler(this.krToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(149, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // dCCToolStripMenuItem
            // 
            this.dCCToolStripMenuItem.Name = "dCCToolStripMenuItem";
            this.dCCToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dCCToolStripMenuItem.Text = "DCC";
            // 
            // listViewd
            // 
            this.listViewd.ContextMenuStrip = this.contextMenuStrip2;
            this.listViewd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewd.FullRowSelect = true;
            this.listViewd.Location = new System.Drawing.Point(0, 0);
            this.listViewd.Name = "listViewd";
            this.listViewd.ShowItemToolTips = true;
            this.listViewd.Size = new System.Drawing.Size(496, 166);
            this.listViewd.TabIndex = 2;
            this.listViewd.UseCompatibleStateImageBehavior = false;
            this.listViewd.SelectedIndexChanged += new System.EventHandler(this.listViewd_SelectedIndexChanged);
            // 
            // textbox
            // 
            this.textbox.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textbox.Location = new System.Drawing.Point(0, 0);
            this.textbox.Name = "textbox";
            this.textbox.Size = new System.Drawing.Size(749, 163);
            this.textbox.TabIndex = 1;
            this.textbox.Load += new System.EventHandler(this.textbox_Load);
            // 
            // qToolStripMenuItem
            // 
            this.qToolStripMenuItem.Name = "qToolStripMenuItem";
            this.qToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.qToolStripMenuItem.Text = "+q";
            this.qToolStripMenuItem.Click += new System.EventHandler(this.qToolStripMenuItem_Click);
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aToolStripMenuItem.Text = "+a";
            this.aToolStripMenuItem.Click += new System.EventHandler(this.aToolStripMenuItem_Click);
            // 
            // oToolStripMenuItem
            // 
            this.oToolStripMenuItem.Name = "oToolStripMenuItem";
            this.oToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.oToolStripMenuItem.Text = "+o";
            this.oToolStripMenuItem.Click += new System.EventHandler(this.oToolStripMenuItem_Click);
            // 
            // hToolStripMenuItem
            // 
            this.hToolStripMenuItem.Name = "hToolStripMenuItem";
            this.hToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hToolStripMenuItem.Text = "+h";
            this.hToolStripMenuItem.Click += new System.EventHandler(this.hToolStripMenuItem_Click);
            // 
            // vToolStripMenuItem
            // 
            this.vToolStripMenuItem.Name = "vToolStripMenuItem";
            this.vToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.vToolStripMenuItem.Text = "+v";
            this.vToolStripMenuItem.Click += new System.EventHandler(this.vToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // qToolStripMenuItem1
            // 
            this.qToolStripMenuItem1.Name = "qToolStripMenuItem1";
            this.qToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.qToolStripMenuItem1.Text = "-q";
            this.qToolStripMenuItem1.Click += new System.EventHandler(this.qToolStripMenuItem1_Click);
            // 
            // aToolStripMenuItem1
            // 
            this.aToolStripMenuItem1.Name = "aToolStripMenuItem1";
            this.aToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.aToolStripMenuItem1.Text = "-a";
            this.aToolStripMenuItem1.Click += new System.EventHandler(this.aToolStripMenuItem1_Click);
            // 
            // oToolStripMenuItem1
            // 
            this.oToolStripMenuItem1.Name = "oToolStripMenuItem1";
            this.oToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.oToolStripMenuItem1.Text = "-o";
            this.oToolStripMenuItem1.Click += new System.EventHandler(this.oToolStripMenuItem1_Click);
            // 
            // hToolStripMenuItem1
            // 
            this.hToolStripMenuItem1.Name = "hToolStripMenuItem1";
            this.hToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.hToolStripMenuItem1.Text = "-h";
            this.hToolStripMenuItem1.Click += new System.EventHandler(this.hToolStripMenuItem1_Click);
            // 
            // vToolStripMenuItem1
            // 
            this.vToolStripMenuItem1.Name = "vToolStripMenuItem1";
            this.vToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.vToolStripMenuItem1.Text = "-v";
            this.vToolStripMenuItem1.Click += new System.EventHandler(this.vToolStripMenuItem1_Click);
            // 
            // vERSIONToolStripMenuItem
            // 
            this.vERSIONToolStripMenuItem.Name = "vERSIONToolStripMenuItem";
            this.vERSIONToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.vERSIONToolStripMenuItem.Text = "VERSION";
            this.vERSIONToolStripMenuItem.Click += new System.EventHandler(this.vERSIONToolStripMenuItem_Click);
            // 
            // pINGToolStripMenuItem
            // 
            this.pINGToolStripMenuItem.Name = "pINGToolStripMenuItem";
            this.pINGToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pINGToolStripMenuItem.Text = "PING";
            this.pINGToolStripMenuItem.Click += new System.EventHandler(this.pINGToolStripMenuItem_Click);
            // 
            // pAGEToolStripMenuItem
            // 
            this.pAGEToolStripMenuItem.Name = "pAGEToolStripMenuItem";
            this.pAGEToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pAGEToolStripMenuItem.Text = "PAGE";
            this.pAGEToolStripMenuItem.Click += new System.EventHandler(this.pAGEToolStripMenuItem_Click);
            // 
            // tIMEToolStripMenuItem
            // 
            this.tIMEToolStripMenuItem.Name = "tIMEToolStripMenuItem";
            this.tIMEToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tIMEToolStripMenuItem.Text = "TIME";
            this.tIMEToolStripMenuItem.Click += new System.EventHandler(this.tIMEToolStripMenuItem_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xContainer1);
            this.Name = "Window";
            this.Size = new System.Drawing.Size(749, 333);
            this.xContainer1.Panel1.ResumeLayout(false);
            this.xContainer1.Panel2.ResumeLayout(false);
            this.xContainer1.ResumeLayout(false);
            this.xContainer4.Panel1.ResumeLayout(false);
            this.xContainer4.Panel2.ResumeLayout(false);
            this.xContainer4.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.SplitContainer xContainer1;
        public System.Windows.Forms.SplitContainer xContainer4;
        public TextBox textbox;
        public Scrollback scrollback;
        public System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem whoisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem banToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kickBanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem qToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem oToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem hToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem vToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem kickrToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kbToolStripMenuItem;
        public System.Windows.Forms.ListView listViewd;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ctToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vERSIONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pINGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pAGEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tIMEToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem messageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dCCToolStripMenuItem;
        //public PidgeonList pidgeonList;

    }
}
