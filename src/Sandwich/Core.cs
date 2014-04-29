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

        /// <summary>
        /// {0}: board, {1}: tim, {2}: ext
        /// </summary>
        public const string imageLink = @"http://i.4cdn.org/{0}/{1}.{2}";
        /// <summary>
        /// {0}:board, {1}: tim
        /// </summary>
        public const string thumbLink = @"http://t.4cdn.org/{0}/{1}s.jpg";

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
            ServicePointManager.DefaultConnectionLimit = 250;

            Directory.CreateDirectory(cache_dir);

            Directory.CreateDirectory(api_cache_dir);
            Directory.CreateDirectory(img_dir);
            Directory.CreateDirectory(flags_dir);
            Directory.CreateDirectory(misc_dir);
            Directory.CreateDirectory(session_dir);
            Directory.CreateDirectory(log_dir);

            stp = new Amib.Threading.SmartThreadPool() { MaxThreads = 20 };
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

        #region Data Parsers

        /// <summary>
        /// Download the catalog.json file and parse it into an array of pages that contain an array of CatalogItem
        /// Pages contain an array of catalogs items.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="web_progress"></param>
        /// <returns></returns>
        /// 
        public static CatalogItem[][] GetCatalog(string board)
        {
            APIResponse response = LoadAPI("http://a.4cdn.org/%/catalog.json".Replace("%", board));

            switch (response.Error)
            {
                case APIResponse.ErrorType.NoError:

                    List<CatalogItem[]> il = new List<CatalogItem[]>();

                    JArray list = JsonConvert.DeserializeObject<JArray>(response.Data);
                    //p is page index
                    //u is thread index
                    for (int p = 0; p < list.Count(); p++)
                    {
                        JToken page = list[p];

                        List<CatalogItem> Unipage = new List<CatalogItem>();

                        JArray threads = (JArray)page["threads"];

                        for (int u = 0; u < threads.Count; u++)
                        {
                            JToken thread = threads[u];
                            Unipage.Add(ParseJToken_Catalog(thread, p, board));
                        }

                        il.Add(Unipage.ToArray());

                        Unipage = null;
                    }

                    return il.ToArray();

                case APIResponse.ErrorType.NotFound:
                    throw new Exception("404");

                case APIResponse.ErrorType.Other:
                    throw new Exception(response.Data);

                default:
                    return null;
            }

        }

        public static CatalogItem[][] GetCatalogNoAPI(string board)
        {
            APIResponse response = LoadAPI(string.Format("http://boards.4chan.org/{0}/catalog", board));

            switch (response.Error)
            {
                case APIResponse.ErrorType.NoError:
                    return new CatalogItem[][] { Parse_Catalog_Page_HTML(response.Data) };
                case APIResponse.ErrorType.NotFound:
                    throw new Exception("404");
                case APIResponse.ErrorType.Other:
                    throw new Exception(response.Data);
                default:
                    return null;
            }
        }

        private static CatalogItem[] Parse_Catalog_Page_HTML(string data)
        {
            string js_data_xpath = "/html[1]/head[1]/script[3]";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(data);

            string js_data = doc.DocumentNode.SelectSingleNode(js_data_xpath).InnerHtml;

            int var_catalog_index = js_data.IndexOf("var catalog = ", 0);

            int var_style_groupe_index = js_data.LastIndexOf("var style_group =");

            string catalog_data = js_data.Remove(var_style_groupe_index);

            catalog_data = catalog_data.Substring(var_catalog_index);

            catalog_data = catalog_data.Remove(0, 14); // 14 = "var catalog = ".Length

            catalog_data = catalog_data.Trim();

            catalog_data = catalog_data.Remove(catalog_data.Length - 1);

            Newtonsoft.Json.Linq.JObject t = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(catalog_data);

            List<CatalogItem> threads = new List<CatalogItem>();

            Newtonsoft.Json.Linq.JToken thread_json = t["threads"];

            for (int i = 0, c = thread_json.Count(); i < c; i++)
            {
                Newtonsoft.Json.Linq.JProperty thread_prop = (Newtonsoft.Json.Linq.JProperty)thread_json.ElementAt(i);

                CatalogItem ci = new CatalogItem();
                ci.PostNumber = Convert.ToInt32(thread_prop.Name);

                Newtonsoft.Json.Linq.JToken thread = thread_prop.ElementAt(0);

                ci.board = Convert.ToString(t["slug"]);

                ci.time = Common.ParseUTC_Stamp(Convert.ToInt32(thread["date"]));

                ci.text_replies = Convert.ToInt32(thread["r"]);
                ci.image_replies = Convert.ToInt32(thread["i"]);

                if (thread["lr"] != null)
                {
                    int lr_id = Convert.ToInt32(thread["lr"]["id"]);
                    if (lr_id != ci.PostNumber)
                    {
                        ci.trails = new GenericPost[]
                        {
                            new GenericPost()
                            {
                                board = ci.board,
                                PostNumber = lr_id,
                                time = Common.ParseUTC_Stamp(Convert.ToInt32(thread["lr"]["date"])),
                                name = HttpUtility.HtmlDecode(Convert.ToString(thread["lr"]["author"]))
                            }
                        };
                    }
                }

                ci.name = HttpUtility.HtmlDecode(Convert.ToString(thread["author"]));
                ci.subject = HttpUtility.HtmlDecode(Convert.ToString(thread["sub"]));

                ci.comment = Common.DecodeHTML(Convert.ToString(thread["teaser"]));

                if (thread["trip"] != null)
                {
                    ci.trip = HttpUtility.HtmlDecode(Convert.ToString(thread["trip"]));
                }
                else { ci.trip = ""; }

                if (thread["capcode"] != null)
                {
                    ci.capcode = parse_capcode(Convert.ToString(thread["capcode"]));
                }

                if (thread["sticky"] != null)
                {
                    ci.IsSticky = (Convert.ToInt32(thread["sticky"]) == 1);
                }

                if (thread["closed"] != null)
                {
                    ci.IsClosed = (Convert.ToInt32(thread["closed"]) == 1);
                }

                ci.email = "";

                if (thread["imgurl"] != null)
                {
                    PostFile p = new PostFile();
                    p.board = ci.board;
                    p.owner = ci.PostNumber;
                    p.thumbW = Convert.ToInt32(thread["tn_w"]);
                    p.thumbH = Convert.ToInt32(thread["tn_h"]);
                    p.thumbnail_tim = Convert.ToString(thread["imgurl"]);
                    ci.file = p;
                }

                threads.Add(ci);
            }
            return threads.ToArray();
        }

        public static ThreadContainer[] GetPage(string board, int page)
        {
            APIResponse response = LoadAPI("http://api.4chan.org/%/$.json".Replace("%", board).Replace("$", page.ToString()));

            switch (response.Error)
            {
                case APIResponse.ErrorType.NoError:
                    List<ThreadContainer> il = new List<ThreadContainer>();

                    JObject list = JsonConvert.DeserializeObject<JObject>(response.Data);

                    //u is thread index

                    JArray threads = (JArray)list["threads"];

                    for (int u = 0; u < threads.Count; u++)
                    {
                        JToken posts_object = threads[u]; // array of 'posts' objects. Each one is an array of {Main threads + last replies}.
                        JToken posts_property = posts_object["posts"];

                        //first one is 0 -- > a thread
                        //the rest is the last replies

                        ThreadContainer tc = new ThreadContainer(ParseThread(posts_property[0], board));

                        for (int post_index = 1; post_index < posts_property.Count(); post_index++)
                        {
                            JToken single_post = posts_property[post_index];
                            tc.AddReply(ParseReply(single_post, board));
                        }
                        il.Add(tc);
                    }
                    return il.ToArray();

                case APIResponse.ErrorType.NotFound:
                    throw new Exception("404");

                case APIResponse.ErrorType.Other:
                    throw new Exception(response.Data);

                default:
                    return null;
            }
        }

        public static ThreadContainer GetThreadData(string board, int id)
        {
            APIResponse response = LoadAPI(string.Format("http://a.4cdn.org/{0}/res/{1}.json", board, id));

            switch (response.Error)
            {
                case APIResponse.ErrorType.NoError:
                    ThreadContainer tc = null;

                    JObject list = JsonConvert.DeserializeObject<JObject>(response.Data);

                    if (list["posts"] != null && list["posts"].Type != JTokenType.Null)
                    {
                        JContainer data = (JContainer)list["posts"];
                        tc = new ThreadContainer(ParseThread(data[0], board));

                        for (int index = 1; index < data.Count; index++)
                        {
                            tc.AddReply(ParseReply(data[index], board));
                        }
                    }

                    return tc;

                case APIResponse.ErrorType.NotFound:
                    throw new Exception("404");

                case APIResponse.ErrorType.Other:
                    throw new Exception(response.Data);

                default:
                    return null;
            }
        }

        private static Thread ParseThread(JToken data, string board)
        {
            Thread t = new Thread();

            t.board = board;

            //comment
            if (data["com"] != null)
            {
                t.comment = data["com"].ToString();
            }
            else
            {
                t.comment = "";
            }

            //mail
            if (data["email"] != null)
            {
                t.email = HttpUtility.HtmlDecode(data["email"].ToString());
            }
            else
            {
                t.email = "";
            }

            //poster name
            if (data["name"] != null)
            {
                t.name = HttpUtility.HtmlDecode(data["name"].ToString());
            }
            else
            {
                t.name = "";
            }

            //subject
            if (data["sub"] != null)
            {
                t.subject = HttpUtility.HtmlDecode(data["sub"].ToString());
            }
            else
            {
                t.subject = "";
            }

            if (data["trip"] != null)
            {
                t.trip = data["trip"].ToString();
            }
            else
            {
                t.trip = "";
            }

            if (data["capcode"] != null)
            {
                t.capcode = parse_capcode(Convert.ToString(data["capcode"]));
            }

            if (data["tag"] != null)
            {
                t.Tag = parse_tag(Convert.ToString(data["tag"]));
            }
            else { t.Tag = GenericPost.ThreadTag.NoTag; }

            if (data["sticky"] != null)
            {
                t.IsSticky = (data["sticky"].ToString() == "1");
            }

            if (data["closed"] != null)
            {
                t.IsClosed = (Convert.ToInt32(data["closed"]) == 1);
            }

            if (data["country"] != null)
            {
                t.country_flag = data["country"].ToString();
            }
            else
            {
                t.country_flag = "";
            }

            if (data["country_name"] != null)
            {
                t.country_name = data["country_name"].ToString();
            }
            else
            {
                t.country_name = "";
            }

            t.file = ParseFile(data, board);

            t.image_replies = Convert.ToInt32(data["images"]); ;

            t.PostNumber = Convert.ToInt32(data["no"]); ;

            t.text_replies = Convert.ToInt32(data["replies"]);
            t.time = Common.ParseUTC_Stamp(Convert.ToInt32(data["time"]));

            return t;
        }

        private static GenericPost.Capcode parse_capcode(string cap)
        {
            switch (cap.ToLower())
            {
                /*none, mod, admin, admin_highlight, developer*/
                case "admin":
                case "admin_highlight":
                    return GenericPost.Capcode.Admin;
                case "developer":
                    return GenericPost.Capcode.Developer;
                case "mod":
                    return GenericPost.Capcode.Mod;
                default:
                    return GenericPost.Capcode.None;
            }
        }

        private static PostFile ParseFile(JToken data, string board)
        {
            if (data["filename"] != null)
            {
                PostFile pf = new PostFile();
                pf.filename = HttpUtility.HtmlDecode(data["filename"].ToString());
                pf.ext = data["ext"].ToString().Remove(0, 1);
                pf.height = Convert.ToInt32(data["h"]);
                pf.width = Convert.ToInt32(data["w"]);
                pf.thumbW = Convert.ToInt32(data["tn_w"]);
                pf.thumbH = Convert.ToInt32(data["tn_h"]);
                pf.owner = Convert.ToInt32(data["no"]);
                pf.thumbnail_tim = data["tim"].ToString();
                pf.board = board;
                pf.hash = data["md5"].ToString();
                pf.size = Convert.ToInt32(data["fsize"]);
                if (data["spoiler"] != null)
                {
                    pf.IsSpoiler = (Convert.ToInt32(data["spoiler"]) == 1);
                }
                return pf;
            }
            else
            {
                return null;
            }
        }

        private static GenericPost ParseReply(JToken data, string board)
        {
            GenericPost t = new GenericPost();

            t.board = board;

            //comment
            if (data["com"] != null)
            {
                t.comment = data["com"].ToString();
            }
            else
            {
                t.comment = "";
            }

            //mail
            if (data["email"] != null)
            {
                t.email = HttpUtility.HtmlDecode(data["email"].ToString());
            }
            else
            {
                t.email = "";
            }

            //poster name
            if (data["name"] != null)
            {
                t.name = HttpUtility.HtmlDecode(data["name"].ToString());
            }
            else
            {
                t.name = "";
            }

            //subject
            if (data["sub"] != null)
            {
                t.subject = HttpUtility.HtmlDecode(data["sub"].ToString());
            }
            else
            {
                t.subject = "";
            }

            if (data["trip"] != null)
            {
                t.trip = data["trip"].ToString();
            }
            else
            {
                t.trip = "";
            }

            if (data["country"] != null)
            {
                t.country_flag = data["country"].ToString();
            }
            else
            {
                t.country_flag = "";
            }

            if (data["country_name"] != null)
            {
                t.country_name = data["country_name"].ToString();
            }
            else
            {
                t.country_name = "";
            }
            
            if (data["capcode"] != null)
            {
                t.capcode = parse_capcode(Convert.ToString(data["capcode"]));
            }

            t.file = ParseFile(data, board);

            t.PostNumber = Convert.ToInt32(data["no"]); ;

            t.time = Common.ParseUTC_Stamp(Convert.ToInt32(data["time"]));

            return t;
        }

        private static CatalogItem ParseJToken_Catalog(JToken thread, int pagenumber, string board)
        {
            GenericPost base_data = ParseReply(thread, board);

            CatalogItem ci = new CatalogItem(base_data);

            if (thread["last_replies"] != null)
            {
                JContainer li = (JContainer)thread["last_replies"];

                List<GenericPost> repl = new List<GenericPost>();

                foreach (Newtonsoft.Json.Linq.JObject j in li)
                {
                    repl.Add(ParseReply(j, board));
                }

                ci.trails = repl.ToArray();
            }

            if (thread["tag"] != null)
            {
                ci.Tag = parse_tag(Convert.ToString(thread["tag"]));
            }
            else { ci.Tag = GenericPost.ThreadTag.NoTag; }

            ci.image_replies = Convert.ToInt32(thread["images"]);
            ci.text_replies = Convert.ToInt32(thread["replies"]);
            ci.page_number = pagenumber;

            ci.board = board;

            return ci;
            /*
           "bumplimit": 0,
           "imagelimit": 0,
           "omitted_posts": 1,
           "omitted_images": 0,*/
        }

        private static GenericPost.ThreadTag parse_tag(string tag)
        {
            switch (tag)
            {
                case "Other":
                    return GenericPost.ThreadTag.Other;
                case "Anime":
                    return GenericPost.ThreadTag.Anime;
                case "Game":
                    return GenericPost.ThreadTag.Game;
                case "Hentai":
                    return GenericPost.ThreadTag.Hentai;
                case "Japanese":
                    return GenericPost.ThreadTag.Japanese;
                case "Loop":
                    return GenericPost.ThreadTag.Loop;
                case "Porn":
                    return GenericPost.ThreadTag.Porn;
                default:
                    return GenericPost.ThreadTag.Unknown;
            }
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

        private static APIResponse LoadAPI(string url)
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

        #endregion

        private static void delete_file(string s)
        {
            if (File.Exists(s)) { File.Delete(s); }
        }

        public static ReportStatus ReportPost(string board, int post_id, ReportReason reason, SolvedCaptcha captcha)
        {
            string url = String.Format(@"https://sys.4chan.org/{0}/imgboard.php?mode=report&no={1}", board.ToLower(), post_id.ToString());

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}={1}", "board", board.ToLower());

            sb.AppendFormat("&{0}={1}", "no", post_id.ToString());

            string report_cat = "vio";

            switch (reason)
            {
                case ReportReason.CommercialSpam:
                case ReportReason.Advertisement:
                    report_cat = "spam";
                    break;
                case ReportReason.IllegalContent:
                    report_cat = "illegal";
                    break;
                case ReportReason.RuleViolation:
                    report_cat = "vio";
                    break;
                default:
                    break;
            }

            sb.AppendFormat("&{0}={1}", "cat", report_cat);

            sb.AppendFormat("&{0}={1}", "recaptcha_response_field", captcha.ResponseField);

            sb.AppendFormat("&{0}={1}", "recaptcha_challenge_field", captcha.ChallengeField);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "POST";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Referer = url;

            request.ContentType = "application/x-www-form-urlencoded";

            using (var requestStream = request.GetRequestStream())
            {
                byte[] temp = Encoding.ASCII.GetBytes(sb.ToString());
                requestStream.Write(temp, 0, temp.Length);
            }

            string response_text;

            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var stream = new MemoryStream())
                    {
                        {
                            responseStream.CopyTo(stream);
                            response_text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }

            ReportStatus status = ReportStatus.Unkown;

            if (!String.IsNullOrEmpty(response_text))
            {
                response_text = response_text.ToLower();

                if (response_text.Contains("report submitted"))
                {
                    status = ReportStatus.Success;
                }

                if (response_text.Contains("you seem to have mistyped the captcha. please try again"))
                {
                    status = ReportStatus.Captcha;
                }

                if (response_text.Contains("that post doesn't exist anymore"))
                {
                    status = ReportStatus.PostGone;
                }

                if (response_text.Contains("banned"))
                {
                    status = ReportStatus.Banned;
                }
            }

            return status;
        }

        public enum ReportStatus { Success, Captcha, PostGone, Banned, Unkown }

        public enum ReportReason { RuleViolation, IllegalContent, CommercialSpam, Advertisement }

        public static DeleteStatus DeletePost(string board, int thread_id, int post_id, string password, bool file_only)
        {
            string url = string.Format(@"https://sys.4chan.org/{0}/imgboard.php", board);

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}={1}", "mode", "usrdel");

            sb.AppendFormat("&{0}={1}", "res", thread_id);

            sb.AppendFormat("&{0}={1}", "pwd", password);

            sb.AppendFormat("&{0}={1}", post_id, "delete");

            if (file_only)
            {
                sb.AppendFormat("&{0}={1}", "onlyimgdel", "on");
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:26.0) Gecko/20100101 Firefox/26.0";
            request.Referer = string.Format("http://boards.4chan.org/{0}/res/{1}", board, thread_id);

            request.ContentType = "application/x-www-form-urlencoded";

            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] temp = Encoding.ASCII.GetBytes(sb.ToString());
                requestStream.Write(temp, 0, temp.Length);
            }

            string response_text;

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        {
                            responseStream.CopyTo(stream);
                            response_text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }

            DeleteStatus status = DeleteStatus.Unkown;

            if (!String.IsNullOrEmpty(response_text))
            {
                response_text = response_text.ToLower();

                if (response_text.Contains("updating index...") ||
                    response_text.Contains("please delete posts less often!") ||
                    response_text.Contains(string.Format("<meta http-equiv=\"refresh\" content=\"0;url=http://boards.4chan.org/{0}/res/{1}#p{1}\">", board, thread_id)))
                {
                    status = DeleteStatus.Success;
                }

                if (response_text.Contains("error: password incorrect"))
                {
                    status = DeleteStatus.BadPassword;
                }

                if (response_text.Contains("please wait longer before deleting your post"))
                {
                    status = DeleteStatus.WaitLonger;
                }

                if (response_text.Contains("can't find the post"))
                {
                    status = DeleteStatus.PostGone;
                }

                if (response_text.Contains("4chan - banned"))
                {
                    status = DeleteStatus.Banned;
                }

                if (response_text.Contains("error: our system thinks your post is spam"))
                {
                    status = DeleteStatus.Spam;
                }

            }
            File.WriteAllText(Path.Combine(log_dir, string.Format("delete-post-{0}-{1}-{2}", board, thread_id, post_id)), response_text);
            return status;
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
