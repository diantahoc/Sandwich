using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Web;
using Sandwich.Helpers;
using Sandwich.DataTypes;

namespace Sandwich.Helpers
{
    public static class ThreadHelper
    {

        public static CommentToken[] TokenizeComment(GenericPost gp)
        {
            return TokenizeComment(gp.comment, gp.type);
        }

        public static CommentToken[] TokenizeComment(string comment, GenericPost.PostType type)
        {
            if (String.IsNullOrEmpty(comment))
            {
                return new CommentToken[] { };
            }
            else
            {
                List<CommentToken> tokens = new List<CommentToken>();

                HtmlDocument d = new HtmlDocument();

                d.LoadHtml(comment);

                switch (type)
                {
                    case GenericPost.PostType.FourChan:
                        foreach (HtmlNode node in d.DocumentNode.ChildNodes)
                        {
                            try
                            {
                                switch (node.Name)
                                {
                                    case "#text":
                                        tokens.Add(new CommentToken(CommentToken.TokenType.Text, HttpUtility.HtmlDecode(node.InnerText)));
                                        break;
                                    case "a":
                                        if (node.GetAttributeValue("class", "") == "quotelink")
                                        {
                                            string inner_text = HttpUtility.HtmlDecode(node.InnerText);
                                            if (inner_text.StartsWith(">>>"))
                                            {
                                                //board redirect (sometimes with a post number)
                                                int test_i = -1;
                                                try
                                                {
                                                    test_i = Convert.ToInt32(inner_text.Split('/').Last()); // The last should be a number or an empty string. I guess

                                                    //if success, it's a board_thread_redirect OR it's a cross-thread link ( I don't know if 4chan handle both the same way )

                                                    string board_letter = inner_text.Replace(">", "").Replace("/", "").Replace(test_i.ToString(), "");

                                                    tokens.Add(new CommentToken(CommentToken.TokenType.BoardThreadRedirect, board_letter + "-" + test_i.ToString()));

                                                }
                                                catch (Exception)
                                                {
                                                    // it is a plain board redirect such as >>>/g/
                                                    tokens.Add(new CommentToken(CommentToken.TokenType.BoardRedirect, inner_text.Replace(">", "").Replace("/", ""))); // ex: >>>/g/ -> g
                                                }
                                            }
                                            else if (inner_text.StartsWith(">>"))
                                            {
                                                int test_i = -1;
                                                try
                                                {
                                                    test_i = Convert.ToInt32(inner_text.Remove(0, 2));
                                                    //it's a post quote link
                                                    tokens.Add(new CommentToken(CommentToken.TokenType.Quote, inner_text.Remove(0, 2)));
                                                }
                                                catch (Exception)
                                                {
                                                    throw;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //throw new Exception("Unsupported data type");
                                            break;
                                        }
                                        break;
                                    case "br":
                                        tokens.Add(new CommentToken(CommentToken.TokenType.Newline, ""));
                                        break;
                                    case "wbr":
                                        //no action
                                        break;
                                    case "span":
                                        if (node.GetAttributeValue("class", "") == "quote")
                                        {
                                            tokens.Add(new CommentToken(CommentToken.TokenType.GreenText, HttpUtility.HtmlDecode(node.InnerText)));
                                        }
                                        else if (node.GetAttributeValue("class", "") == "deadlink")
                                        {
                                            //dead link
                                            string inner_text = HttpUtility.HtmlDecode(node.InnerText);
                                            int test_i = -1;
                                            try
                                            {
                                                test_i = Convert.ToInt32(inner_text.Remove(0, 2));
                                                //it's a post quote link
                                                tokens.Add(new CommentToken(CommentToken.TokenType.DeadLink, inner_text.Remove(0, 2)));
                                            }
                                            catch (Exception)
                                            {
                                                throw;
                                            }

                                        }
                                        else if (node.GetAttributeValue("class", "") == "fortune")
                                        {
                                            string data = HttpUtility.HtmlDecode(node.InnerText);
                                            string color = node.GetAttributeValue("style", "");
                                            tokens.Add(new CommentToken(CommentToken.TokenType.ColoredFText, "#" + color.Split('#')[1] + "$" + data));
                                        }
                                        else if (node.GetAttributeValue("class", "") == "abbr")
                                        {
                                            string exif_table_id = "";

                                            foreach (HtmlNode a in node.ChildNodes)
                                            {
                                                if (a.Name == "a" && a.GetAttributeValue("onclick", "").StartsWith("toggle"))
                                                {
                                                    exif_table_id = a.GetAttributeValue("onclick", "").Replace("toggle('", "").Replace("')", "");
                                                }
                                            }

                                            HtmlNode exif_table = d.GetElementbyId(exif_table_id);

                                            StringBuilder s = new StringBuilder();

                                            foreach (HtmlNode tr in exif_table.ChildNodes)
                                            {
                                                switch (tr.ChildNodes.Count)
                                                {
                                                    case 1:
                                                        if (string.IsNullOrEmpty(tr.InnerText))
                                                            s.Append("----------");
                                                        else
                                                            s.Append(tr.InnerText);
                                                        break;
                                                    case 2:
                                                        s.AppendFormat("{0} : {1}", tr.ChildNodes[0].InnerText, tr.ChildNodes[1].InnerText);
                                                        break;
                                                    default:
                                                        break;
                                                }
                                                s.AppendLine();
                                            }

                                            tokens.Add(new CommentToken(CommentToken.TokenType.ExifTable, HttpUtility.HtmlDecode(s.ToString())));

                                            break;
                                        }
                                        else
                                        {
                                            throw new Exception("Unsupported data type");
                                        }
                                        break;
                                    case "pre":
                                        if (node.GetAttributeValue("class", "") == "prettyprint")
                                        {
                                            StringBuilder sb = new StringBuilder();
                                            foreach (HtmlNode prenode in node.ChildNodes)
                                            {
                                                if (prenode.Name == "br")
                                                {
                                                    sb.AppendLine();
                                                }
                                                else
                                                {
                                                    sb.Append(prenode.InnerText);
                                                }
                                            }
                                            tokens.Add(new CommentToken(CommentToken.TokenType.CodeBlock, HttpUtility.HtmlDecode(sb.ToString())));
                                        }
                                        break;
                                    case "s":
                                        tokens.Add(new CommentToken(CommentToken.TokenType.SpoilerText, HttpUtility.HtmlDecode(node.InnerText)));
                                        break;
                                    case "small":
                                        //Oekaki Post 
                                        break;
                                    case "font":
                                        break;
                                    case "strong":
                                        break;
                                    case "table":
                                        if (node.GetAttributeValue("class", "") == "exif")
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            //do handle 
                                            break;
                                        }
                                    default:
                                        throw new Exception("Unsupported data type");
                                    // break;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        break;
                    case GenericPost.PostType.FoolFuuka:
                        foreach (HtmlNode node in d.DocumentNode.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case "a":
                                    if (node.GetAttributeValue("class", "") == "backlink")
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.Quote, node.GetAttributeValue("data-post", "")));
                                    }
                                    else
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.URL, node.GetAttributeValue("href", "")));
                                    }
                                    break;
                                case "#text":
                                    tokens.Add(new CommentToken(CommentToken.TokenType.Text, HttpUtility.HtmlDecode(node.InnerText.Trim())));
                                    break;
                                case "br":
                                    tokens.Add(new CommentToken(CommentToken.TokenType.Newline, null));
                                    break;
                                case "span":
                                    if (node.GetAttributeValue("class", "") == "greentext")
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.GreenText, HttpUtility.HtmlDecode(node.InnerText)));
                                    }
                                    else if (node.GetAttributeValue("class", "") == "spoiler")
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.SpoilerText, HttpUtility.HtmlDecode(node.InnerText)));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case GenericPost.PostType.Fuuka:
                        foreach (HtmlNode node in d.DocumentNode.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case "a":
                                    if (node.GetAttributeValue("class", "") == "backlink")
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.Quote, node.GetAttributeValue("href", "").Replace("#p", "")));
                                    }
                                    else
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.URL, node.GetAttributeValue("href", "")));
                                    }
                                    break;
                                case "br":
                                    tokens.Add(new CommentToken(CommentToken.TokenType.Newline, null));
                                    break;
                                case "#text":
                                    tokens.Add(new CommentToken(CommentToken.TokenType.Text, HttpUtility.HtmlDecode(node.InnerText)));
                                    break;
                                case "span":
                                    if (node.GetAttributeValue("class", "") == "unkfunc")
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.GreenText, HttpUtility.HtmlDecode(node.InnerText)));
                                    }
                                    else if (node.GetAttributeValue("class", "") == "spoiler")
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.SpoilerText, HttpUtility.HtmlDecode(node.InnerText)));
                                    }
                                    break;
                                case "code":
                                    if (node.GetAttributeValue("class", "") == "prettyprint")
                                    {
                                        tokens.Add(new CommentToken(CommentToken.TokenType.CodeBlock, HttpUtility.HtmlDecode(GetNodeText(node))));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
                return tokens.ToArray();
            }
        }


        public static string GetNodeText(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();

            if (node.Name == "br")
            {
                sb.AppendLine();
            }
            else if (node.ChildNodes.Count > 0)
            {
                foreach (HtmlNode a in node.ChildNodes)
                {
                    sb.Append(ThreadHelper.GetNodeText(a));
                }
            }
            else
            {
                return node.InnerText;
            }


            return sb.ToString();
        }

        public static string Guess_Post_Title(GenericPost t)
        {
            if (String.IsNullOrEmpty(t.subject))
            {
                if (String.IsNullOrEmpty(t.comment))
                {
                    return "/" + t.board + "/ - " + BoardInfo.GetBoardTitle(t.board) + " - " + t.PostNumber.ToString();
                }
                else
                {
                    string comment = "";

                    HtmlAgilityPack.HtmlDocument d = new HtmlAgilityPack.HtmlDocument();
                    d.LoadHtml(t.comment);

                    comment = HttpUtility.HtmlDecode(d.DocumentNode.InnerText);
                    if (comment.Length > 25)
                    {
                        return "/" + t.board + "/ - " + comment.Remove(24) + "...";
                    }
                    else
                    {
                        return "/" + t.board + "/ - " + comment + "...";
                    }

                }
            }
            else
            {
                return HttpUtility.HtmlDecode(t.subject);
            }
        }
    }
}
