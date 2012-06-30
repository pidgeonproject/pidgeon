namespace Client
{
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
            this.@__LP = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // __LP
            // 
            this.@__LP.ColumnCount = 1;
            this.@__LP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.@__LP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.@__LP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.@__LP.Location = new System.Drawing.Point(0, 0);
            this.@__LP.Name = "__LP";
            this.@__LP.RowCount = 2;
            this.@__LP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.60104F));
            this.@__LP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.39896F));
            this.@__LP.Size = new System.Drawing.Size(766, 386);
            this.@__LP.TabIndex = 2;
            this.@__LP.Paint += new System.Windows.Forms.PaintEventHandler(this.@__LP_Paint);
            // 
            // Recovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 386);
            this.Controls.Add(this.@__LP);
            this.Name = "Recovery";
            this.Text = "Recovery";
            this.Load += new System.EventHandler(this.Recovery_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel __LP;
    }
}