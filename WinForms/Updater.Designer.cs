//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.


namespace Client
{
    partial class Updater
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
            this.lStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.update = new System.Windows.Forms.Button();
            this.lUpdateLink = new System.Windows.Forms.LinkLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lStatus
            // 
            this.lStatus.Location = new System.Drawing.Point(28, 19);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(550, 166);
            this.lStatus.TabIndex = 0;
            this.lStatus.Text = "label1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(31, 257);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(547, 35);
            this.progressBar1.TabIndex = 1;
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(448, 206);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(130, 32);
            this.update.TabIndex = 2;
            this.update.Text = "button1";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.update_Click);
            this.lUpdateLink.Location = new System.Drawing.Point(20, 180);
            this.lUpdateLink.Visible = false;
            this.lUpdateLink.AutoSize = true;
            this.lUpdateLink.Click += new System.EventHandler(this.UpdateLink_Click);
            this.lUpdateLink.Text = "update";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Updater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 323);
            this.Controls.Add(this.update);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lUpdateLink);
            this.Controls.Add(this.lStatus);
            this.Name = "Updater";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updater";
            this.Load += new System.EventHandler(this.Updater_Load);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// Label that contains the status info
        /// </summary>
        public System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.LinkLabel lUpdateLink;
        /// <summary>
        /// Update button
        /// </summary>
        public System.Windows.Forms.Button update;
        private System.Windows.Forms.Timer timer1;

    }
}
