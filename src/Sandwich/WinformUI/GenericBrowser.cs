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
    public partial class GenericBrowser : Form
    {
        private string _board = "";

        IndexBrowser ib;

        BoardHomePage bhp;

        public string Board
        {
            get { return _board; }
        }

        public GenericBrowser(string board)
        {
            InitializeComponent();
            _board = board;
            this.Icon = BoardInfo.get_board_def_icon(board);
        }

        private void GenericBrowser_Shown(object sender, EventArgs e)
        {
            this.Text = _board;
            OpenMainPage();
        }

        public void OpenCatalog()
        {
           
        }

        public void OpenIndex()
        {
            if (ib != null)
            {
                ib.Show();
                ib.Activate();
            }
            else
            {
                ib = new IndexBrowser(_board);
                ib.MdiParent = this;
                ib.Show();
            }
        }

        private void MdiTabStrip1_MdiNewTabClicked(object sender, EventArgs e)
        {
            OpenCatalog();
        }

        public void OpenMainPage() 
        {
            if (bhp != null)
            {
                bhp.Activate();
            }
            else 
            {
                bhp = new BoardHomePage(_board);
                bhp.MdiParent = this;
                bhp.Show();
            }
        }

        public void OpenThread(int id)
        {
            //for (int i = 0; i < this.MdiChildren.Length; i++)
            //{
            //    if (this.MdiChildren.Length <= i) { break; }

            //    if (this.MdiChildren[i].GetType() == typeof(ThreadViewer)) 
            //    {
            //        ThreadViewer tvi = (ThreadViewer)(this.MdiChildren[i]);
            //        if (tvi.ThreadId == id)
            //        {
            //            tvi.Activate();
            //            return;
            //        }
            //    }
            //}

            //ThreadViewer tv = new ThreadViewer(id, _board);
            //tv.Text = id.ToString();
            //tv.MdiParent = this;
            //tv.Show();
            //tv.Activate();
        }

        private void GenericBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < this.MdiChildren.Length; i++ )
            {
                if (this.MdiChildren.Length <= i) { break; }
                this.MdiChildren[i].Close();
            }
            GC.Collect();
        }

        private void MdiTabStrip1_MdiTabClicked(object sender, MdiTabStrip.MdiTabStripTabClickedEventArgs e)
        {
            this.Text = MdiTabStrip1.ActiveTab.Form.Text;
        }
    }
}
