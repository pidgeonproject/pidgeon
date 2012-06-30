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
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

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
            this.scrollback = new Client.Scrollback();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listView = new System.Windows.Forms.ListView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.whoisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickBanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textbox = new Client.TextBox();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.qToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.oToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.hToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.vToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.xContainer1)).BeginInit();
            this.xContainer1.Panel1.SuspendLayout();
            this.xContainer1.Panel2.SuspendLayout();
            this.xContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xContainer4)).BeginInit();
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
            this.xContainer4.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint_1);
            // 
            // xContainer4.Panel2
            // 
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
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(496, 166);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whoisToolStripMenuItem,
            this.modeToolStripMenuItem,
            this.kickToolStripMenuItem,
            this.banToolStripMenuItem,
            this.kickBanToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(153, 136);
            // 
            // whoisToolStripMenuItem
            // 
            this.whoisToolStripMenuItem.Name = "whoisToolStripMenuItem";
            this.whoisToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.whoisToolStripMenuItem.Text = "Whois";
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.qToolStripMenuItem,
            this.aToolStripMenuItem,
            this.oToolStripMenuItem,
            this.hToolStripMenuItem,
            this.vToolStripMenuItem,
            this.toolStripMenuItem1,
            this.qToolStripMenuItem1,
            this.aToolStripMenuItem1,
            this.oToolStripMenuItem1,
            this.hToolStripMenuItem1,
            this.vToolStripMenuItem1});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // qToolStripMenuItem
            // 
            this.qToolStripMenuItem.Name = "qToolStripMenuItem";
            this.qToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.qToolStripMenuItem.Text = "+q";
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aToolStripMenuItem.Text = "+a";
            // 
            // oToolStripMenuItem
            // 
            this.oToolStripMenuItem.Name = "oToolStripMenuItem";
            this.oToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.oToolStripMenuItem.Text = "+o";
            // 
            // hToolStripMenuItem
            // 
            this.hToolStripMenuItem.Name = "hToolStripMenuItem";
            this.hToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hToolStripMenuItem.Text = "+h";
            // 
            // vToolStripMenuItem
            // 
            this.vToolStripMenuItem.Name = "vToolStripMenuItem";
            this.vToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.vToolStripMenuItem.Text = "+v";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kickToolStripMenuItem.Text = "Kick";
            // 
            // banToolStripMenuItem
            // 
            this.banToolStripMenuItem.Name = "banToolStripMenuItem";
            this.banToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.banToolStripMenuItem.Text = "Ban";
            // 
            // kickBanToolStripMenuItem
            // 
            this.kickBanToolStripMenuItem.Name = "kickBanToolStripMenuItem";
            this.kickBanToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kickBanToolStripMenuItem.Text = "Kick + Ban";
            this.kickBanToolStripMenuItem.Click += new System.EventHandler(this.kickBanToolStripMenuItem_Click);
            // 
            // textbox
            // 
            this.textbox.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.textbox.ContextMenuStrip = this.contextMenuStrip3;
            this.textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textbox.Location = new System.Drawing.Point(0, 0);
            this.textbox.Name = "textbox";
            this.textbox.Size = new System.Drawing.Size(749, 163);
            this.textbox.TabIndex = 1;
            this.textbox.Load += new System.EventHandler(this.textbox_Load);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(61, 4);
            // 
            // qToolStripMenuItem1
            // 
            this.qToolStripMenuItem1.Name = "qToolStripMenuItem1";
            this.qToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.qToolStripMenuItem1.Text = "-q";
            // 
            // aToolStripMenuItem1
            // 
            this.aToolStripMenuItem1.Name = "aToolStripMenuItem1";
            this.aToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.aToolStripMenuItem1.Text = "-a";
            // 
            // oToolStripMenuItem1
            // 
            this.oToolStripMenuItem1.Name = "oToolStripMenuItem1";
            this.oToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.oToolStripMenuItem1.Text = "-o";
            // 
            // hToolStripMenuItem1
            // 
            this.hToolStripMenuItem1.Name = "hToolStripMenuItem1";
            this.hToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.hToolStripMenuItem1.Text = "-h";
            // 
            // vToolStripMenuItem1
            // 
            this.vToolStripMenuItem1.Name = "vToolStripMenuItem1";
            this.vToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.vToolStripMenuItem1.Text = "-v";
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
            ((System.ComponentModel.ISupportInitialize)(this.xContainer1)).EndInit();
            this.xContainer1.ResumeLayout(false);
            this.xContainer4.Panel1.ResumeLayout(false);
            this.xContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xContainer4)).EndInit();
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
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
        //public PidgeonList pidgeonList;

    }
}
