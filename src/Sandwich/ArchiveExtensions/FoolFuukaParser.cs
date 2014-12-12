using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.IO.Compression;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Sandwich.ArchiveExtensions
{
    public class FoolFuukaParser
    {
        private static string get_data(string url)
        {
            using (System.Net.WebClient wv = new System.Net.WebClient())
            {
                wv.Headers.Add("user-agent", "Mozilla/18.0 (compatible; MSIE 10.0; Windows NT 5.2; .NET CLR 3.5.3705;)");

                byte[] data = wv.DownloadData(url);

                try
                {
                    byte[] un_data = Decompress(data);

                    return System.Text.Encoding.UTF8.GetString(un_data);
                }
                catch (Exception)
                {
                    return System.Text.Encoding.UTF8.GetString(data);
                }
            }
        }

        private static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public static ThreadContainer Parse(Archive archive, string board, int threadID)
        {
            try
            {
                return parse_ffuuka_json(archive, board, threadID);
            }
            catch (Exception ex)
            {
                if (ex.Message == "404")
                {
                    return parse_html(archive, board, threadID);
                }
                else
                {
                    throw;
                }
            }
        }

        private static ThreadContainer parse_html(Archive archive, string board, int threadID)
        {
            ThreadContainer tc = null;

            HtmlDocument doc = new HtmlDocument();

            // doc.LoadHtml(File.ReadAllText(@"C:\Users\Istan\Desktop\foolfuuka.html"));

            doc.LoadHtml(get_data(string.Format(archive.ThreadUrl, board, threadID)));

            /* Archive fake = new Archive();

             fake.SupportedBoards = new string[] { "a" };
             fake.SupportedFullImages = new string[] { "a" };
             fake.Type = Archive.ArchiveType.FoolFuuka;
             threadID = 1981461;*/

            //Get the article node and parse it

            HtmlNode article_node = null;

            foreach (HtmlNode a in doc.DocumentNode.ChildNodes)
            {
                if (a.Name == "html")
                {
                    foreach (HtmlNode b in a.ChildNodes)
                    {
                        if (b.Name == "body")
                        {
                            foreach (HtmlNode c in b.ChildNodes)
                            {
                                if (c.GetAttributeValue("class", "") == "container-fluid")
                                {
                                    foreach (HtmlNode d in c.ChildNodes)
                                    {
                                        if (d.Id == "main")
                                        {
                                            foreach (HtmlNode e in d.ChildNodes)
                                            {
                                                if (e.Id == threadID.ToString())
                                                {
                                                    article_node = e;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            tc = new ThreadContainer(parse_op(article_node, archive, board));

            foreach (HtmlNode aside in article_node.ChildNodes)
            {
                if (aside.Name == "aside")
                {
                    foreach (HtmlNode arct in aside.ChildNodes)
                    {
                        if (arct.Name == "article")
                        {
                            tc.AddReply(parse_reply(arct, archive, board));
                        }
                    }
                }
            }



            return tc;
        }

        private static Thread parse_op(HtmlNode node, Archive archive, string board)
        {
            Thread t = new Thread();

            t.PostNumber = Convert.ToInt32(node.Id);

            t.type = GenericPost.PostType.FoolFuuka;

            t.board = board;

            //Get file info

            foreach (HtmlNode file_info in node.ChildNodes)
            {
                if (file_info.GetAttributeValue("class", "") == "thread_image_box")
                {
                    PostFile pf = new PostFile();

                    foreach (HtmlNode tok in file_info.ChildNodes)
                    {

                        switch (tok.GetAttributeValue("class", ""))
                        {
                            case "thread_image_link":
                                //Setup thumbnail first
                                foreach (HtmlNode thumb_src in tok.ChildNodes)
                                {
                                    if (thumb_src.GetAttributeValue("class", "") == "thread_image")
                                    {
                                        pf.hash = thumb_src.GetAttributeValue("data-md5", "");

                                        pf.ThumbLink = thumb_src.GetAttributeValue("src", "");

                                        pf.thumbnail_tim = pf.ThumbLink.Split('/').Last().Split('.')[0].Replace("s", "");

                                        pf.thumbH = Convert.ToInt32(thumb_src.GetAttributeValue("height", ""));
                                        pf.thumbW = Convert.ToInt32(thumb_src.GetAttributeValue("width", ""));
                                    }

                                }


                                if (archive.IsFileSupported(board))
                                {
                                    if (tok.GetAttributeValue("href", "") != "")
                                    {
                                        pf.FullImageLink = tok.GetAttributeValue("href", "");
                                        pf.ext = pf.FullImageLink.Split('/').Last().Split('.')[1];
                                    }
                                    else
                                    {
                                        pf.FullImageLink = "nofile";
                                        pf.ext = pf.ThumbLink.Split('/').Last().Split('.')[1];
                                    }
                                }
                                else
                                {
                                    pf.FullImageLink = "nofile";
                                    pf.ext = pf.ThumbLink.Split('/').Last().Split('.')[1];
                                }
                                break;
                            case "post_file":
                                string[] data = tok.InnerText.Trim().Split(',');

                                pf.size = parse_file_size(data[0].Trim());
                                pf.height = Convert.ToInt32(data[1].Split('x')[0].Trim());
                                pf.width = Convert.ToInt32(data[1].Split('x')[1].Trim());

                                pf.filename = data[2].Trim().Split('.')[0];

                                break;

                            default:
                                break;
                        }

                    }

                    pf.board = board;
                    pf.owner = t.PostNumber;

                    t.file = pf;
                }

                if (file_info.Name == "header")
                {
                    foreach (HtmlNode post_data in file_info.ChildNodes)
                    {
                        if (post_data.GetAttributeValue("class", "") == "post_data")
                        {
                            foreach (HtmlNode sub_n in post_data.ChildNodes)
                            {
                                switch (sub_n.GetAttributeValue("class", ""))
                                {
                                    case "post_title":
                                        t.subject = System.Web.HttpUtility.HtmlDecode(sub_n.InnerText);
                                        break;
                                    case "post_poster_data":
                                        foreach (HtmlNode z in sub_n.ChildNodes)
                                        {
                                            switch (z.GetAttributeValue("class", ""))
                                            {
                                                case "post_author":
                                                    t.name = System.Web.HttpUtility.HtmlDecode(z.InnerText);
                                                    break;
                                                case "post_tripcode":
                                                    t.trip = System.Web.HttpUtility.HtmlDecode(z.InnerText);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "time_wrap":
                                        foreach (HtmlNode aaa in sub_n.ChildNodes)
                                        {
                                            if (aaa.Name == "time")
                                            {
                                                t.time = System.Xml.XmlConvert.ToDateTime(aaa.GetAttributeValue("datetime", ""), System.Xml.XmlDateTimeSerializationMode.Utc);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                if (file_info.GetAttributeValue("class", "") == "text")
                {
                    t.comment = file_info.InnerHtml;
                }
            }

            return t;
        }

        private static GenericPost parse_reply(HtmlNode article_node, Archive archive, string board)
        {
            GenericPost gp = new GenericPost();

            gp.type = GenericPost.PostType.FoolFuuka;

            gp.PostNumber = Convert.ToInt32(article_node.Id);

            gp.board = board;

            bool has_image = article_node.GetAttributeValue("class", "").Contains("has_image");

            PostFile pf = null;

            if (has_image)
            {
                pf = new PostFile();
            }

            foreach (HtmlNode post_wrapper in article_node.ChildNodes)
            {
                if (post_wrapper.GetAttributeValue("class", "") == "post_wrapper")
                {
                    foreach (HtmlNode file_info in post_wrapper.ChildNodes)
                    {
                        if (has_image)
                        {
                            switch (file_info.GetAttributeValue("class", ""))
                            {
                                case "thread_image_box":

                                    //Setup thumbnail first

                                    foreach (HtmlNode anchor in file_info.ChildNodes)
                                    {
                                        if (anchor.Name == "a")
                                        {

                                            foreach (HtmlNode thumb_src in anchor.ChildNodes)
                                            {

                                                if (thumb_src.GetAttributeValue("class", "").Contains("post_image"))
                                                {
                                                    pf.hash = thumb_src.GetAttributeValue("data-md5", "");

                                                    pf.ThumbLink = thumb_src.GetAttributeValue("src", "");

                                                    pf.thumbnail_tim = pf.ThumbLink.Split('/').Last().Split('.')[0].Replace("s", "");

                                                    pf.thumbH = Convert.ToInt32(thumb_src.GetAttributeValue("height", ""));
                                                    pf.thumbW = Convert.ToInt32(thumb_src.GetAttributeValue("width", ""));
                                                }
                                            }

                                            if (archive.IsFileSupported(board))
                                            {
                                                if (anchor.GetAttributeValue("href", "") != "")
                                                {
                                                    pf.FullImageLink = anchor.GetAttributeValue("href", "");
                                                    pf.ext = pf.FullImageLink.Split('/').Last().Split('.')[1];
                                                }
                                                else
                                                {
                                                    pf.FullImageLink = "nofile";
                                                    pf.ext = pf.ThumbLink.Split('/').Last().Split('.')[1];
                                                }
                                            }
                                            else
                                            {
                                                pf.FullImageLink = "nofile";
                                                pf.ext = pf.ThumbLink.Split('/').Last().Split('.')[1];
                                            }
                                        }
                                    }

                                    break;

                                case "post_file":
                                    foreach (HtmlNode nnnnn in file_info.ChildNodes)
                                    {
                                        if (nnnnn.GetAttributeValue("class", "") == "post_file_metadata")
                                        {
                                            string[] meta_data = nnnnn.InnerText.Trim().Split(',');

                                            pf.size = parse_file_size(meta_data[0].Trim());
                                            pf.height = Convert.ToInt32(meta_data[1].Split('x')[0].Trim());
                                            pf.width = Convert.ToInt32(meta_data[1].Split('x')[1].Trim());
                                        }

                                        if (nnnnn.GetAttributeValue("class", "") == "post_file_filename")
                                        {
                                            pf.filename = nnnnn.GetAttributeValue("title", "").Split('.').First(); ;
                                        }

                                        if (archive.Name == "4plebs")
                                        {
                                            if (nnnnn.Name == "#text" && (nnnnn.NextSibling != null) && (nnnnn.PreviousSibling != null))
                                            {
                                                pf.filename = System.Web.HttpUtility.HtmlDecode(nnnnn.InnerText.Trim()).Split('.').First();
                                            }
                                        }
                                    }

                                    // pf.filename = data[2].Trim().Split('.')[0];

                                    break;

                                default:
                                    break;
                            }


                        }

                        if (file_info.Name == "header")
                        {
                            foreach (HtmlNode post_data in file_info.ChildNodes)
                            {
                                if (post_data.GetAttributeValue("class", "") == "post_data")
                                {
                                    foreach (HtmlNode sub_n in post_data.ChildNodes)
                                    {
                                        switch (sub_n.GetAttributeValue("class", ""))
                                        {
                                            case "post_title":
                                                gp.subject = System.Web.HttpUtility.HtmlDecode(sub_n.InnerText);
                                                break;
                                            case "post_poster_data":
                                                foreach (HtmlNode z in sub_n.ChildNodes)
                                                {
                                                    switch (z.GetAttributeValue("class", ""))
                                                    {
                                                        case "post_author":
                                                            gp.name = System.Web.HttpUtility.HtmlDecode(z.InnerText);
                                                            break;
                                                        case "post_tripcode":
                                                            gp.trip = System.Web.HttpUtility.HtmlDecode(z.InnerText);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                break;
                                            case "time_wrap":
                                                foreach (HtmlNode aaa in sub_n.ChildNodes)
                                                {
                                                    if (aaa.Name == "time")
                                                    {
                                                        gp.time = System.Xml.XmlConvert.ToDateTime(aaa.GetAttributeValue("datetime", ""), System.Xml.XmlDateTimeSerializationMode.Utc);
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        if (file_info.GetAttributeValue("class", "") == "text")
                        {
                            gp.comment = file_info.InnerHtml;
                        }
                    }
                }
            }

            if (pf != null)
            {
                pf.owner = gp.PostNumber;
                pf.board = board;
                gp.file = pf;
            }

            return gp;
        }

        private static int parse_file_size(string s)
        {
            string[] data = s.Trim().Split(' ');

            double result = double.Parse(data[0]);
            switch (data[1].ToUpper())
            {
                case "KB":
                    result = result * 1024;
                    break;
                case "MB":
                    result = result * 1024 * 1024;
                    break;
                default:
                    break;
            }
            return Convert.ToInt32(result);
        }


        private static string fetch_api(Archive arch, string board, int tid)
        {
            string url = string.Format("{0}/_/api/chan/thread/?board={1}&num={2}", arch.Domain, board, tid);

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);

            wr.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:26.0) Gecko/20100101 Firefox/26.0";
            wr.Referer = string.Format("http://boards.4chan.org/{0}/res/{1}", board, tid);

            if (arch.Domain.Contains("4plebs"))
            {
                wr.CookieContainer = new CookieContainer();
                wr.CookieContainer.Add(new Cookie("__cfduid", "d41b5040928012e9fc3add85cfb8ba00b1394037831178") { Domain = "archive.4plebs.org" });
                wr.CookieContainer.Add(new Cookie("foolframe_HPj_foolframe_csrf_token", "3e8fa2cd4e17dc6e1e5b457aed855d7f") { Domain = "archive.4plebs.org" });
            }

            wr.Method = "GET";

            byte[] data = null;

            using (WebResponse wbr = wr.GetResponse())
            {
                using (Stream s = wbr.GetResponseStream())
                {
                    using (MemoryStream memio = new MemoryStream())
                    {
                        s.CopyTo(memio);
                        data = memio.ToArray();
                    }
                }
            }

            try
            {
                byte[] uncompressed = Decompress(data);
                return System.Text.Encoding.UTF8.GetString(uncompressed);
            }
            catch (Exception)
            {
                return System.Text.Encoding.UTF8.GetString(data);
            }
        }

        private static ThreadContainer parse_ffuuka_json(Archive arch, string board, int tid)
        {
            ThreadContainer tc = null;

            //string data = File.ReadAllText(@"C:\Users\Istan\Desktop\sample.txt");

            string data = fetch_api(arch, board, tid);

            //List<Dictionary<string, object>> list = (List<Dictionary<string, object>>)Newtonsoft.Json.JsonConvert.DeserializeObject(data, typeof(List<Dictionary<string, object>>));

            JObject ob = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data, typeof(JObject));

            JToken op_post = ob[tid.ToString()]["op"];

            tc = new ThreadContainer(parse_thread(op_post, board));

            JToken replies = ob[tid.ToString()]["posts"];

            foreach (JProperty t in replies)
            {
                JToken a = replies[t.Name];
                tc.AddReply(parse_reply(a, board));
            }

            return tc;
        }

        private static Thread parse_thread(JToken data, string board)
        {
            Thread t = new Thread();
            t.type = GenericPost.PostType.FoolFuuka;

            t.board = board;

            t.PostNumber = Convert.ToInt32(data["thread_num"]);

            if (data["comment_processed"] != null)
            {
                t.comment = data["comment_processed"].ToString();
            }

            if (data["email"] != null)
            {
                t.email = data["email"].ToString();
            }

            if (data["media"] != null)
            {
                t.file = parse_file(data, board);
            }

            if (data["title"] != null)
            {
                t.subject = data["title"].ToString();
            }

            if (data["capcode"] != null)
            {
                switch (data["capcode"].ToString())
                {
                    case "N":
                        t.capcode = GenericPost.Capcode.None;
                        break;
                    default:
                        break;
                }
            }

            t.IsClosed = true;

            if (data["sticky"] != null)
            {
                t.IsSticky = (data["sticky"].ToString() == "1");
            }

            if (data["name"] != null)
            {
                t.name = data["name"].ToString();
            }

            if (data["trip"] != null)
            {
                t.trip = data["trip"].ToString();
            }

            t.time = Common.ParseUTC_Stamp(Convert.ToInt32(data["timestamp"]));

            return t;
        }
        private static GenericPost parse_reply(JToken data, string board)
        {
            GenericPost gp = new GenericPost();

            gp.type = GenericPost.PostType.FoolFuuka;

            gp.board = board;

            gp.PostNumber = Convert.ToInt32(data["num"]);

            if (data["comment_processed"] != null)
            {
                gp.comment = data["comment_processed"].ToString();
            }

            if (data["email"] != null)
            {
                gp.email = data["email"].ToString();
            }

            if (data["title"] != null)
            {
                gp.subject = data["title"].ToString();
            }

            if (data["media"] != null)
            {
                gp.file = parse_file(data, board);
            }

            if (data["capcode"] != null)
            {
                switch (data["capcode"].ToString())
                {
                    case "N":
                        gp.capcode = GenericPost.Capcode.None;
                        break;
                    default:
                        break;
                }
            }

            if (data["name"] != null)
            {
                gp.name = data["name"].ToString();
            }

            if (data["trip"] != null)
            {
                gp.trip = data["trip"].ToString();
            }

            gp.time = Common.ParseUTC_Stamp(Convert.ToInt32(data["timestamp"]));

            return gp;
        }
        private static PostFile parse_file(JToken data, string board)
        {
            if (data["media"] != null)
            {
                JToken media = data["media"];
                if (media.Count() == 0) { return null; }
                if (media["banned"].ToString() != "0") { return null; }

                PostFile pf = new PostFile();

                pf.board = board;
                pf.filename = media["media_filename_processed"].ToString();

                pf.ext = pf.filename.Split('.').Last();
                pf.filename = pf.filename.Split('.').First();


                pf.FullImageLink = media["media_link"].ToString();
                pf.hash = media["media_hash"].ToString();

                pf.height = Convert.ToInt32(media["media_h"]);
                pf.width = Convert.ToInt32(media["media_w"]);
                if (media["spoiler"] != null) { pf.IsSpoiler = (media["spoiler"].ToString() != "0"); }

                pf.thumbH = Convert.ToInt32(media["preview_h"]);
                pf.thumbW = Convert.ToInt32(media["preview_w"]);

                pf.size = Convert.ToInt32(media["media_size"]);

                pf.thumbnail_tim = media["media"].ToString().Split('.').First();

                pf.ThumbLink = media["thumb_link"].ToString();

                return pf;
            }
            else { return null; }
        }

    }
}
