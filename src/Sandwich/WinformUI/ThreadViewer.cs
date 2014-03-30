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
    public partial class ThreadViewer : Form
    {
      //  private int _tid;
      //  private string _board;
       // private ReplyForm replier; 

       // ThreadContainer tc;

       // private bool has_404 = false;

        public ThreadViewer(int id, string board)
        {
            InitializeComponent();
            //_tid = id;
            //_board = board;
            //replier = new ReplyForm(1, id, board);
            //((ThreadViewerWPF)this.elementHost1.Child).Background = new System.Windows.Media.SolidColorBrush(WPFHelpers.DColorToMColor(BoardInfo.get_board_bgcolor(board)));
            //this.Icon = BoardInfo.get_board_def_icon(board);
        }

        //public int ThreadId { get { return _tid; } }

        //bool is_refreshing = false;

        //private void ThreadViewer_Shown(object sender, EventArgs e)
        //{
        //    refresh();
        //}

        //void refresh() 
        //{
        //    if (!(is_refreshing | has_404))
        //    {
        //        is_refreshing = true;
        //        backgroundWorker1.RunWorkerAsync();
        //        update_in.Text = "Refreshing thread ...";
        //    }
        //}

        //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        tc = Core.GetThreadData(_board, _tid);
        //    }
        //    catch (Exception ex) 
        //    {
        //        if (ex.Message == "404")
        //        {
        //            _404();
        //        }
        //        else 
        //        {
        //            throw ex;
        //        }
        //    }
        //}

        //private void _404() 
        //{
        //    MessageBox.Show("This thread has 404'ed");
        //    has_404 = true; //Never allow new updates
        //    timer1.Stop();
        //    update_in.Text = "404'ed";
        //    toolStripButton1.Enabled = false;
        //    toolStripButton2.Enabled = false;
        //}

        //private void AddPost(GenericPost gp) 
        //{
        //    PostDisplayerWPF pw = new PostDisplayerWPF(gp);
            
        //    pw.ImageClicked += new Common.ImageClickEvent(gpd_ImageClicked);
        //    pw.PostTitleClicked += new Common.PostTitleClickEvent(gpd_PostTitleClicked);
        //    ((ThreadViewerWPF)this.elementHost1.Child).AddPost(pw);

        //}

        //void gpd_PostTitleClicked(GenericPost gp)
        //{
        //    if (replier != null)
        //    {
        //        replier.com_f.Text = replier.com_f.Text + ">>" + gp.PostNumber.ToString() + Environment.NewLine;
        //        replier.Show();
        //        replier.Activate();
        //    }
        //}

        //void gpd_ImageClicked(GenericPost gp, bool autofocus)
        //{
        //     foreach (Form f in this.MdiParent.MdiChildren)
        //        {
        //            if (f.GetType() == typeof(FullImageViewer))
        //            {
        //                if ((int)f.Tag == gp.PostNumber)
        //                {
        //                    return;
        //                }
        //            }
        //        }
        //    FullImageViewer fiv = new FullImageViewer(gp);
        //    fiv.Tag = gp.PostNumber;
        //    fiv.MdiParent = this.MdiParent;
        //    if (autofocus)
        //    {
        //        fiv.Show();
        //    }
        //    else 
        //    {
        //        fiv.Show();
        //        this.Activate();
        //    }
        //}

        //private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (!has_404)
        //    {
        //        handle_tc();
        //        is_refreshing = false;
        //    }
        //}

        //void handle_tc() 
        //{
        //    AddPost(tc.Instance);
        //    foreach (Reply a in tc.Replies) 
        //    {
        //        AddPost(a);
        //    }
        //    this.Text = tc.Title;
        //}

        //private void toolStripButton1_Click(object sender, EventArgs e)
        //{
        //    refresh();
        //}

        //int update_int = 180;

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    if (!is_refreshing) 
        //    {
        //        update_int -= 1;
                
        //        if (update_int <= 0) 
        //        {
        //            refresh();
        //            update_int = 180;
        //        }

        //        update_in.Text = "Updating in " + update_int.ToString() + " sec";
        //    }
        //}

        
        //private void toolStripButton2_Click(object sender, EventArgs e)
        //{
        //    replier.Show();
        //}

        //private void toolStripButton3_Click(object sender, EventArgs e)
        //{
        //    ((ThreadViewerWPF)this.elementHost1.Child).FocusTop();
        //}

        //private void toolStripButton4_Click(object sender, EventArgs e)
        //{
        //    ((ThreadViewerWPF)this.elementHost1.Child).FocusBottom();
        //}

        //private void dumpImageListToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog sf = new SaveFileDialog();
        //    sf.Filter = "Text files|*.txt";
        //    sf.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //    sf.OverwritePrompt = true;
        //    sf.FileName = this.tc.Instance.PostNumber.ToString() + ".txt";
        //    if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
        //    {
        //        List<string> il = new List<string>();

        //        if (tc.Instance.file != null)
        //        {
        //            il.Add(tc.Instance.file.FullImageLink);
        //        }
        //        foreach (Reply gp in tc.Replies)
        //        {
        //            if (gp.file != null)
        //            {
        //                il.Add(gp.file.FullImageLink);
        //            }
        //        }
        //        System.IO.File.WriteAllLines(sf.FileName, il.ToArray());
        //    }
        //}

        //private void monitorThreadToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("Not implemented yet", "Info");
        //}

        //private void ThreadViewer_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (replier != null)
        //    {
        //        replier.Close();
        //    }
        //}

        //private void countFilesTotalSizeToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    int g = 0;

        //    if (tc.Instance.file != null)
        //        {
        //            g += tc.Instance.file.size;
        //        }
        //        foreach (Reply gp in tc.Replies)
        //        {
        //            if (gp.file != null)
        //            {
        //                g += gp.file.size;
        //            }
        //        }
        //        MessageBox.Show(Common.format_size_string(g), "Total size");
        //}

    }
}
