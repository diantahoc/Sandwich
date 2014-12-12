using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

using Sandwich.WPF;
using Sandwich.ArchiveExtensions;

namespace Sandwich
{
    /// <summary>
    /// Interaction logic for ThreadViewerWPF.xaml
    /// </summary>
    public partial class ThreadViewerWPF : UserControl, TabElement
    {

        #region TabElement Interface
        public BoardInfo Board { get; private set; }

        public int ID { get; private set; }

        public BoardBrowserWPF TParent { get; private set; }

        public Common.ElementType Type { get { return Common.ElementType.Thread; } }

        public string Title { get; private set; }
        #endregion

        private ThreadContainer tc;

        /// <summary>
        ///  post id, [stackpanel index, item height]
        /// </summary>
        private Dictionary<int, int> dic = new Dictionary<int, int>();

        BackgroundWorker bg;
        System.Windows.Forms.Timer timer;

        private int update_in = 180;
        private bool is_404 = false;
        private bool dead_thread_loaded = false;

        public ThreadViewerWPF(int tid, BoardInfo board, BoardBrowserWPF parent)
        {
            InitializeComponent();

            //set up interface
            this.Board = board;
            this.ID = tid;
            this.TParent = parent;
            this.Title = String.Format("Thread No. {0}", tid);


            SessionManager.RegisterThread(this.TParent.Chan, board, tid);


            bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            bg.WorkerSupportsCancellation = true;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; //1 sec
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            refresh();

            this.Background = ElementsColors.ThreadBGColor;
        }

        #region ThreadUpdater And Fetcher

        void timer_Tick(object sender, EventArgs e)
        {
            if (!bg.IsBusy)
            {
                update_in--;
                statusbar.Content = String.Format("Updating in {0} seconds", update_in);
                if (update_in <= 0)
                {
                    refresh();
                    update_in = 180;
                }
            }
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }
            else
            {
                if (is_404)
                {
                    if (dead_thread_loaded)
                    {
                        handle_tc();
                        statusbar.Content = "Dead thread loaded";
                        updatebtn.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        _404();
                    }
                }
                else
                {
                    handle_tc();
                }
            }
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            if (is_404 && TParent.Chan.HasArchives)
            {
                try
                {
                    tc = ArchiveExtensions.ArchiveDataProvider.GetThreadData(this.Board.Letter, this.ID);
                    dead_thread_loaded = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Cannot load thread from archive: {0}", ex.Message));
                    e.Cancel = true;
                }
            }
            else
            {
                try
                {
                    tc = TParent.Chan.GetThread(this.Board.Letter, this.ID);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "404")
                    {
                        is_404 = true;
                        return;
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Cannot refresh thread '{0}': {1}", this.ID, ex.Message), "Error");
                        e.Cancel = true;
                    }
                }
            }
        }

        private void refresh()
        {
            if (!bg.IsBusy)
            {
                if (is_404)
                {
                    if (TParent.Chan.HasArchives)
                    {
                        this.statusbar.Content = "Loading from archive...";
                    }
                    else
                    {
                        this.statusbar.Content = "No archive found. Cannot update thread";
                        return;
                    }
                }
                else
                {
                    statusbar.Content = "Refreshing thread...";
                }

                bg.RunWorkerAsync();
            }
        }

        private void handle_tc()
        {
            FormatAndAddPost(tc.Instance);
            int j = tc.Replies.Count();
            for (int i = 0; i < j; i++)
            {
                FormatAndAddPost(tc.Replies[i]);
            }

            this.Title = tc.Title;
        }

        #endregion

        private void _404()
        {
            is_404 = true;
            statusbar.Content = "404'ed";
            statusbar.Foreground = Brushes.Red;

            replybtn.IsEnabled = false;
            updatebtn.IsEnabled = false;
            timer.Stop();

            if (this.TParent.Chan.ArchiveDataProvider != null)
            {
                if (this.TParent.Chan.ArchiveDataProvider.BoardHasArchive(this.Board.Letter))
                {
                    loadDead.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void FormatAndAddPost(GenericPost gp)
        {
            if (dic.ContainsKey(gp.PostNumber))
            {
                //update the quoted by elements, marke the file as deleted, etc.
                PostDisplayerWPF pdf = GetPost(gp.PostNumber);
                if (pdf != null)
                {
                    pdf.UpdateQuotedBy(gp.QuotedBy);
                }
                else
                {
                    dic.Remove(gp.PostNumber);
                }

                if (gp.file == null & pdf.HasFile)
                {
                    pdf.MarkFileAsDeleted();
                }

            }
            else
            {
                PostDisplayerWPF pdf = new PostDisplayerWPF(this);
                pdf.LoadData(gp);
                dic.Add(gp.PostNumber, this.stackpanel.Children.Add(pdf));
            }
        }

        #region PostDisplayer Accessible Methods

        public void PostDisplayer_ImageClicked(GenericPost gp, bool autofocus)
        {
            this.TParent.OpenImage(gp, autofocus);
        }

        public void PostDisplayer_QuoteClicked(int quote)
        {
            this.FocusPost(quote);
        }

        public void PostDisplayer_PostTitleClicked(GenericPost gp)
        {
            this.TParent.FocusQR(this.ID);
            QuickReplyTab t = this.TParent.GetOwnQr(this.ID);
            t.shb.Text = t.shb.Text + ">>" + gp.PostNumber.ToString() + Environment.NewLine;
        }

        public void PostDisplayer_Report(int id)
        {
            return;
        }

        public void PostDisplayer_DeleteMenuClicked(int id)
        {
            this.TParent.ShowDeletePostDialog(this.tc.Instance.PostNumber, id);
        }

        #endregion

        public PostDisplayerWPF GetPost(int id)
        {
            if (dic.ContainsKey(id))
            {
                int index = dic[id];
                return (PostDisplayerWPF)this.stackpanel.Children[index];
            }
            else
            { return null; }
        }

        private PostDisplayerWPF focused_post = null;

        public void FocusTop()
        {
            if (focused_post != null) { focused_post.Unfocus(); }
            this.sc.ScrollToHome();
        }
        public void FocusBottom()
        {
            if (focused_post != null) { focused_post.Unfocus(); }
            this.sc.ScrollToBottom();
        }

        public void FocusPost(int id)
        {
            if (dic.ContainsKey(id))
            {
                int index = dic[id];

                double offset = 0;
                int i = 0;

                for (i = 0; i < index; i++)
                {
                    offset += ((UserControl)this.stackpanel.Children[i]).ActualHeight + 5;
                }

                if (focused_post != null) { focused_post.Unfocus(); }
                focused_post = GetPost(id);
                if (focused_post != null) { focused_post.Focus(); }

                sc.ScrollToVerticalOffset(offset);
            }
        }

        public void CleanUp()
        {
            this.bg.Dispose();
            this.timer.Dispose();

            this.TParent.RemoveQr(this.ID);

            this.stackpanel.Children.Clear();
            this.dic.Clear();

            if (tc != null) { this.tc.Dispose(); }

            SessionManager.UnRegisterThread(this.TParent.Chan, this.Board, this.ID);
        }

        #region Buttons Handlers
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.TParent.ShowQuickReply(this.ID);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void scroll_top(object sender, RoutedEventArgs e) { FocusTop(); }

        private void scroll_bottom(object sender, RoutedEventArgs e) { FocusBottom(); }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog())
            {
                sf.Filter = "Text files|*.txt";
                sf.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                sf.OverwritePrompt = true;
                sf.FileName = this.tc.Instance.PostNumber.ToString() + ".txt";
                if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<string> il = new List<string>();

                    if (tc.Instance.file != null)
                    {
                        il.Add(tc.Instance.file.FullImageLink);
                    }
                    foreach (GenericPost gp in tc.Replies)
                    {
                        if (gp.file != null)
                        {
                            il.Add(gp.file.FullImageLink);
                        }
                    }
                    System.IO.File.WriteAllLines(sf.FileName, il.ToArray());
                }
            }
        }

        private void loadDead_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        #endregion

    }
}
