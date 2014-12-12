using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich.Chans._4chan
{
    public class _4chanPostSenderResponse : IPostSenderResponse
    {
        public _4chanPostSenderResponse(string data)
        {
            this.ResponseStatus = get_status(data);

            if (this.ResponseStatus == Core.PostSendingStatus.Success)
            {
                try
                {
                    this.AdditionalData = get_new_reply_id(data); //should be in this format: [threadID:newReplyID] or [0:newThreadID]
                }
                catch (Exception)
                {
                }
            }

            this.ResponseHTML = data;
        }

        public Core.PostSendingStatus ResponseStatus { get; private set; }

        private Core.PostSendingStatus get_status(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (str.IndexOf("Post successful") != -1)
                {
                    return Core.PostSendingStatus.Success;
                }

                if (str.IndexOf("Flood") != -1)
                {
                    return Core.PostSendingStatus.Flood;
                }
                if (str.IndexOf("Duplicate") != -1)
                {
                    return Core.PostSendingStatus.DuplicateFile;
                }
                if (str.IndexOf("banned") != -1)
                {
                    if (str.IndexOf("will not expire") != -1)
                        return Core.PostSendingStatus.PermaBanned;
                    else
                        return Core.PostSendingStatus.Banned;
                }
                if (str.IndexOf("verification") != -1 || str.IndexOf("expired") != -1 || str.IndexOf("solve the CAPTCHA") != -1 || str.Contains("mistyped the CAPTCHA"))
                {
                    return Core.PostSendingStatus.CAPTCHA;
                }
                if (str.IndexOf("board doesn") != -1)
                {
                    return Core.PostSendingStatus.InvalidBoard;
                }
                if (str.IndexOf("ISP") != -1)
                {
                    return Core.PostSendingStatus.RangeBanned;
                }
                if (str.IndexOf("Max limit of") != -1)
                {
                    return Core.PostSendingStatus.Limit;
                }
                if (str.IndexOf("Thread specified") != -1)
                {
                    return Core.PostSendingStatus.InvalidThreadID;
                }
                if (str.IndexOf("appears corrupt") != -1)
                {
                    return Core.PostSendingStatus.CorruptedImage;
                }
                if (str.IndexOf("before posting") != -1)
                {
                    return Core.PostSendingStatus.Muted;
                }
                if (str.IndexOf("require a subject") != -1)
                {
                    return Core.PostSendingStatus.SubjectRequired;
                }
                if (str.IndexOf("spam") != -1)
                {
                    return Core.PostSendingStatus.SpamFilter;
                }
                if (str.IndexOf("No file selected") != -1)
                {
                    return Core.PostSendingStatus.FileRequired;
                }

                if (str.Contains("issued a warning"))
                {
                    return Core.PostSendingStatus.Warned;
                }

                if (str.Contains("Maximum file size allowed is"))
                {
                    return Core.PostSendingStatus.FileTooLarge;
                }
            }

            return Core.PostSendingStatus.Unkown;
        }

        private KeyValuePair<int, int> get_new_reply_id(string response_data)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response_data);

            HtmlAgilityPack.HtmlNode d = doc.DocumentNode;

            string data = "";

            foreach (HtmlAgilityPack.HtmlNode n1 in d.ChildNodes)
            {
                if (n1.Name == "body")
                {
                    foreach (HtmlAgilityPack.HtmlNode n2 in n1.ChildNodes)
                    {
                        if (n2.Name == "#comment")
                        {
                            data = n2.InnerText;
                        }
                    }
                }
            }

            //"<!-- thread:38545194,no:38545729 -->"

            if (data != "")
            {
                data = data.Replace("<!--", "").Replace("-->", "").Trim();

                string[] dataa = data.Split(',');

                int tid = Convert.ToInt32(dataa[0].Split(':')[1]);

                int replyid = Convert.ToInt32(dataa[1].Split(':')[1]);

                return new KeyValuePair<int, int>(tid, replyid);
            }
            else
            {
                return new KeyValuePair<int, int>();
            }
        }

        public string ResponseHTML { get; private set; }

        public KeyValuePair<int, int> AdditionalData { get; private set; }
    }
}
