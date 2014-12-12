using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for BoardHomePageWPF.xaml
    /// </summary>
    public partial class BoardHomePageWPF : UserControl, TabElement
    {
        public BoardInfo Board { get; private set; }

        public int ID { get { return 0; } }

        public BoardBrowserWPF TParent { get; private set; }

        public Common.ElementType Type { get { return Common.ElementType.HomePage; } }

        private BoardRss br;

        private ContextMenu item_menu;

        public string Title { get; private set; }

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public BoardHomePageWPF(BoardInfo board, BoardBrowserWPF parent)
        {
            InitializeComponent();

            this.Board = board;
            this.TParent = parent;
            this.Title = "Home Page";

            br = new BoardRss(board.Letter);

            br.RssLoaded += new BoardRss.RssLoadedEvent(br_RssLoaded);

            title.Content = board.Description;

            item_menu = new ContextMenu();

            MenuItem item1 = new MenuItem();
            item1.Header = "Open threads";
            item1.Click += new RoutedEventHandler(item1_Click);
            item_menu.Items.Add(item1);

            listview1.ContextMenu = item_menu;

            timer.Interval = 2 * 60 * 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            refresh();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            refresh();
        }

        void item1_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in listview1.SelectedItems)
            {
                if (item.GetType() == typeof(Label))
                {
                    Label b = (Label)(item);
                    this.TParent.OpenThread(Convert.ToInt32(b.Tag));
                }
            }
        }

        void br_RssLoaded(List<Rss.RssItem> dic)
        {
            this.listview1.Items.Clear();

            foreach (Rss.RssItem item in dic)
            {
                Label a = new Label();
                a.SetValue(Label.MinHeightProperty, 20d);
                int tid = Convert.ToInt32(item.Guid.Name.Split('/').Last());

                a.Content = String.Format("{0} - {1}", item.Title, item.PubDate.ToString());

                a.ToolTip = String.Format("Thread No. {0}", tid);

                a.Tag = tid;
                a.MouseDoubleClick += new MouseButtonEventHandler(a_MouseDoubleClick);
                this.listview1.Items.Add(a);
            }

            grbox1.Header = String.Format("Showing latest {0} threads - {1} thread", this.Board.Description, dic.Count());

            refreshbutton.IsEnabled = true;
        }

        void a_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.TParent.OpenThread(Convert.ToInt32(((Label)sender).Tag));
        }

        private void refresh()
        {
            try
            {
                grbox1.Header = String.Format("Fetching latest {0} threads", this.Board.Description);
                br.Load();
            }
            catch (Exception)
            {
                grbox1.Header = "An error has been occured while trying to load threads. Please try again";
            }

            refreshbutton.IsEnabled = false;
        }

        private void refreshbutton_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.TParent.OpenCatalog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.TParent.OpenIndex();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.TParent.ShowQuickReply(0);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(textbox1.Text))
            {
                if (Common.IsNumeric(textbox1.Text)) 
                {
                    this.TParent.OpenThread(Int32.Parse(textbox1.Text));
                }
                else if (this.TParent.Chan.ValidateURL(textbox1.Text))
                {
                    var action = this.TParent.Chan.ParseURL(textbox1.Text);

                    if (action.Board == this.Board && action.Action == Chans.URLParserResults.ActionType.Thread)
                    {
                        this.TParent.OpenThread(action.ThreadID);
                    }
                    else 
                    {
                        MessageBox.Show("This URL is not for this board");
                    }
                }
            }
        }

        public void CleanUp()
        {
            this.timer.Stop();
            this.timer.Dispose();

            br.Dispose();
        }
    }
}
