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
    partial class Scrollback
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openLinkInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mode1b2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mode1q2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mode1e2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mode1I2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mrhToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retrieveTopicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleSimpleLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleAdvancedLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refresh = new System.Windows.Forms.Timer(this.components);
            this.pwx1 = new System.Windows.Forms.Panel();
            this.Data = new System.Windows.Forms.WebBrowser();
            this.pwx2 = new System.Windows.Forms.Panel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.simpleview = new System.Windows.Forms.TextBox();
            this.joinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.pwx1.SuspendLayout();
            this.pwx2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openLinkInBrowserToolStripMenuItem,
            this.mode1b2ToolStripMenuItem,
            this.mode1q2ToolStripMenuItem,
            this.mode1e2ToolStripMenuItem,
            this.joinToolStripMenuItem,
            this.mode1I2ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mrhToolStripMenuItem,
            this.scrollToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.channelToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.retrieveTopicToolStripMenuItem,
            this.toggleSimpleLayoutToolStripMenuItem,
            this.toggleAdvancedLayoutToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(202, 340);
            // 
            // openLinkInBrowserToolStripMenuItem
            // 
            this.openLinkInBrowserToolStripMenuItem.Name = "openLinkInBrowserToolStripMenuItem";
            this.openLinkInBrowserToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openLinkInBrowserToolStripMenuItem.Text = "Open link in browser";
            this.openLinkInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openLinkInBrowserToolStripMenuItem_Click);
            // 
            // mode1b2ToolStripMenuItem
            // 
            this.mode1b2ToolStripMenuItem.Name = "mode1b2ToolStripMenuItem";
            this.mode1b2ToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.mode1b2ToolStripMenuItem.Text = "/mode $1 +b $2";
            this.mode1b2ToolStripMenuItem.Click += new System.EventHandler(this.mode1b2ToolStripMenuItem_Click);
            // 
            // mode1q2ToolStripMenuItem
            // 
            this.mode1q2ToolStripMenuItem.Name = "mode1q2ToolStripMenuItem";
            this.mode1q2ToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.mode1q2ToolStripMenuItem.Text = "/mode $1 +q $2";
            this.mode1q2ToolStripMenuItem.Click += new System.EventHandler(this.mode1q2ToolStripMenuItem_Click);
            // 
            // mode1e2ToolStripMenuItem
            // 
            this.mode1e2ToolStripMenuItem.Name = "mode1e2ToolStripMenuItem";
            this.mode1e2ToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.mode1e2ToolStripMenuItem.Text = "/mode $1 +e $2";
            this.mode1e2ToolStripMenuItem.Click += new System.EventHandler(this.mode1e2ToolStripMenuItem_Click);
            // 
            // mode1I2ToolStripMenuItem
            // 
            this.mode1I2ToolStripMenuItem.Name = "mode1I2ToolStripMenuItem";
            this.mode1I2ToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.mode1I2ToolStripMenuItem.Text = "/mode $1 +I $2";
            this.mode1I2ToolStripMenuItem.Click += new System.EventHandler(this.mode1I2ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(198, 6);
            // 
            // mrhToolStripMenuItem
            // 
            this.mrhToolStripMenuItem.Name = "mrhToolStripMenuItem";
            this.mrhToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.mrhToolStripMenuItem.Text = "Clean";
            this.mrhToolStripMenuItem.Click += new System.EventHandler(this.mrhToolStripMenuItem_Click);
            // 
            // scrollToolStripMenuItem
            // 
            this.scrollToolStripMenuItem.Checked = true;
            this.scrollToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.scrollToolStripMenuItem.Name = "scrollToolStripMenuItem";
            this.scrollToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.scrollToolStripMenuItem.Text = "Scroll";
            this.scrollToolStripMenuItem.Click += new System.EventHandler(this.scrollToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Visible = false;
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // channelToolStripMenuItem
            // 
            this.channelToolStripMenuItem.Name = "channelToolStripMenuItem";
            this.channelToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.channelToolStripMenuItem.Text = "Channel";
            this.channelToolStripMenuItem.Click += new System.EventHandler(this.channelToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // retrieveTopicToolStripMenuItem
            // 
            this.retrieveTopicToolStripMenuItem.Name = "retrieveTopicToolStripMenuItem";
            this.retrieveTopicToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.retrieveTopicToolStripMenuItem.Text = "Retrieve topic";
            this.retrieveTopicToolStripMenuItem.Click += new System.EventHandler(this.retrieveTopicToolStripMenuItem_Click);
            // 
            // toggleSimpleLayoutToolStripMenuItem
            // 
            this.toggleSimpleLayoutToolStripMenuItem.Name = "toggleSimpleLayoutToolStripMenuItem";
            this.toggleSimpleLayoutToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.toggleSimpleLayoutToolStripMenuItem.Text = "Toggle simple layout";
            this.toggleSimpleLayoutToolStripMenuItem.Click += new System.EventHandler(this.toggleSimpleLayoutToolStripMenuItem_Click);
            // 
            // toggleAdvancedLayoutToolStripMenuItem
            // 
            this.toggleAdvancedLayoutToolStripMenuItem.Name = "toggleAdvancedLayoutToolStripMenuItem";
            this.toggleAdvancedLayoutToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.toggleAdvancedLayoutToolStripMenuItem.Text = "Toggle advanced layout";
            this.toggleAdvancedLayoutToolStripMenuItem.Click += new System.EventHandler(this.toggleAdvancedLayoutToolStripMenuItem_Click);
            // 
            // refresh
            // 
            this.refresh.Interval = 60;
            this.refresh.Tick += new System.EventHandler(this.refresh_Tick);
            // 
            // pwx1
            // 
            this.pwx1.Controls.Add(this.Data);
            this.pwx1.Location = new System.Drawing.Point(3, 21);
            this.pwx1.Name = "pwx1";
            this.pwx1.Size = new System.Drawing.Size(223, 209);
            this.pwx1.TabIndex = 3;
            this.pwx1.Resize += new System.EventHandler(this.ResizeWebs);
            // 
            // Data
            // 
            this.Data.ContextMenuStrip = this.contextMenuStrip1;
            this.Data.IsWebBrowserContextMenuEnabled = false;
            this.Data.Location = new System.Drawing.Point(3, 3);
            this.Data.MinimumSize = new System.Drawing.Size(20, 20);
            this.Data.Name = "Data";
            this.Data.ScriptErrorsSuppressed = true;
            this.Data.Size = new System.Drawing.Size(345, 262);
            this.Data.TabIndex = 2;
            this.Data.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.Clicked);
            // 
            // pwx2
            // 
            this.pwx2.Controls.Add(this.webBrowser1);
            this.pwx2.Location = new System.Drawing.Point(244, 21);
            this.pwx2.Name = "pwx2";
            this.pwx2.Size = new System.Drawing.Size(217, 209);
            this.pwx2.TabIndex = 4;
            // 
            // webBrowser1
            // 
            this.webBrowser1.ContextMenuStrip = this.contextMenuStrip1;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(-41, -69);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(345, 262);
            this.webBrowser1.TabIndex = 3;
            this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.Clicked);
            // 
            // simpleview
            // 
            this.simpleview.BackColor = System.Drawing.Color.White;
            this.simpleview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.simpleview.ContextMenuStrip = this.contextMenuStrip1;
            this.simpleview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleview.Location = new System.Drawing.Point(0, 0);
            this.simpleview.Multiline = true;
            this.simpleview.Name = "simpleview";
            this.simpleview.ReadOnly = true;
            this.simpleview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.simpleview.Size = new System.Drawing.Size(566, 320);
            this.simpleview.TabIndex = 6;
            // 
            // joinToolStripMenuItem
            // 
            this.joinToolStripMenuItem.Name = "joinToolStripMenuItem";
            this.joinToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.joinToolStripMenuItem.Text = "Join";
            this.joinToolStripMenuItem.Click += new System.EventHandler(this.joinToolStripMenuItem_Click);
            // 
            // Scrollback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.simpleview);
            this.Controls.Add(this.pwx2);
            this.Controls.Add(this.pwx1);
            this.DoubleBuffered = true;
            this.Name = "Scrollback";
            this.Size = new System.Drawing.Size(566, 320);
            this.Load += new System.EventHandler(this.Scrollback_Load);
            this.Resize += new System.EventHandler(this.ResizeWebs);
            this.contextMenuStrip1.ResumeLayout(false);
            this.pwx1.ResumeLayout(false);
            this.pwx2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		
        #endregion

        private System.Windows.Forms.Timer refresh;
        private System.Windows.Forms.ToolStripMenuItem mrhToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrollToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem channelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.Panel pwx1;
        private System.Windows.Forms.Panel pwx2;
        private System.Windows.Forms.ToolStripMenuItem retrieveTopicToolStripMenuItem;
        public System.Windows.Forms.WebBrowser Data;
        public System.Windows.Forms.WebBrowser webBrowser1;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TextBox simpleview;
        private System.Windows.Forms.ToolStripMenuItem toggleSimpleLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleAdvancedLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLinkInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mode1b2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mode1q2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mode1e2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mode1I2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem joinToolStripMenuItem;

    }
}
