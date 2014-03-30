namespace Sandwich
{
    partial class GenericPostDisplayer
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.post_icon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.subject_label = new System.Windows.Forms.ToolStripLabel();
            this.poster_name = new System.Windows.Forms.ToolStripLabel();
            this.trip_label = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.post_date = new System.Windows.Forms.ToolStripLabel();
            this.post_no = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.option_menu = new System.Windows.Forms.ToolStripDropDownButton();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byTripcodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byEmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bySubjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.reportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.file_name = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.file_info = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.image_menu = new System.Windows.Forms.ToolStripSplitButton();
            this.filterImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byImageHashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sauceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tinEyeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iQDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.htmlLabel1 = new HtmlRenderer.HtmlPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.post_icon,
            this.toolStripSeparator1,
            this.subject_label,
            this.poster_name,
            this.trip_label,
            this.toolStripSeparator5,
            this.post_date,
            this.post_no,
            this.toolStripSeparator2,
            this.option_menu});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(700, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // post_icon
            // 
            this.post_icon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.post_icon.Image = global::Sandwich.Properties.Resources.window;
            this.post_icon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.post_icon.Name = "post_icon";
            this.post_icon.Size = new System.Drawing.Size(23, 22);
            this.post_icon.Text = "toolStripButton1";
            this.post_icon.ToolTipText = "Post Icon";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // subject_label
            // 
            this.subject_label.ForeColor = System.Drawing.Color.Blue;
            this.subject_label.Name = "subject_label";
            this.subject_label.Size = new System.Drawing.Size(42, 22);
            this.subject_label.Text = "subject";
            // 
            // poster_name
            // 
            this.poster_name.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.poster_name.Name = "poster_name";
            this.poster_name.Size = new System.Drawing.Size(63, 22);
            this.poster_name.Text = "Anonymous";
            // 
            // trip_label
            // 
            this.trip_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(0)))));
            this.trip_label.Name = "trip_label";
            this.trip_label.Size = new System.Drawing.Size(27, 22);
            this.trip_label.Text = "!trip";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // post_date
            // 
            this.post_date.Name = "post_date";
            this.post_date.Size = new System.Drawing.Size(56, 22);
            this.post_date.Text = "Yesterday";
            // 
            // post_no
            // 
            this.post_no.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.post_no.Name = "post_no";
            this.post_no.Size = new System.Drawing.Size(78, 22);
            this.post_no.Text = "toolStripLabel2";
            this.post_no.Click += new System.EventHandler(this.post_no_Click);
            this.post_no.MouseEnter += new System.EventHandler(this.post_no_MouseEnter);
            this.post_no.MouseLeave += new System.EventHandler(this.post_no_MouseLeave);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // option_menu
            // 
            this.option_menu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.option_menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterToolStripMenuItem,
            this.toolStripSeparator6,
            this.reportToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.option_menu.Image = global::Sandwich.Properties.Resources.process;
            this.option_menu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.option_menu.Name = "option_menu";
            this.option_menu.Size = new System.Drawing.Size(29, 22);
            this.option_menu.Text = "toolStripDropDownButton1";
            this.option_menu.ToolTipText = "Menu";
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.byNameToolStripMenuItem,
            this.byTripcodeToolStripMenuItem,
            this.byEmailToolStripMenuItem,
            this.bySubjectToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // byNameToolStripMenuItem
            // 
            this.byNameToolStripMenuItem.Name = "byNameToolStripMenuItem";
            this.byNameToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.byNameToolStripMenuItem.Text = "By name";
            // 
            // byTripcodeToolStripMenuItem
            // 
            this.byTripcodeToolStripMenuItem.Name = "byTripcodeToolStripMenuItem";
            this.byTripcodeToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.byTripcodeToolStripMenuItem.Text = "By tripcode";
            // 
            // byEmailToolStripMenuItem
            // 
            this.byEmailToolStripMenuItem.Name = "byEmailToolStripMenuItem";
            this.byEmailToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.byEmailToolStripMenuItem.Text = "By email";
            // 
            // bySubjectToolStripMenuItem
            // 
            this.bySubjectToolStripMenuItem.Name = "bySubjectToolStripMenuItem";
            this.bySubjectToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.bySubjectToolStripMenuItem.Text = "By subject";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(104, 6);
            // 
            // reportToolStripMenuItem
            // 
            this.reportToolStripMenuItem.Name = "reportToolStripMenuItem";
            this.reportToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.reportToolStripMenuItem.Text = "Report";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip2);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.htmlLabel1);
            this.splitContainer1.Panel2MinSize = 300;
            this.splitContainer1.Size = new System.Drawing.Size(700, 220);
            this.splitContainer1.SplitterDistance = 233;
            this.splitContainer1.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Sandwich.Properties.Resources.progressindicator;
            this.pictureBox1.Location = new System.Drawing.Point(0, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(233, 195);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file_name,
            this.toolStripSeparator3,
            this.file_info,
            this.toolStripSeparator4,
            this.image_menu});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(233, 25);
            this.toolStrip2.TabIndex = 7;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // file_name
            // 
            this.file_name.Name = "file_name";
            this.file_name.Size = new System.Drawing.Size(78, 22);
            this.file_name.Text = "toolStripLabel2";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // file_info
            // 
            this.file_info.Name = "file_info";
            this.file_info.Size = new System.Drawing.Size(78, 22);
            this.file_info.Text = "toolStripLabel3";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // image_menu
            // 
            this.image_menu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.image_menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterImageToolStripMenuItem,
            this.sauceToolStripMenuItem});
            this.image_menu.Image = global::Sandwich.Properties.Resources.image;
            this.image_menu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.image_menu.Name = "image_menu";
            this.image_menu.Size = new System.Drawing.Size(32, 22);
            this.image_menu.Text = "toolStripSplitButton1";
            // 
            // filterImageToolStripMenuItem
            // 
            this.filterImageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.byFileNameToolStripMenuItem,
            this.byImageHashToolStripMenuItem});
            this.filterImageToolStripMenuItem.Name = "filterImageToolStripMenuItem";
            this.filterImageToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.filterImageToolStripMenuItem.Text = "Filter Image";
            // 
            // byFileNameToolStripMenuItem
            // 
            this.byFileNameToolStripMenuItem.Name = "byFileNameToolStripMenuItem";
            this.byFileNameToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.byFileNameToolStripMenuItem.Text = "By file name";
            // 
            // byImageHashToolStripMenuItem
            // 
            this.byImageHashToolStripMenuItem.Name = "byImageHashToolStripMenuItem";
            this.byImageHashToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.byImageHashToolStripMenuItem.Text = "By image hash";
            // 
            // sauceToolStripMenuItem
            // 
            this.sauceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.googleToolStripMenuItem,
            this.tinEyeToolStripMenuItem,
            this.iQDBToolStripMenuItem});
            this.sauceToolStripMenuItem.Name = "sauceToolStripMenuItem";
            this.sauceToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.sauceToolStripMenuItem.Text = "Sauce";
            // 
            // googleToolStripMenuItem
            // 
            this.googleToolStripMenuItem.Name = "googleToolStripMenuItem";
            this.googleToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.googleToolStripMenuItem.Text = "Google";
            // 
            // tinEyeToolStripMenuItem
            // 
            this.tinEyeToolStripMenuItem.Name = "tinEyeToolStripMenuItem";
            this.tinEyeToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.tinEyeToolStripMenuItem.Text = "TinEye";
            // 
            // iQDBToolStripMenuItem
            // 
            this.iQDBToolStripMenuItem.Name = "iQDBToolStripMenuItem";
            this.iQDBToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.iQDBToolStripMenuItem.Text = "IQDB";
            // 
            // htmlLabel1
            // 
            this.htmlLabel1.AutoScroll = true;
            this.htmlLabel1.AutoScrollMinSize = new System.Drawing.Size(463, 17);
            this.htmlLabel1.AvoidGeometryAntialias = false;
            this.htmlLabel1.AvoidImagesLateLoading = false;
            this.htmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.htmlLabel1.BaseStylesheet = null;
            this.htmlLabel1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.htmlLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlLabel1.Location = new System.Drawing.Point(0, 0);
            this.htmlLabel1.Name = "htmlLabel1";
            this.htmlLabel1.Size = new System.Drawing.Size(463, 220);
            this.htmlLabel1.TabIndex = 8;
            this.htmlLabel1.Text = "htmlPanel1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // GenericPostDisplayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.MaximumSize = new System.Drawing.Size(700, 245);
            this.Name = "GenericPostDisplayer";
            this.Size = new System.Drawing.Size(700, 245);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripButton post_icon;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel poster_name;
        private System.Windows.Forms.ToolStripLabel trip_label;
        private System.Windows.Forms.ToolStripLabel post_date;
        private System.Windows.Forms.ToolStripLabel post_no;
        private System.Windows.Forms.ToolStripDropDownButton option_menu;
        private System.Windows.Forms.ToolStripMenuItem reportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Timer timer1;
        private HtmlRenderer.HtmlPanel htmlLabel1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel file_name;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel file_info;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSplitButton image_menu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel subject_label;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byTripcodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byEmailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bySubjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byFileNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byImageHashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sauceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem googleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tinEyeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iQDBToolStripMenuItem;

    }
}
