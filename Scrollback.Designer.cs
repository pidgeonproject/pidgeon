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
            this.mrhToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refresh = new System.Windows.Forms.Timer(this.components);
            this.pwx1 = new System.Windows.Forms.Panel();
            this.Data = new System.Windows.Forms.WebBrowser();
            this.pwx2 = new System.Windows.Forms.Panel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.contextMenuStrip1.SuspendLayout();
            this.pwx1.SuspendLayout();
            this.pwx2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mrhToolStripMenuItem,
            this.scrollToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.channelToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(119, 114);
            // 
            // mrhToolStripMenuItem
            // 
            this.mrhToolStripMenuItem.Name = "mrhToolStripMenuItem";
            this.mrhToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.mrhToolStripMenuItem.Text = "Clean";
            this.mrhToolStripMenuItem.Click += new System.EventHandler(this.mrhToolStripMenuItem_Click);
            // 
            // scrollToolStripMenuItem
            // 
            this.scrollToolStripMenuItem.Name = "scrollToolStripMenuItem";
            this.scrollToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.scrollToolStripMenuItem.Text = "Scroll";
            this.scrollToolStripMenuItem.Click += new System.EventHandler(this.scrollToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Visible = false;
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // channelToolStripMenuItem
            // 
            this.channelToolStripMenuItem.Name = "channelToolStripMenuItem";
            this.channelToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.channelToolStripMenuItem.Text = "Channel";
            this.channelToolStripMenuItem.Click += new System.EventHandler(this.channelToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // refresh
            // 
            this.refresh.Interval = 200;
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
            // 
            // Scrollback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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

        }
		
        #endregion

        private System.Windows.Forms.Timer refresh;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mrhToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrollToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem channelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.Panel pwx1;
        private System.Windows.Forms.WebBrowser Data;
        private System.Windows.Forms.Panel pwx2;
        private System.Windows.Forms.WebBrowser webBrowser1;

    }
}
