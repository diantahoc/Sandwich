using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sandwich.DataTypes;
using System.Windows.Media.Imaging;

namespace Sandwich.Chans._4chan
{
    public class _4chan : IChan
    {
        private string[] common_files = new string[] { "jpg", "png", "gif", "webm" };

        private BitmapImage sfw_normal;
        private BitmapImage nsfw_normal;

        public _4chan()
        {
            sfw_normal = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/icons/sfw-normal.png", UriKind.Absolute));
            sfw_normal.Freeze();

            nsfw_normal = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/icons/nsfw-normal.png", UriKind.Absolute));
            nsfw_normal.Freeze();
        }

        public string Name { get { return "4chan"; } }

        public string Description { get { return "4chan is a simple image-based bulletin board where anyone can post comments and share images."; } }

        public string Key { get { return "4chan"; } }

        private Regex url_validator = new Regex(@"boards\.4chan\.org/([a-z]?[a-z]?[a-z])(/?)(catalog|(\d+)|((thread|res)/(\d)+)|)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Regex is_number = new Regex(@"\d+", RegexOptions.Compiled);

        public bool ValidateURL(string url)
        {
            //http://boards.4chan.org/g/thread/42251884
            //http://boards.4chan.org/g/thread/42254199/thinking-of-buying-of-these-because-mindless
            return url_validator.IsMatch(url);
        }

        public BoardInfo ValidateBoard(string s)
        {
            if (s.Length > 3)
            {
                return null;
            }
            else
            {
                var boards = this.GetBoards();

                foreach (var board in boards)
                {
                    if (s == board.Key)
                    {
                        return board.Value;
                    }
                }

                return null;
            }
        }

        public URLParserResults ParseURL(string url)
        {
            string match = url_validator.Match(url).ToString();

            string[] data = match.Split('/');

            if (data[0] == "boards.4chan.org")
            {
                string board_l = data[1];

                BoardInfo board = ValidateBoard(board_l);

                if (board == null)
                {
                    return new URLParserResults()
                    {
                        Action = URLParserResults.ActionType.Unkown
                    };
                }

                if (data.Length == 2)
                {
                    // url = boards.4chan.org/g
                    return new URLParserResults()
                    {
                        Action = URLParserResults.ActionType.HomePage,
                        Board = board
                    };
                }
                else if (data.Length == 3)
                {
                    /*
                     boards.4chan.org/g/catalog
                     boards.4chan.org/g/
                     boards.4chan.org/g/10 
                    */

                    if (data[2] == "catalog")
                    {
                        return new URLParserResults()
                        {
                            Action = URLParserResults.ActionType.Catalog,
                            Board = board
                        };
                    }
                    else if (data[2] == "" || is_number.IsMatch(data[2]))
                    {
                        return new URLParserResults()
                        {
                            Action = URLParserResults.ActionType.Page,
                            Board = board,
                            PageNumber = is_number.IsMatch(data[2]) ? Int32.Parse(data[2]) : 1
                        };
                    }
                }
                else if (data.Length == 4)
                {
                    if (data[2] == "thread" || data[2] == "res")
                    {
                        int tid = -1;
                        Int32.TryParse(data[3], out tid);
                        if (tid > 0)
                        {
                            return new URLParserResults()
                            {
                                Action = URLParserResults.ActionType.Thread,
                                Board = board,
                                ThreadID = tid
                            };
                        }
                        else
                        {
                            return new URLParserResults()
                            {
                                Action = URLParserResults.ActionType.Unkown
                            };
                        }
                    }
                    else
                    {
                        return new URLParserResults()
                        {
                            Action = URLParserResults.ActionType.Unkown
                        };
                    }
                }
            }

            return new URLParserResults()
            {
                Action = URLParserResults.ActionType.Unkown
            };


        }

        public string GetThreadURL(string board, int id)
        {
            return string.Format("http://boards.4chan.org/{0}/thread/{1}", board, id);
        }

        public KeyValuePair<string, BoardInfo>[] GetBoards()
        {
            APIResponse api_r = Core.LoadAPI("http://a.4cdn.org/boards.json");

            if (api_r.Error == APIResponse.ErrorType.NoError)
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(api_r.Data);

                JArray boards = (JArray)json["boards"];

                List<KeyValuePair<string, BoardInfo>> il = new List<KeyValuePair<string, BoardInfo>>();

                for (int i = 0; i < boards.Count; i++)
                {
                    JToken board = boards[i];

                    BoardInfo bi = new BoardInfo()
                    {
                        Chan = this,
                        Letter = Convert.ToString(board["board"]),
                        Description = Convert.ToString(board["title"]),
                        AllowedFiles = this.common_files,
                        WorkSafe = Convert.ToBoolean(board["ws_board"]),
                        PagesCount = Convert.ToInt32(board["pages"]),
                        MaximumFileSize = get_board_max_file_size(Convert.ToString(board["board"])),
                    };

                    if (bi.WorkSafe)
                    {
                        bi.Icon = this.sfw_normal;
                    }
                    else
                    {
                        bi.Icon = this.nsfw_normal;
                    }

                    if (bi.Letter == "f")
                    {
                        bi.AllowedFiles = new string[] { "swf" };
                    }

                    if (bi.Letter == "gd" || bi.Letter == "tg" || bi.Letter == "po")
                    {
                        bi.ExtraFiles = new string[] { "pdf" };
                    }

                    bi.HasSpoilers = bi.Letter == "a" || bi.Letter == "co" || bi.Letter == "jp" || bi.Letter == "m" || bi.Letter == "mlp"
                                        || bi.Letter == "tg" || bi.Letter == "tv" || bi.Letter == "v" || bi.Letter == "vg" || bi.Letter == "vp";

                    if (bi.HasSpoilers)
                    {
                        bi.SpoilerImage = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/spoiler_images/spoiler-" + bi.Letter + "1.png", UriKind.Absolute));
                        bi.SpoilerImage.Freeze();
                    }

                    if (bi.Letter == "a" || bi.Letter == "b")
                    {
                        bi.Bumplimit = 500;
                    }
                    else
                    {
                        bi.Bumplimit = 300;
                    }

                    bi.ImageLimit = 151;

                    il.Add(new KeyValuePair<string, BoardInfo>(bi.Letter, bi));
                }
                return il.ToArray();
            }
            else
            {
                return new KeyValuePair<string, BoardInfo>[0];
            }
        }

        private int get_board_max_file_size(string board)
        {
            if (board == "b" || board == "s4s" || board == "r9k") { return 2 * 1048576; }

            if (board == "gd" || board == "hm" || board == "hr" || board == "po" || board == "r" || board == "s" || board == "trv" || board == "tg") { return 8 * 1048576; }

            if (board == "out" || board == "p" || board == "w" || board == "wg") { return 5 * 1048576; }

            if (board == "gif" || board == "soc" || board == "sp" || board == "wsg") { return 4 * 1048576; }

            return 3 * 1048576;
        }

        public bool HasArchives { get { return false; } }

        public IChanArchiveDataProvider ArchiveDataProvider
        {
            get
            {
                return null;
            }
        }

        public CatalogItem[] GetCatalog(string board)
        {
            APIResponse response = Core.LoadAPI("http://a.4cdn.org/%/catalog.json".Replace("%", board));

            switch (response.Error)
            {
                case APIResponse.ErrorType.NoError:

                    List<CatalogItem> il = new List<CatalogItem>();

                    JArray list = JsonConvert.DeserializeObject<JArray>(response.Data);
                    //p is page index
                    //u is thread index
                    for (int p = 0; p < list.Count(); p++)
                    {
                        JToken page = list[p];

                        JArray threads = (JArray)page["threads"];

                        for (int u = 0; u < threads.Count; u++)
                        {
                            JToken thread = threads[u];
                            il.Add(ParseJToken_Catalog(thread, p, board));
                        }
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

        public ThreadContainer[] GetPage(string board, int page)
        {
            APIResponse response = Core.LoadAPI("http://api.4chan.org/%/$.json".Replace("%", board).Replace("$", page.ToString()));

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

        public ThreadContainer GetThread(string board, int id)
        {
            APIResponse response = Core.LoadAPI(string.Format("http://a.4cdn.org/{0}/res/{1}.json", board, id));

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

        private Thread ParseThread(JToken data, string board)
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

        private GenericPost.Capcode parse_capcode(string cap)
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

        private PostFile ParseFile(JToken data, string board)
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

                if (board == "f")
                {
                    pf.ThumbLink = string.Empty;
                    pf.FullImageLink = string.Format("http://i.4cdn.org/{0}/{1}.{2}", board, pf.filename, pf.ext);
                }
                else
                {
                    pf.ThumbLink = string.Format("http://t.4cdn.org/{0}/{1}s.jpg", board, pf.thumbnail_tim);
                    pf.FullImageLink = string.Format("http://i.4cdn.org/{0}/{1}.{2}", board, pf.thumbnail_tim, pf.ext);
                }

                return pf;
            }
            else
            {
                return null;
            }
        }

        private GenericPost ParseReply(JToken data, string board)
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

        private CatalogItem ParseJToken_Catalog(JToken thread, int pagenumber, string board)
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

        private GenericPost.ThreadTag parse_tag(string tag)
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

        public Core.DeleteStatus DeletePost(string board, int thread_id, int post_id, string password, bool file_only)
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

            Core.DeleteStatus status = Core.DeleteStatus.Unkown;

            if (!String.IsNullOrEmpty(response_text))
            {
                response_text = response_text.ToLower();

                if (response_text.Contains("updating index...") ||
                    response_text.Contains("please delete posts less often!") ||
                    response_text.Contains(string.Format("<meta http-equiv=\"refresh\" content=\"0;url=http://boards.4chan.org/{0}/res/{1}#p{1}\">", board, thread_id)))
                {
                    status = Core.DeleteStatus.Success;
                }

                if (response_text.Contains("error: password incorrect"))
                {
                    status = Core.DeleteStatus.BadPassword;
                }

                if (response_text.Contains("please wait longer before deleting your post"))
                {
                    status = Core.DeleteStatus.WaitLonger;
                }

                if (response_text.Contains("can't find the post"))
                {
                    status = Core.DeleteStatus.PostGone;
                }

                if (response_text.Contains("4chan - banned"))
                {
                    status = Core.DeleteStatus.Banned;
                }

                if (response_text.Contains("error: our system thinks your post is spam"))
                {
                    status = Core.DeleteStatus.Spam;
                }

            }

#if DEBUG
            File.WriteAllText(Path.Combine(Core.log_dir, string.Format("delete-post-{0}-{1}-{2}", board, thread_id, post_id)), response_text);
#endif

            return status;
        }

        public Core.ReportStatus ReportPost(string board, int post_id, Core.ReportReason reason, SolvedCaptcha captcha)
        {
            string url = String.Format(@"https://sys.4chan.org/{0}/imgboard.php?mode=report&no={1}", board.ToLower(), post_id.ToString());

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}={1}", "board", board.ToLower());

            sb.AppendFormat("&{0}={1}", "no", post_id.ToString());

            string report_cat = "vio";

            switch (reason)
            {
                case Core.ReportReason.CommercialSpam:
                case Core.ReportReason.Advertisement:
                    report_cat = "spam";
                    break;
                case Core.ReportReason.IllegalContent:
                    report_cat = "illegal";
                    break;
                case Core.ReportReason.RuleViolation:
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

            Core.ReportStatus status = Core.ReportStatus.Unkown;

            if (!String.IsNullOrEmpty(response_text))
            {
                response_text = response_text.ToLower();

                if (response_text.Contains("report submitted"))
                {
                    status = Core.ReportStatus.Success;
                }

                if (response_text.Contains("you seem to have mistyped the captcha. please try again"))
                {
                    status = Core.ReportStatus.Captcha;
                }

                if (response_text.Contains("that post doesn't exist anymore"))
                {
                    status = Core.ReportStatus.PostGone;
                }

                if (response_text.Contains("banned"))
                {
                    status = Core.ReportStatus.Banned;
                }
            }

            return status;
        }

        public IPostSenderResponse SendPost(RequestPosterData data)
        {

            NameValueCollection values = new NameValueCollection();

            values.Add("MAX_FILE_SIZE", data.Board.MaximumFileSize.ToString());

            values.Add("mode", "regist");

            if (data.IsReply)
            {
                values.Add("resto", data.ThreadID.ToString());
            }

            values.Add("name", data.Name);
            values.Add("email", data.Email);
            values.Add("sub", data.Subject);
            values.Add("com", data.Comment);

            values.Add("pwd", data.PostPassword);

            //Check if the user is using a 4chan pass (is_pass is a boolean).
            //moot remove the captcha when a pass is availble, so recaptcha fields are not needed.
            if (data.CaptchaRequired)
            {
                values.Add("recaptcha_response_field", data.Captcha.ResponseField);
                values.Add("recaptcha_challenge_field", data.Captcha.ChallengeField);
            }


            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://sys.4chan.org/{0}/post", data.Board.Letter));
            request.Method = "POST";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Referer = "http://boards.4chan.org/" + data.Board.Letter;

            //This is the part we encode data using "multipart/form-data" technique 
            //RFC 1867 : http://tools.ietf.org/html/rfc1867#section-6

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", System.Globalization.NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                var files = data.GetFiles();

                if (files.Length == 0)
                {
                    files = new UploadFile[1] { Core.GetEmptyFile("upfile") };
                }

                for (int index = 0; index < files.Length; index++)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", files[index].Name, files[index].Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", files[index].ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);

                    if (files[index].Stream == null)
                    {
                        Stream.Null.CopyTo(requestStream);
                    }
                    else
                    {
                        files[index].Stream.CopyTo(requestStream);
                        files[index].Stream.Dispose();
                    }

                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                byte[] boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            //response text
            string response_text = "";

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        {
                            responseStream.CopyTo(stream);
                            response_text = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(response_text))
            {
                return new _4chanPostSenderResponse(response_text);
            }
            else
            {
                return null;
            }

        }

        public bool RequireCaptcha { get { return true; } }

        private Dictionary<string, BitmapImage> spoiler_cache = new Dictionary<string, BitmapImage>();

        public BitmapImage GetBoardSpoilerImage(BoardInfo board)
        {
            if (spoiler_cache.ContainsKey(board.Letter))
            {
                return spoiler_cache[board.Letter];
            }
            else
            {
                BitmapImage i = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/spoiler_images/spoiler-" + board.Letter + "1.png", UriKind.Absolute));
                i.Freeze();
                spoiler_cache.Add(board.Letter, i);
                return i;
            }
        }

        //public static CatalogItem[][] GetCatalogNoAPI(string board)
        //{
        //    APIResponse response = LoadAPI(string.Format("http://boards.4chan.org/{0}/catalog", board));

        //    switch (response.Error)
        //    {
        //        case APIResponse.ErrorType.NoError:
        //            return new CatalogItem[][] { Parse_Catalog_Page_HTML(response.Data) };
        //        case APIResponse.ErrorType.NotFound:
        //            throw new Exception("404");
        //        case APIResponse.ErrorType.Other:
        //            throw new Exception(response.Data);
        //        default:
        //            return null;
        //    }
        //}

        //private static CatalogItem[] Parse_Catalog_Page_HTML(string data)
        //{
        //    string js_data_xpath = "/html[1]/head[1]/script[3]";

        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

        //    doc.LoadHtml(data);

        //    string js_data = doc.DocumentNode.SelectSingleNode(js_data_xpath).InnerHtml;

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
        //                        name = HttpUtility.HtmlDecode(Convert.ToString(thread["lr"]["author"]))
        //                    }
        //                };
        //            }
        //        }

        //        ci.name = HttpUtility.HtmlDecode(Convert.ToString(thread["author"]));
        //        ci.subject = HttpUtility.HtmlDecode(Convert.ToString(thread["sub"]));

        //        ci.comment = Common.DecodeHTML(Convert.ToString(thread["teaser"]));

        //        if (thread["trip"] != null)
        //        {
        //            ci.trip = HttpUtility.HtmlDecode(Convert.ToString(thread["trip"]));
        //        }
        //        else { ci.trip = ""; }

        //        if (thread["capcode"] != null)
        //        {
        //            ci.capcode = parse_capcode(Convert.ToString(thread["capcode"]));
        //        }

        //        if (thread["sticky"] != null)
        //        {
        //            ci.IsSticky = (Convert.ToInt32(thread["sticky"]) == 1);
        //        }

        //        if (thread["closed"] != null)
        //        {
        //            ci.IsClosed = (Convert.ToInt32(thread["closed"]) == 1);
        //        }

        //        ci.email = "";

        //        if (thread["imgurl"] != null)
        //        {
        //            PostFile p = new PostFile();
        //            p.board = ci.board;
        //            p.owner = ci.PostNumber;
        //            p.thumbW = Convert.ToInt32(thread["tn_w"]);
        //            p.thumbH = Convert.ToInt32(thread["tn_h"]);
        //            p.thumbnail_tim = Convert.ToString(thread["imgurl"]);
        //            ci.file = p;
        //        }

        //        threads.Add(ci);
        //    }
        //    return threads.ToArray();
        //}

    }
}
