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

namespace Sandwich.Chans.RandomBoard
{
    public class RandomBoard : IChan
    {
        private string[] common_files = new string[] { "jpg", "png", "gif", "webm" };

        public string Name { get { return "Randomboard"; } }

        public string Description { get { return "JustRandom"; } }

        public string Key { get { return "rb"; } }

        private Regex url_validator = new Regex(@"randomboard\.org/[a-z]([a-z]?)([a-z]?)/(index|res/\d+|\d+|catalog)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Regex is_number = new Regex(@"\d+", RegexOptions.Compiled);

        public bool ValidateURL(string url)
        {
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
            /*
             http://[randomboard.org/b/res/379].html#379
             http://[randomboard.org/dev/index].html
             http://[randomboard.org/b/4].html
             http://[randomboard.org/b/catalog].html
            */
            string match = url_validator.Match(url).ToString();

            string[] data = match.Split('/');

            if (data[0] == "randomboard.org")
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
            return string.Format("http://randomboard.org/{0}/res/{1}.html", board, id);
        }

        public KeyValuePair<string, BoardInfo>[] GetBoards()
        {
            List<KeyValuePair<string, BoardInfo>> boards = new List<KeyValuePair<string, BoardInfo>>();

            boards.Add(new KeyValuePair<string, BoardInfo>("mlp", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "SHITPOSTING",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "mlp",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            boards.Add(new KeyValuePair<string, BoardInfo>("a", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Anime",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "a",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            boards.Add(new KeyValuePair<string, BoardInfo>("b", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Random",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "b",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));


            boards.Add(new KeyValuePair<string, BoardInfo>("c", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Cars",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "c",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            boards.Add(new KeyValuePair<string, BoardInfo>("d", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Drawing and Art",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "d",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            boards.Add(new KeyValuePair<string, BoardInfo>("g", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Vidya Gaemz",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "g",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));


            boards.Add(new KeyValuePair<string, BoardInfo>("m", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Music",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "m",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            boards.Add(new KeyValuePair<string, BoardInfo>("qt", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Feels",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "qt",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            boards.Add(new KeyValuePair<string, BoardInfo>("w", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Waifu Board",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "w",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            boards.Add(new KeyValuePair<string, BoardInfo>("dev", new BoardInfo()
            {
                AllowedFiles = common_files,
                Chan = this,
                Description = "Development",
                Icon = null,
                SpoilerImage = null,
                WorkSafe = false,
                HasSpoilers = false,
                Letter = "dev",
                MaximumFileSize = 3 * 1048576,
                ExtraFiles = null,
                Bumplimit = 300,
                PagesCount = 10,
                ImageLimit = 150
            }));

            return boards.ToArray();
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
            APIResponse response = Core.LoadAPI("http://randomboard.org/%/catalog.json".Replace("%", board));

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
            APIResponse response = Core.LoadAPI("http://randomboard.org/%/$.json".Replace("%", board).Replace("$", page.ToString()));

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
            APIResponse response = Core.LoadAPI(string.Format("http://randomboard.org/{0}/res/{1}.json", board, id));

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
                pf.ext = data["ext"].ToString().Remove(0, 1).ToLower();
                pf.height = Convert.ToInt32(data["h"]);
                pf.width = Convert.ToInt32(data["w"]);
                pf.thumbW = Convert.ToInt32(data["tn_w"]);
                pf.thumbH = Convert.ToInt32(data["tn_h"]);
                pf.owner = Convert.ToInt32(data["no"]);
                pf.thumbnail_tim = data["tim"].ToString();
                pf.board = board;
                //pf.hash = data["md5"].ToString();
                pf.size = Convert.ToInt32(data["fsize"]);

                pf.ThumbLink = string.Format("http://randomboard.org/{0}/thumb/{1}.{2}", board, pf.thumbnail_tim, pf.ext);

                pf.FullImageLink = string.Format("http://randomboard.org/{0}/src/{1}.{2}", board, pf.thumbnail_tim, pf.ext);

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
            return Core.DeleteStatus.Unkown;
            //            string url = string.Format(@"https://sys.4chan.org/{0}/imgboard.php", board);

            //            StringBuilder sb = new StringBuilder();

            //            sb.AppendFormat("{0}={1}", "mode", "usrdel");

            //            sb.AppendFormat("&{0}={1}", "res", thread_id);

            //            sb.AppendFormat("&{0}={1}", "pwd", password);

            //            sb.AppendFormat("&{0}={1}", post_id, "delete");

            //            if (file_only)
            //            {
            //                sb.AppendFormat("&{0}={1}", "onlyimgdel", "on");
            //            }

            //            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            //            request.Method = "POST";
            //            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:26.0) Gecko/20100101 Firefox/26.0";
            //            request.Referer = string.Format("http://boards.4chan.org/{0}/res/{1}", board, thread_id);

            //            request.ContentType = "application/x-www-form-urlencoded";

            //            using (Stream requestStream = request.GetRequestStream())
            //            {
            //                byte[] temp = Encoding.ASCII.GetBytes(sb.ToString());
            //                requestStream.Write(temp, 0, temp.Length);
            //            }

            //            string response_text;

            //            using (WebResponse response = request.GetResponse())
            //            {
            //                using (Stream responseStream = response.GetResponseStream())
            //                {
            //                    using (MemoryStream stream = new MemoryStream())
            //                    {
            //                        {
            //                            responseStream.CopyTo(stream);
            //                            response_text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            //                        }
            //                    }
            //                }
            //            }

            //            Core.DeleteStatus status = Core.DeleteStatus.Unkown;

            //            if (!String.IsNullOrEmpty(response_text))
            //            {
            //                response_text = response_text.ToLower();

            //                if (response_text.Contains("updating index...") ||
            //                    response_text.Contains("please delete posts less often!") ||
            //                    response_text.Contains(string.Format("<meta http-equiv=\"refresh\" content=\"0;url=http://boards.4chan.org/{0}/res/{1}#p{1}\">", board, thread_id)))
            //                {
            //                    status = Core.DeleteStatus.Success;
            //                }

            //                if (response_text.Contains("error: password incorrect"))
            //                {
            //                    status = Core.DeleteStatus.BadPassword;
            //                }

            //                if (response_text.Contains("please wait longer before deleting your post"))
            //                {
            //                    status = Core.DeleteStatus.WaitLonger;
            //                }

            //                if (response_text.Contains("can't find the post"))
            //                {
            //                    status = Core.DeleteStatus.PostGone;
            //                }

            //                if (response_text.Contains("4chan - banned"))
            //                {
            //                    status = Core.DeleteStatus.Banned;
            //                }

            //                if (response_text.Contains("error: our system thinks your post is spam"))
            //                {
            //                    status = Core.DeleteStatus.Spam;
            //                }

            //            }

            //#if DEBUG
            //            File.WriteAllText(Path.Combine(Core.log_dir, string.Format("delete-post-{0}-{1}-{2}", board, thread_id, post_id)), response_text);
            //#endif

            //            return status;
        }

        public Core.ReportStatus ReportPost(string board, int post_id, Core.ReportReason reason, SolvedCaptcha captcha)
        {
            return Core.ReportStatus.Unkown;
            //string url = String.Format(@"https://sys.4chan.org/{0}/imgboard.php?mode=report&no={1}", board.ToLower(), post_id.ToString());

            //StringBuilder sb = new StringBuilder();

            //sb.AppendFormat("{0}={1}", "board", board.ToLower());

            //sb.AppendFormat("&{0}={1}", "no", post_id.ToString());

            //string report_cat = "vio";

            //switch (reason)
            //{
            //    case Core.ReportReason.CommercialSpam:
            //    case Core.ReportReason.Advertisement:
            //        report_cat = "spam";
            //        break;
            //    case Core.ReportReason.IllegalContent:
            //        report_cat = "illegal";
            //        break;
            //    case Core.ReportReason.RuleViolation:
            //        report_cat = "vio";
            //        break;
            //    default:
            //        break;
            //}

            //sb.AppendFormat("&{0}={1}", "cat", report_cat);

            //sb.AppendFormat("&{0}={1}", "recaptcha_response_field", captcha.ResponseField);

            //sb.AppendFormat("&{0}={1}", "recaptcha_challenge_field", captcha.ChallengeField);

            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            //request.Method = "POST";
            //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            //request.Referer = url;

            //request.ContentType = "application/x-www-form-urlencoded";

            //using (var requestStream = request.GetRequestStream())
            //{
            //    byte[] temp = Encoding.ASCII.GetBytes(sb.ToString());
            //    requestStream.Write(temp, 0, temp.Length);
            //}

            //string response_text;

            //using (var response = request.GetResponse())
            //{
            //    using (var responseStream = response.GetResponseStream())
            //    {
            //        using (var stream = new MemoryStream())
            //        {
            //            {
            //                responseStream.CopyTo(stream);
            //                response_text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            //            }
            //        }
            //    }
            //}

            //Core.ReportStatus status = Core.ReportStatus.Unkown;

            //if (!String.IsNullOrEmpty(response_text))
            //{
            //    response_text = response_text.ToLower();

            //    if (response_text.Contains("report submitted"))
            //    {
            //        status = Core.ReportStatus.Success;
            //    }

            //    if (response_text.Contains("you seem to have mistyped the captcha. please try again"))
            //    {
            //        status = Core.ReportStatus.Captcha;
            //    }

            //    if (response_text.Contains("that post doesn't exist anymore"))
            //    {
            //        status = Core.ReportStatus.PostGone;
            //    }

            //    if (response_text.Contains("banned"))
            //    {
            //        status = Core.ReportStatus.Banned;
            //    }
            //}

            //return status;
        }

        public IPostSenderResponse SendPost(RequestPosterData data)
        {

            string template_url = "";

            if (data.IsReply)
            {
                template_url = this.GetThreadURL(data.Board.Letter, data.ThreadID);
            }
            else
            {
                throw new NotImplementedException();
                template_url = string.Format("http://randomboard.org/{0}/index.html", data.Board.Letter);
            }


            NameValueCollection form_values = new NameValueCollection();
            //{
            //    var doc = new HtmlAgilityPack.HtmlDocument();

            //    var http_data = Core.GetHttpResponse(template_url, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)", "");



            //    if (string.IsNullOrEmpty(http_data.Value))
            //    {
            //        return null;
            //    }

            //    string path = @"C:\Users\Istan\Desktop\rb_1.html";
            //    System.IO.File.WriteAllText(path, http_data.Value);

            //    doc.LoadHtml(http_data.Value);

            //    var input_elements = doc.DocumentNode.SelectNodes("//input");

            //    var aaa = new Dictionary<int, string>();

            //    foreach (var q in input_elements) { aaa.Add(input_elements.IndexOf(q), q.OuterHtml); }
                
            //    int[] indexes = { 0, 1, 2, 3, 4, 5, 6,7,8,9,10,11,12 };

            //    foreach (int i in indexes)
            //    {
            //        HtmlAgilityPack.HtmlNode node = (HtmlAgilityPack.HtmlNode)input_elements[i];
                    
            //        string name = node.GetAttributeValue("name", "");
            //        string value = Common.DecodeHTML(node.GetAttributeValue("value", ""));
            //        form_values.Add(name, value);
            //    }

            //    form_values.Add("body", data.Comment);
            //    form_values.Add("post", data.IsReply ? "New Reply" : "");

            //    //Disable the embed link
            //    form_values.Add("embed", "");

            //}

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://randomboard.org/post.php");
            request.Method = "POST";
            request.Headers.Add("x-ebin", "e");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:26.0) Gecko/20100101 Firefox/26.0";
            //request.Referer = template_url;

            request.AllowAutoRedirect = true;



            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", System.Globalization.NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;
           
            using (Stream requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in form_values.Keys)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(form_values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                var files = data.GetFiles();

                if (files.Length == 0)
                {
                    files = new UploadFile[1] { Core.GetEmptyFile("file") };
                }

                for (int index = 0; index < files.Length; index++)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", files[index].Name, files[index].Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Type: {0}{1}{1}", files[index].ContentType, Environment.NewLine));
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

                    buffer = Encoding.UTF8.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                byte[] boundaryBuffer = Encoding.UTF8.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            //response text
            string response_text = "";

            try
            {
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
            }
            catch (WebException wex) 
            {
                return null;
            }
            catch (Exception exx)
            {
                return null;
            }

            if (!String.IsNullOrEmpty(response_text))
            {
                string path = @"C:\Users\Istan\Desktop\rb.html";
                System.IO.File.WriteAllText(path, response_text);
                return null;
            }
            else
            {
                return null;
            }
        }

        public bool RequireCaptcha { get { return false; } }

        public BitmapImage GetBoardSpoilerImage(BoardInfo board)
        {
            return null;
        }




    }
}
