using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Rss;
namespace Sandwich
{
    class BoardRss : IDisposable
    {

        private string board;
        private BackgroundWorker bg;

        private string link = "http://boards.4chan.org/$/index.rss"; 

        public BoardRss(string b) 
        {
            board = b;
            bg = new BackgroundWorker();
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
        }

        public bool IsBusy { get { return bg.IsBusy; } }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                RssFeed feed = RssFeed.Read(link.Replace("$", board));

                List<RssItem> dic = new List<RssItem>();

                foreach (RssChannel cha in feed.Channels)
                {
                    foreach (RssItem i in cha.Items)
                    {
                        dic.Add(i);
                    }
                }
                e.Result = dic;
            }
            //catch (System.Net.WebException wex)
            //{
            //    if (wex.Status == System.Net.WebExceptionStatus.Timeout ||
            //        wex.Status == System.Net.WebExceptionStatus.ConnectFailure || 
            //        wex.Status == System.Net.WebExceptionStatus.ConnectionClosed) 
            //    {
            // maybe retry
            //    }

            //}
            catch (Exception) { e.Cancel = true; return; }

            return;
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (RssLoaded != null)
                {
                    if (e.Result.GetType() == typeof(List<RssItem>))
                    {
                        RssLoaded(((List<RssItem>)e.Result));
                    }
                }
            }
        }

        public void Load() 
        {
            if (!bg.IsBusy) 
            {
                bg.RunWorkerAsync();
            }
        }

        public delegate void RssLoadedEvent(List<RssItem> dic);
        public event RssLoadedEvent RssLoaded;


        public void Dispose() 
        {
            this.bg.Dispose();
            this.RssLoaded = null;
        }
    }

}
