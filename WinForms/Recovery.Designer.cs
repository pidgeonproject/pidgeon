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
    /// <summary>
    /// This class is responsible for extension handling
    /// </summary>
    partial class Recovery
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
            this.@__LP = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.bShutdown = new System.Windows.Forms.Button();
            this.bContinue = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.@__LP.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // __LP
            // 
            this.@__LP.ColumnCount = 1;
            this.@__LP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.@__LP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.@__LP.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.@__LP.Controls.Add(this.textBox1, 0, 0);
            this.@__LP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.@__LP.Location = new System.Drawing.Point(0, 0);
            this.@__LP.Name = "__LP";
            this.@__LP.RowCount = 2;
            this.@__LP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.60104F));
            this.@__LP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.39896F));
            this.@__LP.Size = new System.Drawing.Size(766, 386);
            this.@__LP.TabIndex = 2;;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.bShutdown);
            this.flowLayoutPanel1.Controls.Add(this.bContinue);
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 345);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(760, 38);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // bShutdown
            // 
            this.bShutdown.Location = new System.Drawing.Point(3, 3);
            this.bShutdown.Name = "bShutdown";
            this.bShutdown.Size = new System.Drawing.Size(109, 26);
            this.bShutdown.TabIndex = 0;
            this.bShutdown.Text = "Shutdown";
            this.bShutdown.UseVisualStyleBackColor = true;
            this.bShutdown.Click += new System.EventHandler(this.bShutdown_Click);
            // 
            // bContinue
            // 
            this.bContinue.Location = new System.Drawing.Point(118, 3);
            this.bContinue.Name = "bContinue";
            this.bContinue.Size = new System.Drawing.Size(107, 26);
            this.bContinue.TabIndex = 1;
            this.bContinue.Text = "Recover";
            this.bContinue.UseVisualStyleBackColor = true;
            this.bContinue.Click += new System.EventHandler(this.bContinue_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(760, 336);
            this.textBox1.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(231, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 26);
            this.button1.TabIndex = 2;
            this.button1.Text = "Terminate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Recovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 386);
            this.ControlBox = false;
            this.Controls.Add(this.@__LP);
            this.Name = "Recovery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recovery";
            this.Load += new System.EventHandler(this.Recovery_Load);
            this.@__LP.ResumeLayout(false);
            this.@__LP.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel __LP;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button bShutdown;
        private System.Windows.Forms.Button bContinue;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button button1;
    }
}
