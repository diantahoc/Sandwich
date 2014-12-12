using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandwich
{
    public static class Core
    {

        #region Variables

        //Directories stuffs
        private static string temporary_folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static string cache_dir = Path.Combine(temporary_folder, "sandwich_cache");

        private static string api_cache_dir = Path.Combine(cache_dir, "api");
        public static string img_dir = Path.Combine(cache_dir, "img");
        private static string flags_dir = Path.Combine(cache_dir, "flags");

        public static string misc_dir = Path.Combine(Core.cache_dir, "misc");
        public static string session_dir = Path.Combine(Core.cache_dir, "session");
        public static string log_dir = Path.Combine(Core.cache_dir, "logs");

        private static Dictionary<string, System.Windows.Media.Imaging.BitmapImage> flags = new Dictionary<string, System.Windows.Media.Imaging.BitmapImage>();
        private static Amib.Threading.SmartThreadPool stp;

        #endregion

        static Core()
        {
            WebRequest.DefaultWebProxy = null;
            ServicePointManager.DefaultConnectionLimit = 300;

            Directory.CreateDirectory(cache_dir);

            Directory.CreateDirectory(api_cache_dir);
            Directory.CreateDirectory(img_dir);
            Directory.CreateDirectory(flags_dir);
            Directory.CreateDirectory(misc_dir);
            Directory.CreateDirectory(session_dir);
            Directory.CreateDirectory(log_dir);

            stp = new Amib.Threading.SmartThreadPool() { MaxThreads = 30 };
            stp.Start();
        }

        #region Download Manager

        public static void QueueAction(Action t)
        {
            if (stp.IsShuttingdown) { return; }
            stp.QueueWorkItem(new Amib.Threading.Action(t), Amib.Threading.WorkItemPriority.Normal);
        }

        public static void QueueAction(Action t, Amib.Threading.WorkItemPriority priority)
        {
            if (stp.IsShuttingdown) { return; }
            stp.QueueWorkItem(new Amib.Threading.Action(t), priority);
        }

        public static void Shutdown()
        {
            stp.Shutdown(true, 500);
            //Directory.Delete(img_dir, true);
            //Directory.Delete(api_cache_dir, true);
            File.WriteAllText(Path.Combine(session_dir, "clean"), "");// mark session shutdown as clean
        }

        #endregion

        #region Network Logic

        public static byte[] LoadURL(string url, Common.ProgressChanged cpc, bool use_cache)
        {
            string local_file = Path.Combine(img_dir, string.Format("{0}.{1}", Common.MD5(url), url.Split('.').Last()));

            if (File.Exists(local_file) && use_cache)
            {
                byte[] data = File.ReadAllBytes(local_file);

                if (cpc != null) { cpc(100.0); }

                return data;
            }
            else
            {
                WebClient wb = new WebClient();

                byte[] data;

                try
                {
                    Stream s = wb.OpenRead(url);
                    WebHeaderCollection whc = wb.ResponseHeaders;

                    string content_length = whc["Content-Length"];

                    double totalLength = Convert.ToDouble(content_length);
                    int iByteSize = 0;

                    byte[] byteBuffer = new byte[2048];

                    MemoryStream MemIo = new MemoryStream();

                    double total_processed = 0;

                    while ((iByteSize = s.Read(byteBuffer, 0, 2048)) > 0)
                    {
                        MemIo.Write(byteBuffer, 0, iByteSize);

                        total_processed += iByteSize;

                        if (cpc != null)
                        {
                            cpc((total_processed / totalLength) * 100);
                        }
                    }

                    s.Close();
                    data = MemIo.ToArray();
                    MemIo.Dispose();

                }
                catch (Exception e)
                {
                    if (e.Message.Contains("404"))
                    {
                        File.WriteAllBytes(local_file, Properties.Resources._404);
                        wb.Dispose();
                        return Properties.Resources._404;
                    }
                    else
                    {
                        throw e;
                        /*data = null;
                        File.WriteAllText(local_file, "");
                        return data;*/
                    }
                }

                wb.Dispose();

                if (use_cache)
                {
                    File.WriteAllBytes(local_file, data);
                }

                return data;
            }
        }

        public static APIResponse LoadAPI(string url)
        {
            string hash = Common.MD5(url);

            string file_path = Path.Combine(api_cache_dir, hash); // contain the last fetch date
            string file_path_data = Path.Combine(api_cache_dir, hash + "_data");

            DateTime d;

            APIResponse result;

            if (File.Exists(file_path))
            {
                d = XmlConvert.ToDateTime(File.ReadAllText(file_path), XmlDateTimeSerializationMode.Local);
            }
            else
            {
                d = Common.UnixEpoch;
            }

            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(url);


            wr.IfModifiedSince = d;
            wr.UserAgent = "Sandwich Client";

            WebResponse wbr = null;

            try
            {

                wbr = wr.GetResponse();

                byte[] data = new byte[] { };

                using (Stream s = wbr.GetResponseStream())
                {
                    using (MemoryStream memio = new MemoryStream())
                    {
                        s.CopyTo(memio);
                        data = memio.ToArray();
                    }
                }

                string text = System.Text.Encoding.UTF8.GetString(data);

                string lm = wbr.Headers["Last-Modified"];
                DateTime lmm = DateTime.Parse(lm);

                File.WriteAllText(file_path, XmlConvert.ToString(lmm, XmlDateTimeSerializationMode.Local));
                File.WriteAllText(file_path_data, text);

                result = new APIResponse(text, APIResponse.ErrorType.NoError);
            }
            catch (WebException wex)
            {
                HttpWebResponse httpResponse = wex.Response as HttpWebResponse;
                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.NotModified)
                    {
                        bool data_file_exist = File.Exists(file_path_data);
                        if (data_file_exist)
                        {
                            result = new APIResponse(File.ReadAllText(file_path_data), APIResponse.ErrorType.NoError);
                        }
                        else
                        {
                            delete_file(file_path);
                            delete_file(file_path_data);
                            return LoadAPI(url); //retry fetch
                            //throw new Exception("Reference to a cached file was not found");
                        }
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        result = new APIResponse(null, APIResponse.ErrorType.NotFound);
                    }
                    else
                    {
                        result = new APIResponse(wex.Message, APIResponse.ErrorType.Other);
                        throw wex;
                    }
                }
                else
                {
                    result = new APIResponse(wex.Message, APIResponse.ErrorType.Other);
                    throw wex;
                }
            }

            if (wbr != null)
            {
                wbr.Close();
            }

            return result;
        }

        public static System.Windows.Media.Imaging.BitmapImage LoadFlag(string name, string board)
        {
            string key = name + board;

            if (flags.ContainsKey(key))
            {
                return flags[key];
            }
            else
            {
                string file_name = Path.Combine(flags_dir, key);
                if (File.Exists(file_name))
                {
                    MemoryStream m = new MemoryStream(File.ReadAllBytes(file_name));

                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = m;
                    bi.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    bi.Freeze();

                    flags.Add(key, bi);
                    return bi;
                }
                else
                {
                    WebClient wc = new WebClient();

                    string flag_url = "http://s.4cdn.org/image/country/{0}.gif";

                    if (board == "pol") { flag_url = "http://s.4cdn.org/image/country/troll/{0}.gif "; }

                    byte[] data = wc.DownloadData(String.Format(flag_url, name.ToLower()));

                    File.WriteAllBytes(file_name, data);

                    wc.Dispose();

                    return LoadFlag(name, board);
                }
            }
        }

        public static KeyValuePair<CookieCollection, string> GetHttpResponse(string url, string user_agent, string refere, CookieCollection send_cookies = null)
        {
            try
            {
                HttpWebRequest w = (HttpWebRequest)WebRequest.Create(url);

                w.Method = "GET";
                w.UserAgent = user_agent;
                w.Referer = refere;

                if (send_cookies != null)
                {
                    w.CookieContainer = new CookieContainer(send_cookies.Count);
                    w.CookieContainer.Add(send_cookies);
                }

                
                using (HttpWebResponse web_response = (HttpWebResponse)w.GetResponse())
                {
                    using (var response_stream = web_response.GetResponseStream())
                    {
                        using (MemoryStream temp = new MemoryStream())
                        {
                            response_stream.CopyTo(temp);
                            return new KeyValuePair<CookieCollection, string>(web_response.Cookies, Encoding.UTF8.GetString(temp.ToArray()));
                        }
                    }
                }

              
            }
            catch (Exception)
            {
                return new KeyValuePair<CookieCollection, string>();
            }
        }

        #endregion


        public static void ShowMessage(string message, MessageType t)
        {
            System.Windows.Forms.MessageBox.Show(message);
        }

        public static void NotifyMessage(string message, string title)
        {

        }

        public enum MessageType
        {
            Info,
            Error,
            Warning,
            Success
        }

        private static void delete_file(string s)
        {
            if (File.Exists(s)) { File.Delete(s); }
        }

        public static UploadFile GetEmptyFile(string name)
        {
            return new UploadFile
            {
                Name = name,
                Filename = "",
                ContentType = "application/octet-stream",
                Stream = Stream.Null
            };
        }

        public enum ReportStatus { Success, Captcha, PostGone, Banned, Unkown }

        public enum ReportReason { RuleViolation, IllegalContent, CommercialSpam, Advertisement }

        public enum CaptchaProviderType
        {
            reCaptcha
        }

        public enum PostSendingStatus
        {
            Success,
            Banned,
            PermaBanned,
            RangeBanned,
            CorruptedImage,
            Muted,
            Flood,
            SpamFilter,
            CAPTCHA,
            DuplicateFile,
            Limit,
            InvalidThreadID,
            SubjectRequired,
            InvalidBoard,
            FileRequired,
            FileTooLarge,
            Warned,
            Unkown,
            WebError
        }

        public enum DeleteStatus { Success, WaitLonger, PostGone, Banned, BadPassword, Unkown, Spam }
    }

    #region Classes

    public class SolvedCaptcha
    {

        public string ChallengeField { get; private set; }
        public string ResponseField { get; private set; }

        public SolvedCaptcha(string challenge, string response)
        {
            this.ChallengeField = challenge;
            this.ResponseField = response;
        }
    }

    public class CatalogItem : GenericPost
    {
        public CatalogItem() { }

        public CatalogItem(GenericPost base_data)
        {
            base.board = base_data.board;
            base.capcode = base_data.capcode;
            base.comment = base_data.comment;
            base.country_flag = base_data.country_flag;
            base.country_name = base_data.country_name;
            base.email = base_data.email;
            base.file = base_data.file;
            base.name = base_data.name;
            base.PostNumber = base_data.PostNumber;
            base.subject = base_data.subject;
            base.Tag = base_data.Tag;
            base.time = base_data.time;
            base.trip = base_data.trip;
            base.type = base_data.type;
            foreach (int i in base_data.QuotedBy) { base.MarkAsQuotedBy(i); }
        }

        public int image_replies;
        public int text_replies;

        public int page_number;

        public int TotalReplies { get { return image_replies + text_replies; } }
        public bool IsClosed { get; set; }
        public bool IsSticky { get; set; }
        public GenericPost[] trails;
    }

    public class Thread : GenericPost
    {
        public int image_replies;
        public int text_replies;
        public int page_number;
        public int TotalReplies { get { return image_replies + text_replies; } }
        public bool IsClosed { get; set; }
        public bool IsSticky { get; set; }
    }

    #endregion

}
