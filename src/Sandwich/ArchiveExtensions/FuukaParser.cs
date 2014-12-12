using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.IO.Compression;

namespace Sandwich.ArchiveExtensions
{
    public static class FuukaParser
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

        public static ThreadContainer Parse(Archive arhive, string board, int threadID)
        {
            ThreadContainer tc;

            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(get_data(string.Format(arhive.ThreadUrl, board, threadID)));

            var divs = doc.DocumentNode.SelectNodes("//div");

            HtmlNode contentDiv = null; // 8

            foreach (HtmlNode div in divs)
            {
                foreach (HtmlAttribute attr in div.Attributes)
                {
                    if (attr.Name == "class" && attr.Value == "content")
                    {
                        contentDiv = div;
                    }
                }
            }

            HtmlNodeCollection childs = contentDiv.ChildNodes;

            HtmlNode op_post = null;

            foreach (HtmlNode node in childs)
            {
                if (node.Name == "div")
                {
                    op_post = node;
                    break;
                }
            }

            tc = new ThreadContainer(parse_op(op_post, arhive, board));

            foreach (HtmlNode node in childs)
            {
                if (node.Name == "table")
                {
                    HtmlNodeCollection tr_childs = node.ChildNodes[0].ChildNodes;

                    foreach (HtmlNode tr_child in tr_childs)
                    {
                        if (tr_child.Name == "td" && tr_child.Id.StartsWith("p"))
                        {
                            tc.AddReply(parse_reply(tr_child, arhive.FileURL));
                        }
                    }
                }
            }



            return tc;
        }

        private static Thread parse_op(HtmlNode node, Archive archive, string board)
        {
            Thread t = new Thread();

            t.type = GenericPost.PostType.Fuuka;

            t.PostNumber = Convert.ToInt32(node.Id.Remove(0, 1));


            t.board = board;

            if (!node.InnerText.Contains("nothumb"))
            {
                PostFile pf = new PostFile();

                pf.board = t.board;

                HtmlNode file_infos_span = node.ChildNodes[1];

                HtmlNode file_info_spanX = file_infos_span.ChildNodes[0]; // text node

                string[] file_info_data = file_info_spanX.InnerText.Replace("File:", "").Split(',');

                pf.size = parse_file_size(file_info_data[0]);

                pf.width = Convert.ToInt32(file_info_data[1].Split('x')[0]);
                pf.height = Convert.ToInt32(file_info_data[1].Split('x')[1]);

                pf.filename = file_info_data[2].Trim();

                //File hash ----------

                HtmlNode image_hash_spanX = file_infos_span.ChildNodes[1]; // comment node

                pf.hash = image_hash_spanX.InnerText.Replace("<!--", "").Replace("-->", "").Trim();


                HtmlNode image_thumb = null;

                foreach (HtmlNode im in node.ChildNodes) //usually it's at node.ChildNodes[11]
                {
                    if (im.Name == "a" && im.InnerHtml.Contains("<img class=\"thumb\""))
                    {
                        foreach (HtmlNode aa in im.ChildNodes)
                        {
                            if (aa.Name == "img") { image_thumb = aa; break; }
                        }
                    }
                }

                pf.thumbH = Convert.ToInt32(image_thumb.GetAttributeValue("height", "250"));
                pf.thumbW = Convert.ToInt32(image_thumb.GetAttributeValue("width", "250"));

                //Image URLS
                string thumbnail_link = image_thumb.GetAttributeValue("src", "");

                if (thumbnail_link != "")
                {
                    pf.ThumbLink = archive.FileURL + thumbnail_link;
                    pf.thumbnail_tim = pf.ThumbLink.Split('/').Last().Split('.')[0]; // thumb file name
                }

                if (archive.IsFileSupported(board))
                {
                    string full_image_link = image_thumb.ParentNode.GetAttributeValue("href", "");

                    if (full_image_link != "")
                    {
                        pf.FullImageLink = archive.FileURL + full_image_link;

                        pf.filename = pf.FullImageLink.Split('/').Last().Split('.')[0];

                        pf.ext = pf.FullImageLink.Split('.').Last();
                    }
                    else 
                    {
                        pf.FullImageLink = "nofile";

                        pf.filename = pf.thumbnail_tim;

                        pf.ext = "jpg";
                    }
                }
                else 
                {
                    pf.FullImageLink = "nofile";

                    pf.filename = pf.thumbnail_tim;

                    pf.ext = "jpg"; //Since there is no full file, we assign the thumbnail extension, which is jpg.
                }
                
                pf.owner = t.PostNumber;

                t.file = pf;
            }
            else
            {
                t.file = null;
            }

            HtmlNode poster_info_label = null;

            foreach (HtmlNode a in node.ChildNodes)
            {
                if (a.Name == "label") { poster_info_label = a; break; }
            }

            if (poster_info_label != null)
            {
                foreach (HtmlNode no in poster_info_label.ChildNodes)
                {
                    if (no.Name == "span")
                    {
                        if (no.GetAttributeValue("class", "") == "postername")
                        {
                            if (no.ChildNodes.Count == 1)
                            {
                                t.name = no.InnerText.Trim();
                            }
                            else
                            {
                                //email field!
                                foreach (HtmlNode xyz in no.ChildNodes)
                                {
                                    if (xyz.Name == "a")
                                    {
                                        if (xyz.GetAttributeValue("href", "").Contains(@"mailto:"))
                                        {
                                            t.name = xyz.InnerText.Trim();
                                            t.email = xyz.GetAttributeValue("href", "").Replace("mailto:", "");
                                        }
                                    }
                                }
                            }
                        }
                        if (no.GetAttributeValue("class", "") == "posttime")
                        {
                            string time_unix = no.GetAttributeValue("title", "");
                            if (time_unix.Length > 0)
                            {
                                time_unix = time_unix.Remove(10);
                            }
                            t.time = Common.ParseUTC_Stamp(Convert.ToInt32(time_unix.Trim()));
                        }
                    }
                }
            }

            foreach (HtmlNode xyz in node.ChildNodes)
            {
                if (xyz.Name == "blockquote")
                {
                    t.comment = xyz.ChildNodes[0].InnerHtml;
                    break;
                }
            }


            if (t.email == null) { t.email = ""; }
            if (t.subject == null) { t.subject = ""; }
            if (t.trip == null) { t.trip = ""; }

            //-----------------------------

            /* pf.thumbW = pf.width;
             pf.thumbH = pf.height;*/

            return t;
        }

        private static GenericPost parse_reply(HtmlNode node, string base_url)
        {
            GenericPost gp = new GenericPost();
            gp.type = GenericPost.PostType.Fuuka;

            gp.PostNumber = Convert.ToInt32(node.Id.Remove(0, 1));

            HtmlNode label = node.ChildNodes[1];

            foreach (HtmlNode subnode in label.ChildNodes)
            {
                if (subnode.Name == "span")
                {
                    if (subnode.GetAttributeValue("class", "") == "postername")
                    {
                        gp.name = subnode.InnerText.Trim();

                        if (subnode.ChildNodes.Count >= 1)
                        {
                            foreach (HtmlNode asdf in subnode.ChildNodes)
                            {
                                if (asdf.Name == "a")
                                {
                                    if (asdf.GetAttributeValue("href", "").Contains("mailto:"))
                                    {
                                        gp.email = asdf.GetAttributeValue("href", "").Remove(0, 7);
                                    }
                                }
                            }
                        }
                    }

                    if (subnode.GetAttributeValue("class", "") == "posttime")
                    {
                        string time_unix = subnode.GetAttributeValue("title", "");

                        if (time_unix.Length > 0) { time_unix = time_unix.Remove(10); }

                        gp.time = Common.ParseUTC_Stamp(Convert.ToInt32(time_unix));
                    }
                }
            }

            //file info
            bool file_found = false;
            PostFile pf = null;

            foreach (HtmlNode spannode in node.ChildNodes)
            {
                if (spannode.Name == "span")
                {
                    if (spannode.InnerText.Contains("File:"))
                    {
                        pf = new PostFile();

                        file_found = true;

                        HtmlNode file_info = spannode.ChildNodes[0];

                        HtmlNode file_hash = spannode.ChildNodes[1];

                        string[] file_info_data = file_info.InnerText.Replace("File:", "").Split(',');

                        pf.size = parse_file_size(file_info_data[0]);
                        pf.width = Convert.ToInt32(file_info_data[1].Split('x')[0]);
                        pf.height = Convert.ToInt32(file_info_data[1].Split('x')[1]);
                        pf.filename = file_info_data[2].Trim();

                        pf.hash = file_hash.InnerText.Replace("<!--", "").Replace("-->", "").Trim();

                        break;
                    }
                }
            }

            if (file_found)
            {
                //look for the image thumb

                foreach (HtmlNode sub_node in node.ChildNodes)
                {
                    if (sub_node.Name == "a")
                    {
                        if (sub_node.ChildNodes.Count >= 1)
                        {
                            foreach (HtmlNode img in sub_node.ChildNodes)
                            {
                                if (img.Name == "img")
                                {
                                    pf.FullImageLink = (sub_node.GetAttributeValue("href", "") != "") ? base_url + sub_node.GetAttributeValue("href", "") : "";

                                    pf.ThumbLink = (img.GetAttributeValue("src", "") != "") ? base_url + img.GetAttributeValue("src", "") : "";

                                    pf.thumbH = Convert.ToInt32(img.GetAttributeValue("height", "250"));
                                    pf.thumbW = Convert.ToInt32(img.GetAttributeValue("width", "250"));

                                    pf.owner = gp.PostNumber;

                                    pf.ext = pf.FullImageLink.Split('.').Last();

                                    pf.filename = pf.FullImageLink.Split('/').Last().Split('.')[0];

                                    pf.thumbnail_tim = pf.FullImageLink.Split('/').Last().Split('.')[0];


                                    break;
                                }
                            }
                        }
                    }
                    else { continue; }
                }
            }

            gp.file = pf;

            foreach (HtmlNode blockquote in node.ChildNodes)
            {
                if (blockquote.Name == "blockquote") { gp.comment = blockquote.ChildNodes[0].InnerHtml; break; }
            }

            return gp;
        }

        private static int parse_file_size(string s)
        {
            string[] data = s.Trim().Split(' ');

            double result = double.Parse(data[0]);
            switch (data[1])
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

    }
}
