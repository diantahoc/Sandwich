using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sandwich
{
    public partial class IndexBrowser : Form
    {
        private string board;

        BackgroundWorker bg;
        ThreadContainer[] data;

        private int current_page = 0;

        public IndexBrowser(string _board)
        {
            InitializeComponent();
            board = _board;
            this.Icon = BoardInfo.get_board_def_icon(_board);
            bg = new BackgroundWorker();
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            this.Text = String.Format("Page {0} - {1}", current_page + 1, BoardInfo.GetBoardTitle(_board)); 
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            data = Core.GetPage(this.board, this.current_page);
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (data != null)
            {
                handle_new_data(data);
            }
            else 
            {
                MessageBox.Show("Cannot refresh index, data is null", "Error");
            }
            progress_bar.Visible = false;
        }

        private void handle_new_data(ThreadContainer[] data) 
        {
            foreach (ThreadContainer ci in data)
            {

                FlowLayoutPanel pl = new FlowLayoutPanel();
                pl.AutoScroll = true;
                pl.AutoSize = true;
                pl.FlowDirection = FlowDirection.TopDown;
                pl.WrapContents = false;
                pl.Dock = DockStyle.Fill;

                pl.Controls.Add(get_gp(ci.Instance));

                if (ci.Replies != null)
                {
                    foreach (Reply a in ci.Replies)
                    {
                        pl.Controls.Add(get_gp(a));
                    }
                }

                flowLayoutPanel1.Controls.Add(pl);

            }
            this.Text = String.Format("Page {0} - {1}", current_page + 1, BoardInfo.GetBoardTitle(board)); 
        }

        private GenericPostDisplayer get_gp(GenericPost gp) 
        {
            GenericPostDisplayer gpgp = new GenericPostDisplayer(gp);

            gpgp.ImageClicked += new Common.ImageClickEvent(gpgp_ImageClicked);
            gpgp.QuoteClicked += new Common.QuoteClickEvent(gpgp_QuoteClicked);
            gpgp.PostTitleClicked += new Common.PostTitleClickEvent(gpgp_PostTitleClicked);

            return gpgp;
         }


        void gpgp_PostTitleClicked(GenericPost gp)
        {
           //open thread
            try
            {
                if (this.MdiParent != null)
                {
                    if (this.MdiParent.GetType() == typeof(GenericBrowser))
                    {
                        GenericBrowser a = (GenericBrowser)(this.MdiParent);
                        a.OpenThread(gp.PostNumber);
                    }
                }
            }
            catch (Exception){ }
        }

        void gpgp_QuoteClicked(int quote)
        {
            return;
        }

        void gpgp_ImageClicked(GenericPost gp, bool autofocus)
        {
           //open image
            foreach (Form f in this.MdiParent.MdiChildren)
            {
                if (f.GetType() == typeof(FullImageViewer))
                {
                    if ((int)f.Tag == gp.PostNumber)
                    {
                        return;
                    }
                }
            }
            FullImageViewer fiv = new FullImageViewer(gp);
            fiv.Tag = gp.PostNumber;
            fiv.MdiParent = this.MdiParent;
            if (autofocus)
            {
                fiv.Show();
            }
            else
            {
                fiv.Show();
                this.Activate();
            }
        }

        private void load_page(int a) 
        {
            if (!bg.IsBusy) 
            {
                current_page = a;
                bg.RunWorkerAsync();
                progress_bar.Visible = true;
            }
        }

        private void IndexBrowser_Shown(object sender, EventArgs e)
        {
            load_page(current_page);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.flowLayoutPanel1.Controls.Count >= 1) { flowLayoutPanel1.Controls[0].Focus(); }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.flowLayoutPanel1.Controls.Count >= 1) { this.flowLayoutPanel1.Controls[this.flowLayoutPanel1.Controls.Count - 1].Focus(); }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            load_page(current_page);
        }
    }
}
