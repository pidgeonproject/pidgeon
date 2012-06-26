namespace Client
{
    partial class ChannelList
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
            this.items = new System.Windows.Forms.TreeView();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // items
            // 
            this.items.Location = new System.Drawing.Point(0, 0);
            this.items.Name = "items";
            this.items.Size = new System.Drawing.Size(182, 321);
            this.items.TabIndex = 0;
            this.items.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.items_AfterSelect);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // ChannelList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.items);
            this.Name = "ChannelList";
            this.Size = new System.Drawing.Size(182, 321);
            this.Resize += new System.EventHandler(Display);
            this.Load += new System.EventHandler(this.ChannelList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView items;
        private System.Windows.Forms.Timer timer2;



    }
}
