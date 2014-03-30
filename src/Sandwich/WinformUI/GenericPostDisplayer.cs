using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sandwich
{
    public partial class GenericPostDisplayer : UserControl
    {

        public GenericPost _gp;


        public GenericPostDisplayer(GenericPost gp)
        {
            InitializeComponent();

            /*if (gp.GetType() == typeof(Thread)) 
            {
                this.Dock = DockStyle.Top;
            }

            if (gp.GetType() == typeof(Reply))
            {
                this.Dock = DockStyle.Left;
            }*/

            if (gp.file != null)
            {
                gp.file.FileLoaded += new PostFile.ImageLoadedEvent(handle_image_loaded);
                gp.file.BeginThumbnailLoading();

                file_name.Text = gp.file.filename + "." + gp.file.ext;
                file_info.Text = Common.format_size_string(gp.file.size) + Environment.NewLine + gp.file.width.ToString() + "x" + gp.file.height.ToString(); 
            }
            else 
            {
                pictureBox1.Visible = false;
                splitContainer1.Panel1Collapsed = true;
            }
            poster_name.Text = gp.name;

            post_no.Text = "No. " + gp.PostNumber.ToString();

            post_date.Text = gp.time.ToString();

            this.htmlLabel1.Text = (gp.comment);

            this.BackColor = BoardInfo.get_board_post_color(gp.board);

            _gp = gp;

            this.Tag = gp.PostNumber;

            toolStrip1.BackColor = this.BackColor;
            toolStrip2.BackColor = this.BackColor;

            splitContainer1.Panel2Collapsed = String.IsNullOrEmpty(gp.comment);

            subject_label.Text = gp.subject;
            trip_label.Text = gp.trip;
        }

        private void handle_image_loaded(PostFile f) 
        {
            this.pictureBox1.Image = f.Image;
        }

        public event Common.QuoteClickEvent QuoteClicked;

        public event Common.PostTitleClickEvent PostTitleClicked;

        public event Common.ImageClickEvent ImageClicked;

        private void post_no_Click(object sender, EventArgs e)
        {
            PostTitleClicked(_gp);
        }

        private void post_no_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void post_no_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;/* timer1.Start();*/
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
           /* timer1.Stop();
            panel1.Hide();*/
            this.Cursor = Cursors.Arrow;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*panel1.Show();
            timer1.Stop();*/
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button) 
            {
                case System.Windows.Forms.MouseButtons.Middle:
                    ImageClicked(_gp, false);
                    break;
                case System.Windows.Forms.MouseButtons.Left:
                    ImageClicked(_gp, true);
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    image_menu.ShowDropDown();
                    image_menu.DropDown.Focus();
                    break;
                default:
                    break;
            }
        }



 
    }
}
