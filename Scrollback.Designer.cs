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
            this.Data = new System.Windows.Forms.WebBrowser();
            this.refresh = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // Data
            // 
            this.Data.Location = new System.Drawing.Point(0, 0);
            this.Data.MinimumSize = new System.Drawing.Size(20, 20);
            this.Data.Name = "Data";
            this.Data.ScriptErrorsSuppressed = true;
            this.Data.Size = new System.Drawing.Size(346, 263);
            this.Data.TabIndex = 1;
            this.Data.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Data_DocumentCompleted);
            // 
            // refresh
            // 
            this.refresh.Enabled = true;
            this.refresh.Tick += new System.EventHandler(this.refresh_Tick);
            // 
            // Scrollback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.Data);
            this.Name = "Scrollback";
            this.Size = new System.Drawing.Size(345, 262);
            this.Load += new System.EventHandler(this.Scrollback_Load);
            this.Resize += new System.EventHandler(this.Recreate);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser Data;
        private System.Windows.Forms.Timer refresh;

    }
}
