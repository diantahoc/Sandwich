using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace Sandwich
{
    public class WallbaseWallpaper
    {
        private string wallpaper_folder = Core.misc_dir;

        private string[] cat_list = new string[] 
        { 
            "http://wallbase.cc/tags/info/33206", /* - for anime */
            "http://wallbase.cc/tags/info/8023", /* - for landscapes */
            "http://wallbase.cc/tags/info/8521" /* - for animals */
        };

        public BitmapImage LoadWallpaperSync()
        {
            string saved_image_path = Path.Combine(Core.misc_dir, string.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

            byte[] data = null;

            if (File.Exists(saved_image_path))
            {
                data = File.ReadAllBytes(saved_image_path);
            }
            else
            {
                data = download_data_b(get_wallpaper_url());
                File.WriteAllBytes(saved_image_path, data);
            }

            if (data != null)
            {
                MemoryStream memio = new MemoryStream(data);

                BitmapImage bbb = new BitmapImage();
                bbb.BeginInit();
                bbb.CacheOption = BitmapCacheOption.OnLoad;
                bbb.StreamSource = memio;
                bbb.EndInit();
                bbb.Freeze();
                return bbb;
            }
            else
            {
                return null;
            }
        }

        private string get_wallpaper_url()
        {
            Random rnd = new Random();

            string content = download_data(cat_list[rnd.Next(0, cat_list.Length)]);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);

            // body -> id:wrap - > id:thumbs -> class:thumbnail

            List<HtmlNode> thumbs = new List<HtmlNode>();

            HtmlNode thumbNode = (HtmlNode)doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[6]/section[1]");

            foreach (HtmlNode node in thumbNode.ChildNodes)
            {
                if (node.GetAttributeValue("class", "").Contains("thumbnail"))
                {
                    thumbs.Add(node);
                }
            }
                                            
            List<string> urls = new List<string>();

            foreach (HtmlNode thumb in thumbs)
            {
                urls.Add(string.Format("http://wallbase.cc/wallpaper/{0}", thumb.Id.Replace("thumb", "")));
            }

            string page_url = urls[rnd.Next(0, urls.Count)];

            string image_url = parse_wallpaper_image_url(page_url);

            return image_url;
        }

        private string parse_wallpaper_image_url(string page_url)
        {
            string content = download_data(page_url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);

            List<HtmlNode> thumbs = new List<HtmlNode>();

            string url = "";

            HtmlNode imgNode = (HtmlNode)doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[2]/div[2]/img[1]");
           
            url = imgNode.GetAttributeValue("src", "");

            return url;
        }

        private string download_data(string url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/18.0 (compatible; MSIE 10.0; Windows NT 5.2; .NET CLR 3.5.3705;)");

                string content = "";

                try
                {
                    content = wc.DownloadString(url);
                }
                catch (WebException wex)
                {
                    if (wex.Status == WebExceptionStatus.Timeout ||
                        wex.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        //try again
                        content = wc.DownloadString(url);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception) { content = ""; }

                return content;
            }
        }

        private byte[] download_data_b(string url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/18.0 (compatible; MSIE 10.0; Windows NT 5.2; .NET CLR 3.5.3705;)");

                byte[] content = null;

                try
                {
                    content = wc.DownloadData(url);
                }
                catch (WebException wex)
                {
                    if (wex.Status == WebExceptionStatus.Timeout ||
                        wex.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        //try again
                        content = wc.DownloadData(url);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception) { content = null; }

                return content;
            }
        }
    }
}