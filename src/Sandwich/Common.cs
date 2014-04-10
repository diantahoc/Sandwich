using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using Sandwich.Helpers;
namespace Sandwich
{
    public static class Common
    {
        public static BitmapImage ErrorImage;
        public static BitmapImage OptionImage;
        public static BitmapImage NoThumbImage;

        //public static System.Text.RegularExpressions.Regex quoteMatcher;

        static Common()
        {
            ErrorImage = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/error.png", UriKind.Absolute));
            ErrorImage.Freeze();

            OptionImage = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/Icons/appbar.settings.png", UriKind.Absolute));
            OptionImage.Freeze();

            NoThumbImage = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/nothumb.png", UriKind.Absolute));
            NoThumbImage.Freeze();

           // quoteMatcher = new System.Text.RegularExpressions.Regex(">>[0-9]+", System.Text.RegularExpressions.RegexOptions.Compiled |  System.Text.RegularExpressions.RegexOptions.Singleline);
        }

        public static string DecodeHTML(string text)
        {
            if (!(String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text)))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); doc.LoadHtml(text);
                return System.Web.HttpUtility.HtmlDecode(ThreadHelper.GetNodeText(doc.DocumentNode));
            }
            else
            {
                return "";
            }
        }

        public static int click_duration { get { return 250; } }

        public enum ElementType { HomePage, Catalog, Thread, ImageView, Index }
        public enum PostingMode { NewThread, Reply }

        //posts events
        public delegate void ImageClickEvent(GenericPost gp, bool autofocus);
        public delegate void PostTitleClickEvent(GenericPost gp);
        public delegate void QuoteClickEvent(int quote);

        //catalog events

        public delegate void ThreadLoadRequest(CatalogItem Ci);

        //core events
        public delegate void ProgressChanged(double p);
        public delegate void DownloadFinished(string work_id, byte[] data, bool error);

        public delegate void BoardIconClicked(string letter);

        //common methodes

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime ParseUTC_Stamp(int timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp);
        }

        public static string MD5(string s)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                return ByteArrayToString(md5.ComputeHash(Encoding.ASCII.GetBytes(s)));
            }
        }

        public static string ByteArrayToString(byte[] arrInput)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte x in arrInput)
            {
                sb.Append(x.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }

        public static string Map_MIME_Type(string filename)
        {
            switch (filename.Split('.')[1].ToLower()) 
            {
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                case "gif":
                    return "image/gif";
                case "pdf":
                    return "application/acrobat";
                default:
                    return "application/octet-stream";
            }
           
        }

        public static string format_size_string(double size)
        {
            double KB = 1024;
            double MB = 1048576;
            double GB = 1073741824;
            if (size < KB)
            {
                return size.ToString() + " B";
            }
            else if (size > KB & size < MB)
            {
                return Math.Round(size / KB, 2).ToString() + " KB";
            }
            else if (size > MB & size < GB)
            {
                return Math.Round(size / MB, 2).ToString() + " MB";
            }
            else if (size > GB)
            {
                return Math.Round(size / GB, 2).ToString() + " GB";
            }
            else
            {
                return Convert.ToString(size);
            }
        }

        public static string format_size_string(int size) 
        {
            return format_size_string(Convert.ToDouble(size));
        }


    }

}
