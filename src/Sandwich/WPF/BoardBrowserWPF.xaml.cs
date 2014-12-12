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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sandwich.Chans;
using Sandwich.DataTypes;

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for BoardBrowserWPF.xaml
    /// </summary>
    public partial class BoardBrowserWPF : UserControl
    {
        public BoardInfo Board { get; private set; }

        public string Title { get; set; }
        public BitmapImage Icon { get; set; }

        private Dictionary<int, ThreadViewerWPF> threads = new Dictionary<int, ThreadViewerWPF>();
        //private List<int> open_images = new List<int>();

        private string board_title;

        QuickReply q;

        public IChan Chan { get; private set; }

        public BoardBrowserWPF(IChan chan, BoardInfo board)
        {
            InitializeComponent();

            this.Chan = chan;
            this.Board = board;
            this.Icon = board.Icon;
            this.board_title = board.Description;
            this.Title = board_title;

            q = new QuickReply(chan, board);
            q.SetValue(Grid.ColumnProperty, 1);

            this.qrCon.Content = (q);

        }

        public void ShowQuickReply(int tid)
        {
            if (tid > 0)
            {
                q.AddThreadReply(tid);
            }
            else
            {
                q.AddNewThread();
            }
            this.qrCon.IsOpen = true;
        }

        public QuickReplyTab GetOwnQr(int tid)
        {
            QuickReplyTab t = q.get_tab(tid);
            if (t == null)
            {
                return q.AddThreadReply(tid);
            }
            else
            {
                return t;
            }
        }

        public void RemoveQr(int tid)
        {
            q.RemoveTab(tid);
        }

        public void FocusQR(int tid)
        {
            this.qrCon.IsOpen = true;
            q.FocusThreadID(tid);
        }

        public void OpenCatalog()
        {
            CatalogPresenter cat = new CatalogPresenter(this.Board, this);
            BindTab(cat, false);
        }

        public void ShowDeletePostDialog(int tid, int id)
        {
            this.fly.Content = new DeletePostDialog(this.Chan, this.Board.Letter, tid, id);
            this.fly.Header = "Deleting post: " + id.ToString();
            this.fly.IsOpenChanged += (s, e) =>
            {
                if (this.fly.IsOpen)
                {
                    this.fly.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.fly.Visibility = System.Windows.Visibility.Collapsed;
                }
            };
            this.fly.IsOpen = true;
        }

        public void OpenIndex()
        {
            MessageBox.Show("Not implemented yet");
        }

        public void OpenIndex(int page)
        {
            MessageBox.Show("Not implemented yet");
        }

        public void OpenThread(int id)
        {
            if (threads.ContainsKey(id))
            {
                int index = -2;
                foreach (ChromeTabs.ChromeTabItem tab in this.tabs.Items)
                {
                    if (tab.Content != null)
                    {
                        if (tab.Content.GetType() == typeof(ThreadViewerWPF))
                        {
                            if (tab.Content == threads[id])
                            {
                                index = tabs.Items.IndexOf(tab);
                                break;
                            }
                        }
                    }
                }
                if (index == -2)
                {
                    //not found
                    BindTab(threads[id], true);
                }
                else
                {
                    this.tabs.SelectedIndex = index;
                }
            }
            else
            {
                ThreadViewerWPF tvw = new ThreadViewerWPF(id, this.Board, this);
                threads.Add(id, tvw);
                BindTab(threads[id], false);
            }
        }

        private void OpenHomePage()
        {
            BoardHomePageWPF hp = new BoardHomePageWPF(this.Board, this);

            BindTab(hp, true);
        }

        public void OpenImage(GenericPost gp, bool autofocus)
        {
            if (get_tabelement(Common.ElementType.ImageView, gp.PostNumber) != null)
            {
                return;
            }
            else
            {
                ImageViewer iv = new ImageViewer(gp, this);
                BindTab(iv, autofocus);
            }
        }

        private TabElement get_tabelement(Common.ElementType type, int id)
        {
            foreach (ChromeTabs.ChromeTabItem tab in this.tabs.Items)
            {
                TabElement element = (TabElement)tab.Content;
                if (element.Type == type)
                {
                    switch (element.Type)
                    {
                        case Common.ElementType.Catalog:
                        case Common.ElementType.HomePage:
                        case Common.ElementType.Index:
                            return element;
                        case Common.ElementType.ImageView:
                        case Common.ElementType.Thread:
                            if (element.ID == id)
                            {
                                return element;
                            }
                            else { continue; }
                        default:
                            continue;
                    }
                }
            }
            return null;
        }

        private void BindTab(TabElement a, bool autofocus)
        {
            ChromeTabs.ChromeTabItem bind = new ChromeTabs.ChromeTabItem();
            bind.Content = a;
            bind.Header = a.Title;

            this.tabs.AddTab(bind, autofocus);
        }

        bool a = true;

        private void tabs_Loaded(object sender, RoutedEventArgs e)
        {
            if (a) { OpenHomePage(); a = false; }
        }

        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    this.tabs.Items.Clear();
        //    foreach (ThreadViewerWPF a in threads.Values)
        //    {
        //        if (a != null) { a.CleanUp(); }
        //    }
        //    GC.Collect();
        //}

        public void CleanUp()
        {
            foreach (object tab in this.tabs.Items)
            {
                clean_tab(tab);
            }
            GC.Collect();
        }

        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.tabs.SelectedItem != null)
            {
                if (this.tabs.SelectedItem.GetType() == typeof(ChromeTabs.ChromeTabItem))
                {
                    ChromeTabs.ChromeTabItem item = (ChromeTabs.ChromeTabItem)this.tabs.SelectedItem;

                    if (item.Content.GetType() == typeof(TabElement))
                    {
                        TabElement content = (TabElement)(item.Content);

                        this.Title = String.Format("{0} - {1}", content.Title, this.board_title);
                    }
                }
            }
        }

        System.Type[] types = { typeof(ThreadViewerWPF), typeof(ImageViewer), typeof(BoardHomePageWPF), typeof(CatalogPresenter), typeof(TabElement) };

        private void tabs_ItemRemoved(object tab)
        {
            clean_tab(tab); GC.Collect();
        }

        private void clean_tab(object tab)
        {
            if (tab.GetType() == typeof(ChromeTabs.ChromeTabItem))
            {
                ChromeTabs.ChromeTabItem t = (ChromeTabs.ChromeTabItem)tab;
                if (Array.IndexOf(types, t.Content.GetType()) >= 0)
                {
                    TabElement a = (TabElement)(t.Content);
                    if (a.Type == Common.ElementType.Thread)
                    {
                        if (threads.ContainsKey(a.ID)) { threads.Remove(a.ID); }
                    }
                    a.CleanUp();
                }
            }
        }

    }
}
