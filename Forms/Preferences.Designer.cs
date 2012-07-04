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
    partial class Preferences
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.grIRC_ = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.lident = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.lquit = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lnick = new System.Windows.Forms.Label();
            this.fl = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.lname = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.grIRC_.SuspendLayout();
            this.fl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grIRC_, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.fl, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.55656F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.44344F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 442);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(194, 380);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // grIRC_
            // 
            this.grIRC_.Controls.Add(this.textBox4);
            this.grIRC_.Controls.Add(this.lname);
            this.grIRC_.Controls.Add(this.textBox3);
            this.grIRC_.Controls.Add(this.lident);
            this.grIRC_.Controls.Add(this.textBox2);
            this.grIRC_.Controls.Add(this.lquit);
            this.grIRC_.Controls.Add(this.textBox1);
            this.grIRC_.Controls.Add(this.lnick);
            this.grIRC_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grIRC_.Location = new System.Drawing.Point(203, 3);
            this.grIRC_.Name = "grIRC_";
            this.grIRC_.Size = new System.Drawing.Size(461, 380);
            this.grIRC_.TabIndex = 4;
            this.grIRC_.TabStop = false;
            this.grIRC_.Text = "IRC";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(95, 101);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(323, 20);
            this.textBox3.TabIndex = 5;
            // 
            // lident
            // 
            this.lident.AutoSize = true;
            this.lident.Location = new System.Drawing.Point(17, 101);
            this.lident.Name = "lident";
            this.lident.Size = new System.Drawing.Size(35, 13);
            this.lident.TabIndex = 4;
            this.lident.Text = "label1";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(95, 62);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(323, 20);
            this.textBox2.TabIndex = 3;
            // 
            // lquit
            // 
            this.lquit.AutoSize = true;
            this.lquit.Location = new System.Drawing.Point(17, 62);
            this.lquit.Name = "lquit";
            this.lquit.Size = new System.Drawing.Size(35, 13);
            this.lquit.TabIndex = 2;
            this.lquit.Text = "label1";
            this.lquit.Click += new System.EventHandler(this.lquit_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(95, 22);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(323, 20);
            this.textBox1.TabIndex = 1;
            // 
            // lnick
            // 
            this.lnick.AutoSize = true;
            this.lnick.Location = new System.Drawing.Point(17, 24);
            this.lnick.Name = "lnick";
            this.lnick.Size = new System.Drawing.Size(35, 13);
            this.lnick.TabIndex = 0;
            this.lnick.Text = "label1";
            // 
            // fl
            // 
            this.fl.Controls.Add(this.bCancel);
            this.fl.Controls.Add(this.bSave);
            this.fl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fl.Location = new System.Drawing.Point(203, 389);
            this.fl.Name = "fl";
            this.fl.Size = new System.Drawing.Size(461, 50);
            this.fl.TabIndex = 5;
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(3, 3);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(114, 29);
            this.bCancel.TabIndex = 0;
            this.bCancel.Text = "button1";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(123, 3);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(137, 29);
            this.bSave.TabIndex = 4;
            this.bSave.Text = "button1";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // lname
            // 
            this.lname.AutoSize = true;
            this.lname.Location = new System.Drawing.Point(17, 141);
            this.lname.Name = "lname";
            this.lname.Size = new System.Drawing.Size(35, 13);
            this.lname.TabIndex = 6;
            this.lname.Text = "label1";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(95, 141);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(323, 20);
            this.textBox4.TabIndex = 7;
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 442);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Preferences";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.grIRC_.ResumeLayout(false);
            this.grIRC_.PerformLayout();
            this.fl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.GroupBox grIRC_;
        private System.Windows.Forms.FlowLayoutPanel fl;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label lquit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lnick;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label lident;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label lname;

    }
}