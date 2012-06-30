namespace Client.Stage
{
    partial class Channel_Window
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
            this.tableLayout1 = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tableLayout1
            // 
            this.tableLayout1.ColumnCount = 1;
            this.tableLayout1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout1.Location = new System.Drawing.Point(0, 0);
            this.tableLayout1.Name = "tableLayout1";
            this.tableLayout1.RowCount = 2;
            this.tableLayout1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.68481F));
            this.tableLayout1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.31519F));
            this.tableLayout1.Size = new System.Drawing.Size(758, 349);
            this.tableLayout1.TabIndex = 0;
            // 
            // Channel_Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 349);
            this.Controls.Add(this.tableLayout1);
            this.Name = "Channel_Window";
            this.Text = "ChannelWindow";
            this.Load += new System.EventHandler(this.Channel_Window_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout1;

    }
}