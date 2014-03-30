namespace Sandwich
{
    partial class GenericBrowser
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
            this.MdiTabStrip1 = new MdiTabStrip.MdiTabStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.MdiTabStrip1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MdiTabStrip1
            // 
            this.MdiTabStrip1.ActiveTabFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MdiTabStrip1.AllowDrop = true;
            this.MdiTabStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.MdiTabStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MdiTabStrip1.InactiveTabFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MdiTabStrip1.Location = new System.Drawing.Point(0, 0);
            this.MdiTabStrip1.MdiNewTabImage = null;
            this.MdiTabStrip1.MdiNewTabVisible = true;
            this.MdiTabStrip1.MdiNewTabWidth = 30;
            this.MdiTabStrip1.MdiWindowState = MdiTabStrip.MdiChildWindowState.Maximized;
            this.MdiTabStrip1.MinimumSize = new System.Drawing.Size(50, 33);
            this.MdiTabStrip1.MouseOverTabFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MdiTabStrip1.Name = "MdiTabStrip1";
            this.MdiTabStrip1.NewTabToolTipText = "Catalog";
            this.MdiTabStrip1.Padding = new System.Windows.Forms.Padding(5, 3, 20, 5);
            this.MdiTabStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MdiTabStrip1.Size = new System.Drawing.Size(969, 37);
            this.MdiTabStrip1.TabIndex = 1;
            this.MdiTabStrip1.TabPermanence = MdiTabStrip.MdiTabPermanence.First;
            this.MdiTabStrip1.Text = "MdiTabStrip1";
            this.MdiTabStrip1.MdiTabClicked += new MdiTabStrip.MdiTabStrip.MdiTabClickedEventHandler(this.MdiTabStrip1_MdiTabClicked);
            this.MdiTabStrip1.MdiNewTabClicked += new MdiTabStrip.MdiTabStrip.MdiNewTabClickedEventHandler(this.MdiTabStrip1_MdiNewTabClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.MdiTabStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(969, 37);
            this.panel1.TabIndex = 2;
            // 
            // GenericBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 657);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IsMdiContainer = true;
            this.Name = "GenericBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GenericBrowser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GenericBrowser_FormClosing);
            this.Shown += new System.EventHandler(this.GenericBrowser_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.MdiTabStrip1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal MdiTabStrip.MdiTabStrip MdiTabStrip1;
        private System.Windows.Forms.Panel panel1;
    }
}