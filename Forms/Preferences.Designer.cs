﻿/***************************************************************************
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
            this.fl = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gro1 = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.lname = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.lident = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.lquit = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lnick = new System.Windows.Forms.Label();
            this.gro4 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.gro3 = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gro2 = new System.Windows.Forms.GroupBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.fl.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gro1.SuspendLayout();
            this.gro4.SuspendLayout();
            this.gro3.SuspendLayout();
            this.gro2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.fl, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.55656F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.44344F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 442);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(194, 380);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // fl
            // 
            this.fl.Controls.Add(this.bSave);
            this.fl.Controls.Add(this.bCancel);
            this.fl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fl.Location = new System.Drawing.Point(203, 389);
            this.fl.Name = "fl";
            this.fl.Size = new System.Drawing.Size(461, 50);
            this.fl.TabIndex = 5;
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(146, 3);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(114, 29);
            this.bCancel.TabIndex = 0;
            this.bCancel.Text = "button1";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(3, 3);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(137, 29);
            this.bSave.TabIndex = 4;
            this.bSave.Text = "button1";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gro1);
            this.panel1.Controls.Add(this.gro4);
            this.panel1.Controls.Add(this.gro3);
            this.panel1.Controls.Add(this.gro2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(203, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(461, 380);
            this.panel1.TabIndex = 6;
            // 
            // gro1
            // 
            this.gro1.Controls.Add(this.textBox4);
            this.gro1.Controls.Add(this.lname);
            this.gro1.Controls.Add(this.textBox3);
            this.gro1.Controls.Add(this.lident);
            this.gro1.Controls.Add(this.textBox2);
            this.gro1.Controls.Add(this.lquit);
            this.gro1.Controls.Add(this.textBox1);
            this.gro1.Controls.Add(this.lnick);
            this.gro1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gro1.Location = new System.Drawing.Point(0, 0);
            this.gro1.Name = "gro1";
            this.gro1.Size = new System.Drawing.Size(461, 380);
            this.gro1.TabIndex = 0;
            this.gro1.TabStop = false;
            this.gro1.Text = "IRC";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(93, 133);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(323, 20);
            this.textBox4.TabIndex = 15;
            // 
            // lname
            // 
            this.lname.AutoSize = true;
            this.lname.Location = new System.Drawing.Point(6, 133);
            this.lname.Name = "lname";
            this.lname.Size = new System.Drawing.Size(35, 13);
            this.lname.TabIndex = 14;
            this.lname.Text = "label1";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(93, 93);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(323, 20);
            this.textBox3.TabIndex = 13;
            // 
            // lident
            // 
            this.lident.AutoSize = true;
            this.lident.Location = new System.Drawing.Point(6, 93);
            this.lident.Name = "lident";
            this.lident.Size = new System.Drawing.Size(35, 13);
            this.lident.TabIndex = 12;
            this.lident.Text = "label1";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(93, 54);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(323, 20);
            this.textBox2.TabIndex = 11;
            // 
            // lquit
            // 
            this.lquit.AutoSize = true;
            this.lquit.Location = new System.Drawing.Point(6, 54);
            this.lquit.Name = "lquit";
            this.lquit.Size = new System.Drawing.Size(35, 13);
            this.lquit.TabIndex = 10;
            this.lquit.Text = "label1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(93, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(323, 20);
            this.textBox1.TabIndex = 9;
            // 
            // lnick
            // 
            this.lnick.AutoSize = true;
            this.lnick.Location = new System.Drawing.Point(6, 17);
            this.lnick.Name = "lnick";
            this.lnick.Size = new System.Drawing.Size(35, 13);
            this.lnick.TabIndex = 8;
            this.lnick.Text = "label1";
            // 
            // gro4
            // 
            this.gro4.Controls.Add(this.label3);
            this.gro4.Controls.Add(this.checkBox7);
            this.gro4.Controls.Add(this.checkBox6);
            this.gro4.Controls.Add(this.checkBox5);
            this.gro4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gro4.Location = new System.Drawing.Point(0, 0);
            this.gro4.Name = "gro4";
            this.gro4.Size = new System.Drawing.Size(461, 380);
            this.gro4.TabIndex = 3;
            this.gro4.TabStop = false;
            this.gro4.Text = "Protections";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Number";
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(17, 96);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(104, 17);
            this.checkBox7.TabIndex = 2;
            this.checkBox7.Text = "CTCP protection";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(17, 63);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(107, 17);
            this.checkBox6.TabIndex = 1;
            this.checkBox6.Text = "Notice protection";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(17, 31);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(102, 17);
            this.checkBox5.TabIndex = 0;
            this.checkBox5.Text = "Flood protection";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // gro3
            // 
            this.gro3.Controls.Add(this.checkBox3);
            this.gro3.Controls.Add(this.checkBox2);
            this.gro3.Controls.Add(this.checkBox1);
            this.gro3.Controls.Add(this.textBox6);
            this.gro3.Controls.Add(this.label2);
            this.gro3.Controls.Add(this.textBox5);
            this.gro3.Controls.Add(this.label1);
            this.gro3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gro3.Location = new System.Drawing.Point(0, 0);
            this.gro3.Name = "gro3";
            this.gro3.Size = new System.Drawing.Size(461, 380);
            this.gro3.TabIndex = 2;
            this.gro3.TabStop = false;
            this.gro3.Text = "Logs";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(21, 184);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(70, 17);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.Text = "XML logs";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(21, 149);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(78, 17);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.Text = "HTML logs";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(21, 113);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(69, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "TXT logs";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(97, 67);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(354, 20);
            this.textBox6.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(97, 32);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(354, 20);
            this.textBox5.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File name";
            // 
            // gro2
            // 
            this.gro2.Controls.Add(this.checkBox4);
            this.gro2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gro2.Location = new System.Drawing.Point(0, 0);
            this.gro2.Name = "gro2";
            this.gro2.Size = new System.Drawing.Size(461, 380);
            this.gro2.TabIndex = 1;
            this.gro2.TabStop = false;
            this.gro2.Text = "System";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(22, 33);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(91, 17);
            this.checkBox4.TabIndex = 0;
            this.checkBox4.Text = "Display CTCP";
            this.checkBox4.UseVisualStyleBackColor = true;
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
            this.fl.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.gro1.ResumeLayout(false);
            this.gro1.PerformLayout();
            this.gro4.ResumeLayout(false);
            this.gro4.PerformLayout();
            this.gro3.ResumeLayout(false);
            this.gro3.PerformLayout();
            this.gro2.ResumeLayout(false);
            this.gro2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.FlowLayoutPanel fl;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox gro1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label lname;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label lident;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label lquit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lnick;
        private System.Windows.Forms.GroupBox gro2;
        private System.Windows.Forms.GroupBox gro3;
        private System.Windows.Forms.GroupBox gro4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.Label label3;

    }
}