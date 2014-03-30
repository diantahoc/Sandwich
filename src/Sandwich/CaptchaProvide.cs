using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Net;
using HtmlAgilityPack;
using System.ComponentModel;

namespace Sandwich
{
    class CaptchaProvider : IDisposable
    {
        /*client.DownloadFile(address, fileName);http://www.google.com/recaptcha/api/noscript?k=6Ldp2bsSAAAAAAJ5uyx_lx34lJeEpTLVkP5k04qc"*/
      
        BackgroundWorker bg;

        public CaptchaProvider()
        {
            bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
        }

        Captcha _cc;

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_cc != null) 
                CaptchaLoaded(_cc);
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            thread();
        }


        public void GetChallenge() 
        {
            if (!bg.IsBusy) 
            {
                bg.RunWorkerAsync();
            }
        }

        private void thread() 
        {
            try
            {
                using (WebClient nc = new WebClient())
                {
                    nc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                    string html = nc.DownloadString(@"http://www.google.com/recaptcha/api/noscript?k=6Ldp2bsSAAAAAAJ5uyx_lx34lJeEpTLVkP5k04qc");

                    HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();

                    doc.LoadHtml(html);

                    string re_challenge = doc.GetElementbyId("recaptcha_challenge_field").Attributes["value"].Value;
                    HtmlNode imagenode = doc.DocumentNode.SelectNodes("//img")[0];
                    string image_src = imagenode.Attributes["src"].Value;

                    if (!String.IsNullOrEmpty(image_src))
                    {
                        byte[] imagedata = nc.DownloadData("http://www.google.com/recaptcha/api/" + image_src);
                        MemoryStream memIO = new MemoryStream(imagedata);
                        Captcha cc = new Captcha(memIO, re_challenge);
                        _cc = cc;
                    }
                    else
                    {
                        CaptchaError(new Exception("Image source is null"));
                    }
                }
            }
            catch (Exception ex)
            {
                _cc = null; 
                CaptchaError(ex);
            }
        }

        public void Dispose() 
        {
            this.bg.Dispose();
        }

        public delegate void CaptchaLoadErrorEvent(Exception c);
        public event CaptchaLoadErrorEvent CaptchaError;

        public delegate void CaptchaLoadedEvent(Captcha c);
        public event CaptchaLoadedEvent CaptchaLoaded;
    }

    public class Captcha : IDisposable
    {
        private MemoryStream memio;

        public Captcha(MemoryStream data, string challenge) 
        {
            this.memio = data;
            this.ChallengeText = challenge;

            BitmapImage bbb = new BitmapImage();

            bbb.BeginInit();

            bbb.CacheOption = BitmapCacheOption.OnLoad;

            bbb.StreamSource = data;

            bbb.EndInit();

            bbb.Freeze();

            this.CaptchaImage = bbb;
        }

        public BitmapImage CaptchaImage
        {
            get;
            private set;
        }

        public string ChallengeText { get; private set; }

        public void Dispose() 
        {
            this.memio.Dispose();
        }
    }


}
