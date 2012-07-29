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
    partial class SBABox
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
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.ScrollBar = new System.Windows.Forms.HScrollBar();
            this.pt = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pt)).BeginInit();
            this.SuspendLayout();
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(540, 0);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(23, 339);
            this.vScrollBar1.TabIndex = 0;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // ScrollBar
            // 
            this.ScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ScrollBar.LargeChange = 1;
            this.ScrollBar.Location = new System.Drawing.Point(0, 315);
            this.ScrollBar.Maximum = 0;
            this.ScrollBar.Name = "ScrollBar";
            this.ScrollBar.Size = new System.Drawing.Size(540, 24);
            this.ScrollBar.TabIndex = 3;
            this.ScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar_Scroll);
            // 
            // pt
            // 
            this.pt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pt.Location = new System.Drawing.Point(0, 0);
            this.pt.Name = "pt";
            this.pt.Size = new System.Drawing.Size(540, 315);
            this.pt.MouseDown += new System.Windows.Forms.MouseEventHandler(ClickHandler);
            this.pt.TabIndex = 1;
            this.pt.TabStop = false;
            this.pt.Paint += new System.Windows.Forms.PaintEventHandler(this.RepaintWindow);
            // 
            // SBABox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pt);
            this.Controls.Add(this.ScrollBar);
            this.Controls.Add(this.vScrollBar1);
            this.DoubleBuffered = true;
            this.Name = "SBABox";
            this.Size = new System.Drawing.Size(563, 339);
            this.Load += new System.EventHandler(this.SBABox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.HScrollBar ScrollBar;
        private System.Windows.Forms.PictureBox pt;
    }
}
