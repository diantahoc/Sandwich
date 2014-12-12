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
using Sandwich.Chans;

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Dictionary<string, IChan> Chans = new Dictionary<string, IChan>() 
        {
            {"4chan", new Sandwich.Chans._4chan._4chan()},
            {"rb", new Sandwich.Chans.RandomBoard.RandomBoard()}
        };

        public MainWindow()
        {
            InitializeComponent();

            foreach (var chan in this.Chans)
            {
                //BoardIcon icon = new BoardIcon();
                //icon.IconClicked += new Common.BoardIconClicked(icon_IconClicked);
                //this.boardlist.Children.Add(icon);
            }

            #region BackGroundImage

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

            #endregion
        }

        private BoardBrowserWPF OpenBoard(IChan chan, BoardInfo board)
        {
            foreach (object child in this.tabs.Items)
            {
                if (child.GetType() == typeof(BoardBrowserWPF))
                {
                    BoardBrowserWPF s = (BoardBrowserWPF)child;
                    if (s.Board == board && s.Chan == chan)
                    {
                        this.tabs.SelectedItem = child;
                        return s;
                    }
                }
            }

            BoardBrowserWPF gc = new BoardBrowserWPF(chan, board);

            ChromeTabs.ChromeTabItem item = new ChromeTabs.ChromeTabItem();
            item.Header = string.Format(@"/{0}/ - {1}", board, board.Description);
            item.Content = gc;

            this.tabs.AddTab(item, true);
            return gc;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                foreach (var chan in Chans)
                {
                    if (textbox1.Text.StartsWith(chan.Value.Key))
                    {
                        string data = textbox1.Text.Split('/').Last();

                        BoardInfo bi = chan.Value.ValidateBoard(data);

                        if (bi != null)
                        {
                            OpenBoard(chan.Value, bi);
                        }
                    }
                    else if (chan.Value.ValidateURL(textbox1.Text))
                    {

                        var action = chan.Value.ParseURL(textbox1.Text);

                        switch (action.Action)
                        {
                            case URLParserResults.ActionType.Unkown:
                                return;
                            case URLParserResults.ActionType.Catalog:
                                {
                                    var board = OpenBoard(chan.Value, action.Board);
                                    board.OpenCatalog();
                                    return;
                                }
                            case URLParserResults.ActionType.HomePage:
                                {
                                    OpenBoard(chan.Value, action.Board);
                                    return;
                                }
                            case URLParserResults.ActionType.Page:
                                {
                                    var board = OpenBoard(chan.Value, action.Board);
                                    board.OpenIndex(action.PageNumber);
                                    return;
                                }
                            case URLParserResults.ActionType.Thread:
                                {
                                    var board = OpenBoard(chan.Value, action.Board);
                                    board.OpenThread(action.ThreadID);
                                    return;
                                }
                            default:
                                return;
                        }
                    }
                }
            }
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



        //private void a555()
        //{
        //    string data = System.IO.File.ReadAllText(@"C:\Users\\Desktop\catalog.htm");

        //    string js_data_xpath = "/html[1]/head[1]/script[3]";

        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

        //    doc.LoadHtml(data);

        //    string js_data = doc.DocumentNode.SelectSingleNode(js_data_xpath).InnerHtml;

        //    //System.IO.File.WriteAllText(@"C:\Users\Istan\Desktop\catalog.js", js_data);

        //    int var_catalog_index = js_data.IndexOf("var catalog = ", 0);

        //    int var_style_groupe_index = js_data.LastIndexOf("var style_group =");

        //    string catalog_data = js_data.Remove(var_style_groupe_index);

        //    catalog_data = catalog_data.Substring(var_catalog_index);

        //    catalog_data = catalog_data.Remove(0, 14); // 14 = "var catalog = ".Length

        //    catalog_data = catalog_data.Trim();

        //    catalog_data = catalog_data.Remove(catalog_data.Length - 1);

        //    Newtonsoft.Json.Linq.JObject t = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(catalog_data);

        //    List<CatalogItem> threads = new List<CatalogItem>();

        //    Newtonsoft.Json.Linq.JToken thread_json = t["threads"];

        //    for (int i = 0, c = thread_json.Count(); i < c; i++)
        //    {
        //        Newtonsoft.Json.Linq.JProperty thread_prop = (Newtonsoft.Json.Linq.JProperty)thread_json.ElementAt(i);

        //        CatalogItem ci = new CatalogItem();
        //        ci.PostNumber = Convert.ToInt32(thread_prop.Name);

        //        Newtonsoft.Json.Linq.JToken thread = thread_prop.ElementAt(0);

        //        ci.board = Convert.ToString(t["slug"]);

        //        ci.time = Common.ParseUTC_Stamp(Convert.ToInt32(thread["date"]));

        //        ci.text_replies = Convert.ToInt32(thread["r"]);
        //        ci.image_replies = Convert.ToInt32(thread["i"]);

        //        if (thread["lr"] != null)
        //        {
        //            int lr_id = Convert.ToInt32(thread["lr"]["id"]);
        //            if (lr_id != ci.PostNumber)
        //            {
        //                ci.trails = new GenericPost[]
        //                {
        //                    new GenericPost()
        //                    {
        //                        board = ci.board,
        //                        PostNumber = lr_id,
        //                        time = Common.ParseUTC_Stamp(Convert.ToInt32(thread["lr"]["date"])),
        //                        name = Convert.ToString(thread["lr"]["author"])
        //                    }
        //                };
        //            }
        //        }

        //        ci.name = Convert.ToString(thread["author"]);
        //        ci.subject = Convert.ToString(thread["sub"]);
        //        ci.comment = Convert.ToString(thread["teaser"]);


        //        if (thread["imgurl"] != null) 
        //        {
        //            PostFile p = new PostFile();
        //            p.owner = ci.PostNumber;
        //            p.thumbW = Convert.ToInt32(thread["tn_w"]);
        //            p.thumbH = Convert.ToInt32(thread["tn_h"]);
        //            p.thumbnail_tim = Convert.ToString(thread["imgurl"]);
        //            ci.file = p;
        //        }

        //        threads.Add(ci);
        //    }

        //    return;

        //}

    }
}
