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
using Sandwich.WPF;
using System.Text.RegularExpressions;


namespace Sandwich
{
    /// <summary>
    /// Interaction logic for CatalogPresenter.xaml
    /// </summary>
    public partial class CatalogPresenter : UserControl, TabElement, IDisposable
    {
        public string Board { get; private set; }

        public int ID { get { return 0; } }
        public Common.ElementType Type { get { return Common.ElementType.Catalog; } }

        public BoardBrowserWPF TParent { get; private set; }
        public string Title { get { return "Catalog"; } }


        private CatalogItem[][] data;
        private System.ComponentModel.BackgroundWorker bg;
        private System.Windows.Forms.Timer timer;

        private int updin = 60;

        System.Type cpi_type = typeof(CatalogPresenterItem);

        private int _sort_type = 0;
        private int _sort_index = 0;

        public CatalogPresenter(string board, BoardBrowserWPF parent)
        {
            InitializeComponent();

            SessionManager.RegisterCatalog(board);

            this.Board = board;
            this.TParent = parent;

            this.bg = new System.ComponentModel.BackgroundWorker();
            this.bg.DoWork += new System.ComponentModel.DoWorkEventHandler(bg_DoWork);
            this.bg.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);


            this.timer = new System.Windows.Forms.Timer();
            this.timer.Interval = 60000; // 1 min
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Start();

            this.refresh();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!bg.IsBusy)
            {
                updin--;
                if (updin <= 0)
                {
                    refresh();
                    updin = 60;
                }
                status.Content = "Updating in " + updin.ToString() + " mins";
            }
        }

        void bg_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Cannot refresh the catalog, please try again", "Error");
            }
            else
            {
                handle_data();
            }
        }

        void bg_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                data = Core.GetCatalog(this.Board);
            }
            catch (Exception)
            {
                e.Cancel = true;
                return;
            }
        }

        private void handle_data()
        {
            if (data != null)
            {
                RemoveAll();
                GC.Collect();

                for (int i = 0; i < data.Length; i++)
                {
                    foreach (CatalogItem ci in data[i])
                    {
                        CatalogPresenterItem cpi = new CatalogPresenterItem(ci, this);

                        cpi.PictureBoxClicked += cpi_PictureBoxClicked;

                        this.AddItem(cpi);
                    }
                }
            }
        }

        void cpi_PictureBoxClicked(CatalogItem Ci)
        {
            this.TParent.OpenThread(Ci.PostNumber);
        }

        private void refresh()
        {
            if (bg.IsBusy)
            {
                return;
            }
            else
            {
                bg.RunWorkerAsync();
                status.Content = "Refreshing catalog...";
            }
        }

        public void AddItem(CatalogPresenterItem cpi)
        {
            this.cataloglist.Children.Add(cpi);
        }

        public void CleanUp()
        {
            this.bg.Dispose();
            this.timer.Dispose();

            RemoveAll();

            SessionManager.UnRegisterCatalog(this.Board);
        }

        public void Dispose() { CleanUp(); }

        public void RemoveAll()
        {
            for (int i = 0; i < this.cataloglist.Children.Count; )
            {
                if (this.cataloglist.Children[i].GetType() == cpi_type)
                {
                    CatalogPresenterItem cpi = (CatalogPresenterItem)(this.cataloglist.Children[i]);
                    cpi.PictureBoxClicked -= cpi_PictureBoxClicked;
                    cpi.Dispose();
                    this.cataloglist.Children.RemoveAt(i);
                }
            }
            this.cataloglist.Children.Clear();
        }

        public void SetFilter(string filter)
        {
            if (!(string.IsNullOrEmpty(filter) || string.IsNullOrWhiteSpace(filter)))
            {

                Regex r = null;

                try
                {
                    r = new Regex(filter, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                }
                catch (Exception)
                {
                    return;
                }

                foreach (UIElement ci in this.cataloglist.Children)
                {
                    if (ci.GetType() == cpi_type)
                    {
                        CatalogPresenterItem c = (CatalogPresenterItem)(ci);

                        if (r.IsMatch(c.Comment) | r.IsMatch(c.Subject))
                        {
                            c.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            c.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
            }
            else
            {
                foreach (UIElement ci in this.cataloglist.Children) { ci.Visibility = System.Windows.Visibility.Visible; }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetFilter(textbox1.Text);
        }

        private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetFilter(textbox1.Text);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox se = (ComboBox)sender;

            _sort_index = se.SelectedIndex;

            sort();
        }

        private void sort() 
        {
            if (this.cataloglist != null)
            {
                if (this.cataloglist.Children != null)
                {
                    IEnumerable<CatalogPresenterItem> data = this.cataloglist.Children.Cast<CatalogPresenterItem>();
                    IEnumerable<CatalogPresenterItem> sorted = null;

                    switch (_sort_index)
                    {
                        case 0:
                            sorted = data.OrderBy(x => x.LastReplyTime);
                            break;
                        case 1:
                            sorted = data.OrderBy(x => x.TotalReplies);
                            break;
                        case 2:
                            sorted = data.OrderBy(x => x.Time);
                            break;
                        case 3:
                            sorted = data.OrderBy(x => x.LastReplyTime);
                            break;
                        case 4:
                            sorted = data.OrderBy(x => x.PostNumber);
                            break;
                        default:
                            return;
                    }

                    if (_sort_type == 1) { sorted = sorted.Reverse(); }

                    foreach (CatalogPresenterItem cpu in sorted)
                    {
                        this.cataloglist.Children.Remove(cpu);
                        this.cataloglist.Children.Add(cpu);
                    }
                }
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ComboBox se = (ComboBox)sender;

            double size = 250 + se.SelectedIndex * 100;

            if (this.cataloglist != null)
            {
                if (this.cataloglist.Children != null)
                {
                    foreach (UIElement ci in this.cataloglist.Children)
                    {
                        if (ci.GetType() == cpi_type)
                        {
                            CatalogPresenterItem c = (CatalogPresenterItem)(ci);
                            c.ChangeSize(size);
                        }
                    }
                }
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RegexHelp rexg = new RegexHelp();
            rexg.Show();
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            ComboBox se = (ComboBox)sender;

            _sort_type = se.SelectedIndex;
            sort();
        }


    }
}
