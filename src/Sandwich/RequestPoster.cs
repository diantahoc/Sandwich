using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;
using System.ComponentModel;

namespace Sandwich
{
    class RequestPoster
    {

        string _board;
        RequestPosterData _data;

        BackgroundWorker bg;

        public RequestPoster(string board)
        {
            _board = board;
            bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
        }

        RequestPosterResponse _rpp;

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_rpp != null)
            {
                SendCompleted(_rpp);
            }
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                thread_t();
            }
            catch (Exception ex)
            {
                if (this.SendError != null) { SendError(ex); };
            }

        }

        private string url_template = "https://sys.4chan.org/%/post";

        public void SendRequest(RequestPosterData data)
        {
            if (!bg.IsBusy)
            {
                _data = data;
                bg.RunWorkerAsync();
            }
        }

        private void thread_t()
        {
            if (_data != null)
            {
                if (SendingProgressChanged != null) { SendingProgressChanged(-1); }

                //To hold all the form variables
                NameValueCollection values = new NameValueCollection();

                List<UploadFile> files = new List<UploadFile>(); ;

                // 'upfile' is the name of the the uploaded file
                if (_data.file_path != "" && _data.file_name != "")
                {
                    files.Add(
                            new UploadFile
                            {
                                Name = "upfile",
                                Filename = _data.file_name,
                                ContentType = Common.Map_MIME_Type(_data.file_name),
                                Stream = new MemoryStream(File.ReadAllBytes(_data.file_path))
                            }
                    );
                }
                else
                {
                    files.Add(
                            new UploadFile
                            {
                                Name = "upfile",
                                Filename = "",
                                ContentType = "application/octet-stream",
                                Stream = null
                            }
                    );
                }

                int maxium_file_size = BoardInfo.GetBoardMaximumFileSize(_board);
                values.Add("MAX_FILE_SIZE", maxium_file_size.ToString());

                //values.Add("MAX_FILE_SIZE", "3145728");

                values.Add("mode", "regist");

                if (_data.is_reply)
                {
                    values.Add("resto", _data.tid.ToString());
                }

                values.Add("name", _data.name);
                values.Add("email", _data.email);
                values.Add("sub", _data.subject);
                values.Add("com", _data.comment);

                values.Add("pwd", _data.password);

                //Check if the user is using a 4chan pass (is_pass is a boolean).
                //moot remove the captcha when a pass is availble, so recaptcha fields are not needed.
                if (!_data.is_pass)
                {
                    values.Add("recaptcha_response_field", _data.Captcha.ResponseField);
                    values.Add("recaptcha_challenge_field", _data.Captcha.ChallengeField);
                }


                //-------------------------
                // url_template is "https://sys.4chan.org/%/post" , _board is the board letter 

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url_template.Replace("%", _board));
                request.Method = "POST";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                request.Referer = "http://boards.4chan.org/" + _board;


                // request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)"); // "Sandwich (Like Mozilla 5.0 | Gecko | Apple Webkit)"; //Custom UA. I suggest to use a real user-agent

                //This is the part we encode data using "multipart/form-data" technique 
                //RFC 1867 : http://tools.ietf.org/html/rfc1867#section-6

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
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
                    for (int index = 0; index < files.Count; index++)
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
                            MemoryStream fs_s = files[index].Stream;
                            double file_length = Convert.ToDouble(fs_s.Length);
                            int b_s = 0;
                            byte[] read_buffer = new byte[2048];
                            double uploaded = 0;

                            while ((b_s = fs_s.Read(read_buffer, 0, 2048)) > 0)
                            {
                                requestStream.Write(read_buffer, 0, b_s);
                                if (this.SendingProgressChanged != null)
                                {
                                    uploaded += b_s;
                                    SendingProgressChanged(uploaded / file_length);
                                }
                            }

                            //files[index].Stream.CopyTo(requestStream);
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
                                response_text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(response_text))
                {
                    _rpp = new RequestPosterResponse(response_text);
                }
            }
        }

        public delegate void DataSendCompleteEvent(RequestPosterResponse response);

        public event DataSendCompleteEvent SendCompleted;

        public event Common.ProgressChanged SendingProgressChanged;

        public delegate void SendErrorEvent(Exception ex);
        public event SendErrorEvent SendError;

        //public void SendAsync(RequestPosterData _data)
        //{
        //    if (_data != null)
        //    {
        //        //To hold all the form variables
        //        NameValueCollection values = new NameValueCollection();

        //        List<UploadFile> files = new List<UploadFile>(); ;

        //        // 'upfile' is the name of the the uploaded file
        //        if (_data.file_path != "" && _data.file_name != "")
        //        {
        //            files.Add(
        //                    new UploadFile
        //                    {
        //                        Name = "upfile",
        //                        Filename = _data.file_name,
        //                        ContentType = Common.Map_MIME_Type(_data.file_name),
        //                        Stream = new MemoryStream(File.ReadAllBytes(_data.file_path))
        //                    }
        //            );
        //        }
        //        else
        //        {
        //            files.Add(
        //                    new UploadFile
        //                    {
        //                        Name = "upfile",
        //                        Filename = "",
        //                        ContentType = "application/octet-stream",
        //                        Stream = null
        //                    }
        //            );
        //        }

        //        int maxium_file_size = BoardInfo.GetBoardMaximumFileSize(_board);
        //        values.Add("MAX_FILE_SIZE", maxium_file_size.ToString());

        //        values.Add("mode", "regist");

        //        if (_data.is_reply)
        //        {
        //            values.Add("resto", _data.tid.ToString());
        //        }

        //        values.Add("name", _data.name);
        //        values.Add("email", _data.email);
        //        values.Add("sub", _data.subject);
        //        values.Add("com", _data.comment);

        //        values.Add("pwd", _data.password);

        //        //Check if the user is using a 4chan pass (is_pass is a boolean).
        //        //moot remove the captcha when a pass is availble, so recaptcha fields are not needed.
        //        if (!_data.is_pass)
        //        {
        //            values.Add("recaptcha_response_field", _data.Captcha.ResponseField);
        //            values.Add("recaptcha_challenge_field", _data.Captcha.ChallengeField);
        //        }


        //        //-------------------------
        //        // url_template is "https://sys.4chan.org/%/post" , _board is the board letter 

        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url_template.Replace("%", _board));
        //        request.Method = "POST";
        //        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
        //        request.Referer = "http://boards.4chan.org/" + _board;
        //        request.AllowWriteStreamBuffering = false;

        //        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
        //        request.ContentType = "multipart/form-data; boundary=" + boundary;
        //        boundary = "--" + boundary;

        //        //Preparet the request data, since we need the size of it

        //        MemoryStream requestStream_temp = new MemoryStream();

        //        // Write the values
        //        foreach (string name in values.Keys)
        //        {
        //            byte[] buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
        //            requestStream_temp.Write(buffer, 0, buffer.Length);
        //            buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
        //            requestStream_temp.Write(buffer, 0, buffer.Length);
        //            buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
        //            requestStream_temp.Write(buffer, 0, buffer.Length);
        //        }

        //        // Write the files
        //        for (int index = 0; index < files.Count; index++)
        //        {
        //            byte[] buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
        //            requestStream_temp.Write(buffer, 0, buffer.Length);

        //            buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", files[index].Name, files[index].Filename, Environment.NewLine));
        //            requestStream_temp.Write(buffer, 0, buffer.Length);

        //            buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", files[index].ContentType, Environment.NewLine));
        //            requestStream_temp.Write(buffer, 0, buffer.Length);

        //            if (files[index].Stream == null)
        //            {
        //                Stream.Null.CopyTo(requestStream_temp);
        //            }
        //            else
        //            {
        //                files[index].Stream.CopyTo(requestStream_temp);
        //                files[index].Stream.Dispose();
        //            }

        //            buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
        //            requestStream_temp.Write(buffer, 0, buffer.Length);
        //        }

        //        byte[] boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
        //        requestStream_temp.Write(boundaryBuffer, 0, boundaryBuffer.Length);

        //        request.ContentLength = requestStream_temp.Length;

        //        request.BeginGetRequestStream(new AsyncCallback(WriteToStreamCallback), new object[] { requestStream_temp, request });
        //    }
        //}

        //private void WriteToStreamCallback(IAsyncResult asynchronousResult)
        //{
        //    object[] ob = (object[])asynchronousResult.AsyncState;
        //    MemoryStream prepared_request_data = (MemoryStream)ob[0];
        //    HttpWebRequest request = (HttpWebRequest)ob[1];

        //    Stream requestStream = request.EndGetRequestStream(asynchronousResult);

        //    double file_length = Convert.ToDouble(prepared_request_data.Length);
        //    int b_s = 0;
        //    byte[] read_buffer = new byte[2048];
        //    double uploaded = 0;

        //    while ((b_s = prepared_request_data.Read(read_buffer, 0, 2048)) > 0)
        //    {
        //        requestStream.Write(read_buffer, 0, b_s);
        //        requestStream.Flush();
        //        if (this.SendingProgressChanged != null)
        //        {
        //            uploaded += b_s;
        //            SendingProgressChanged(uploaded / file_length);
        //        }
        //    }

        //    prepared_request_data.Dispose();

        //    requestStream.Flush();

        //    request.BeginGetResponse(new AsyncCallback(ReadHttpResponseCallback), request);
        //}

        //private void ReadHttpResponseCallback(IAsyncResult asynchronousResult)
        //{
        //    HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
        //    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult))
        //    {
        //        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
        //        {
        //            string response_text = reader.ReadToEnd(); // Get the result
        //            if (!String.IsNullOrEmpty(response_text))
        //            {
        //                _rpp = new RequestPosterResponse(response_text);
        //                if (SendCompleted != null) { SendCompleted(_rpp); }
        //            }
        //        }
        //    }
        //}

    }

    public class UploadFile
    {
        public UploadFile()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public MemoryStream Stream { get; set; }
    }

    public class RequestPosterData
    {
        public bool is_reply;
        public int tid;
        public string name = "";
        public string subject = "";
        public string email = "";
        public string comment = "";

        //4chan pass
        public bool is_pass;
        public SolvedCaptcha Captcha = null;

        public string password = "";
        public string file_path = "";

        public string file_name = "";


    }

    public class RequestPosterResponse
    {
        public RequestPosterResponse(string response_data)
        {
            this.Status = get_status(response_data);

            if (this.Status == ResponseStatus.Success)
            {
                try
                {
                    this.ResponseBody = get_new_reply_id(response_data); //should be in this format: [threadID:newReplyID] or [0:newThreadID]
                }
                catch (Exception)
                {
                    this.ResponseBody = response_data;
                }
            }
            else
            {
                this.ResponseBody = response_data;
            }
            this.ResponseBody = response_data;
        }

        public enum ResponseStatus
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

        public ResponseStatus Status { get; private set; }

        public string ResponseBody { get; private set; }

        private ResponseStatus get_status(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (str.IndexOf("Post successful") != -1)
                {
                    return ResponseStatus.Success;
                }

                if (str.IndexOf("Flood") != -1)
                {
                    return ResponseStatus.Flood;
                }
                if (str.IndexOf("Duplicate") != -1)
                {
                    return ResponseStatus.DuplicateFile;
                }
                if (str.IndexOf("banned") != -1)
                {
                    if (str.IndexOf("will not expire") != -1)
                        return ResponseStatus.PermaBanned;
                    else
                        return ResponseStatus.Banned;
                }
                if (str.IndexOf("verification") != -1 || str.IndexOf("expired") != -1 || str.IndexOf("solve the CAPTCHA") != -1 || str.Contains("mistyped the CAPTCHA"))
                {
                    return ResponseStatus.CAPTCHA;
                }
                if (str.IndexOf("board doesn") != -1)
                {
                    return ResponseStatus.InvalidBoard;
                }
                if (str.IndexOf("ISP") != -1)
                {
                    return ResponseStatus.RangeBanned;
                }
                if (str.IndexOf("Max limit of") != -1)
                {
                    return ResponseStatus.Limit;
                }
                if (str.IndexOf("Thread specified") != -1)
                {
                    return ResponseStatus.InvalidThreadID;
                }
                if (str.IndexOf("appears corrupt") != -1)
                {
                    return ResponseStatus.CorruptedImage;
                }
                if (str.IndexOf("before posting") != -1)
                {
                    return ResponseStatus.Muted;
                }
                if (str.IndexOf("require a subject") != -1)
                {
                    return ResponseStatus.SubjectRequired;
                }
                if (str.IndexOf("spam") != -1)
                {
                    return ResponseStatus.SpamFilter;
                }
                if (str.IndexOf("No file selected") != -1)
                {
                    return ResponseStatus.FileRequired;
                }

                if (str.Contains("issued a warning"))
                {
                    return ResponseStatus.Warned;
                }

                if (str.Contains("Maximum file size allowed is"))
                {
                    return ResponseStatus.FileTooLarge;
                }
            }

            return ResponseStatus.Unkown;
        }

        private string get_new_reply_id(string response_data)
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

                //   int tid = Convert.ToInt32(dataa[0].Split(':')[1]);

                // int replyid = Convert.ToInt32(dataa[1].Split(':')[1]);

                return String.Format("{0}:{1}", dataa[0].Split(':')[1], dataa[1].Split(':')[1]);
            }
            else
            {
                return response_data;
            }
        }
    }
}
