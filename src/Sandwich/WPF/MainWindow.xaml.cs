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
using MahApps.Metro.Controls;
namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private Gma.CodeCloud.Controls.CloudControl CC { get { return (Gma.CodeCloud.Controls.CloudControl)(this.host.Child); } }

        public MainWindow()
        {
            InitializeComponent();
           
            //this.bg.Source = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/bgs/5.png", UriKind.Absolute));

            foreach (string a in BoardInfo.BoardsLetters)
            {
                BoardIcon icon = new BoardIcon(a);
                icon.IconClicked += new Common.BoardIconClicked(icon_IconClicked);
                this.boardlist.Children.Add(icon);
            }

            System.Threading.Tasks.Task.Factory.StartNew((Action)delegate
            {
                System.Threading.Thread.Sleep(1500);
                WallbaseWallpaper w = new WallbaseWallpaper();

                BitmapImage bi = w.LoadWallpaperSync();

                if (bi != null)
                {
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        this.bg.Source = bi;
                        this.bg.Stretch = Stretch.UniformToFill;
                    });
                }
            });

            //  this.host.Child = new Gma.CodeCloud.Controls.CloudControl();
        }

        void icon_IconClicked(string letter)
        {
            OpenBoard(letter);
        }

        private void OpenBoard(string choice)
        {
            bool found = false;
            foreach (object child in this.tabs.Items)
            {
                if (child.GetType() == typeof(BoardBrowserWPF))
                {
                    BoardBrowserWPF s = (BoardBrowserWPF)child;
                    if (s.Board == choice)
                    {
                        found = true;
                        this.tabs.SelectedItem = child;
                        break;
                    }
                }
            }
            if (!found)
            {
                BoardBrowserWPF gc = new BoardBrowserWPF(choice);

                ChromeTabs.ChromeTabItem item = new ChromeTabs.ChromeTabItem();
                item.Header = string.Format(@"/{0}/ - {1}", choice, BoardInfo.GetBoardTitle(choice));
                item.Content = gc;

                this.tabs.AddTab(item, true);

                auto_hide();
                //   textBox1.Text = "";
            }
        }

        private void auto_hide() { }

        private string clean(string d)
        {
            string[] allowedChars = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "4", "9", "3" };

            StringBuilder sb = new StringBuilder();

            foreach (char c in d)
            {
                if (Array.IndexOf(allowedChars, c.ToString()) >= 0) { sb.Append(c); }
            }

            string result = sb.ToString();
            return result;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog a = new System.Windows.Forms.OpenFileDialog();
            a.Filter = "PNG |*.png";
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.SaveFileDialog sa = new System.Windows.Forms.SaveFileDialog();
                sa.Filter = "7z|*.7z";
                if (sa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Cornelia decoder = new Cornelia();
                    System.IO.File.WriteAllBytes(sa.FileName, decoder.DecodeFile(System.IO.File.ReadAllBytes(a.FileName)));
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //http://boards.4chan.org/g/res/39212541#p39228300
                string g = textbox1.Text.Trim();

                if (Array.IndexOf(BoardInfo.BoardsLetters, g) >= 0)
                {
                    OpenBoard(g);
                    return;
                }

                if (g.StartsWith("http"))
                {
                    g = g.Replace("https://", "").Replace("http://", "");
                    //boards.4chan.org
                    if (g.Contains("boards.4chan.org"))
                    {
                        g = g.Replace("boards.4chan.org", "");
                        g = g.Remove(0, 1);
                        string[] data = g.Split('/');
                        //g/res/39212541#p39228300
                        //0  1         2           
                        string board = data[0];
                        string tid = data[2].Split('#')[0];

                        OpenBoard(board);
                        BoardBrowserWPF a = get_board(board);
                        a.OpenThread(Convert.ToInt32(tid));

                        textbox1.Text = "";
                    }
                }
            }
        }

        private BoardBrowserWPF get_board(string letter)
        {
            foreach (object item in this.tabs.Items)
            {
                if (item.GetType() == typeof(BoardBrowserWPF))
                {
                    BoardBrowserWPF a = (BoardBrowserWPF)(item);
                    if (a.Board == letter) { return a; }
                }
            }
            return null;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (object tab in this.tabs.Items) { clean_tab(tab); }
            Core.Shutdown();
        }

        private void tabs_ItemRemoved(object tab)
        {
            clean_tab(tab);
        }

        private void clean_tab(object tab)
        {
            if (tab.GetType() == typeof(ChromeTabs.ChromeTabItem))
            {
                ChromeTabs.ChromeTabItem t = (ChromeTabs.ChromeTabItem)tab;
                if (t.Content.GetType() == typeof(BoardBrowserWPF))
                {
                    BoardBrowserWPF a = (BoardBrowserWPF)(t.Content);
                    a.CleanUp();
                    GC.Collect();
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.settingsPanel.IsOpen = true;
        }

    }
}
