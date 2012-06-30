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
    partial class PidgeonList
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
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.items = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.joinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.partToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.versionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // items
            // 
            this.items.ContextMenuStrip = this.contextMenuStrip1;
            this.items.Location = new System.Drawing.Point(0, 0);
            this.items.Name = "items";
            this.items.Size = new System.Drawing.Size(182, 321);
            this.items.TabIndex = 0;
            this.items.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.items_AfterSelect);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.joinToolStripMenuItem,
            this.partToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.versionToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(134, 114);
            // 
            // joinToolStripMenuItem
            // 
            this.joinToolStripMenuItem.Name = "joinToolStripMenuItem";
            this.joinToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.joinToolStripMenuItem.Text = "Join";
            // 
            // partToolStripMenuItem
            // 
            this.partToolStripMenuItem.Name = "partToolStripMenuItem";
            this.partToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.partToolStripMenuItem.Text = "Part";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            // 
            // versionToolStripMenuItem
            // 
            this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
            this.versionToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.versionToolStripMenuItem.Text = "Version";
            // 
            // PidgeonList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.items);
            this.Name = "PidgeonList";
            this.Size = new System.Drawing.Size(182, 321);
            this.Load += new System.EventHandler(this.ChannelList_Load);
            this.Resize += new System.EventHandler(this._Display);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.TreeView items;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem joinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem partToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem versionToolStripMenuItem;



    }
}
