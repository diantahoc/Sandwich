using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows.Media.Imaging;


namespace Sandwich
{
    public class PostFile : IDisposable
    {
        private BackgroundWorker ThumbImageLoader;

        private MemoryStream memIO;

        public string filename { get; set; }
        public string hash { get; set; }
        public string ext { get; set; }
        public int thumbH { get; set; }
        public int thumbW { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int size { get; set; }
        public string thumbnail_tim { get; set; }
        public bool IsSpoiler { get; set; }
        public string board { get; set; }

        public int owner { get; set; }

        private BitmapImage _image = null;

        private bool thumb_worker_cancelled = false;

        private bool thumb_spin_lock = true;

        private byte[] thumb_data;

        private void init_thumb_bg()
        {
            if (this.ThumbImageLoader == null)
            {
                this.ThumbImageLoader = new BackgroundWorker();
                this.ThumbImageLoader.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
                this.ThumbImageLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
                this.ThumbImageLoader.WorkerSupportsCancellation = true;
            }
        }

        #region Thumb_loading

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (FileLoaded != null && !e.Cancelled)
            {
                FileLoaded(this);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.ThumbLink))
            {
                thumb_worker_cancelled = false;

                Core.QueueAction((Action)(this.thumb_thread_queue_order));

                //Keep waiting until this lock is set to false (when the data is loaded)
                while (thumb_spin_lock)
                {
                    System.Threading.Thread.Sleep(1000);
                }

                if (!thumb_worker_cancelled)
                {
                    if (thumb_data != null)
                    {
                        memIO = new MemoryStream(thumb_data);
                        try
                        {
                            BitmapImage bbb = new BitmapImage();

                            bbb.BeginInit();

                            bbb.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;

                            bbb.CacheOption = BitmapCacheOption.OnLoad;

                            bbb.StreamSource = memIO;

                            bbb.EndInit();
                            bbb.Freeze();

                            _image = bbb;
                        }
                        catch (Exception)
                        {
                            _image = Common.ErrorImage;
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void thumb_thread_queue_order()
        {
            try
            {
                this.thumb_data = Core.LoadURL(this.ThumbLink, null, true);
            }
            catch (System.Net.WebException wex)
            {
                if (wex.Status == System.Net.WebExceptionStatus.Timeout ||
                    wex.Status == System.Net.WebExceptionStatus.SendFailure ||
                    wex.Status == System.Net.WebExceptionStatus.ConnectFailure)
                {
                    try
                    {
                        //try again
                        this.thumb_data = Core.LoadURL(this.ThumbLink, null, true);
                    }
                    catch (Exception)
                    {
                        //secondary fail, abort
                        this.thumb_data = null;
                    }
                }
            }
            catch (Exception)
            {
                this.thumb_data = null;
            }
            this.thumb_spin_lock = false;
        }

        public void BeginThumbnailLoading()
        {
            if (this.ThumbLink == "")
            {
                this._image = Common.NoThumbImage;
                if (FileLoaded != null)
                {
                    FileLoaded(this);
                }
            }
            else
            {
                init_thumb_bg();
                if (!ThumbImageLoader.IsBusy) { ThumbImageLoader.RunWorkerAsync(); }
            }
        }

        #endregion


        #region Properties

        public BitmapImage Image
        {
            get
            {
                if (_image == null)
                {
                    throw new Exception("Thumbnail image has not been loaded");
                }
                else
                {
                    return _image;
                }
            }
        }

        private string custom_t_link = "";//HACK

        public string ThumbLink
        {
            get
            {
                if (custom_t_link == "")
                {
                    if (this.board == "f")
                    {
                        return "";
                    }
                    else
                    {
                        return Core.thumbLink.Replace("#", this.board).Replace("$", this.thumbnail_tim);
                    }
                }
                else
                {
                    return custom_t_link;
                }
            }
            set { custom_t_link = value; }
        }

        private string custom_full_link = ""; //HACK

        public string FullImageLink
        {
            get
            {
                if (custom_full_link == "")
                {
                    if (this.board == "f")
                    {
                        return Core.imageLink.Replace("#", this.board).Replace("$", this.filename + "." + this.ext);
                    }
                    else
                    {
                        return Core.imageLink.Replace("#", this.board).Replace("$", this.thumbnail_tim + "." + this.ext);
                    }
                }
                else
                {
                    return custom_full_link;
                }
            }
            set { custom_full_link = value; }
        }
        #endregion

        #region Events

        public delegate void ImageLoadedEvent(PostFile sender);

        public event ImageLoadedEvent FileLoaded;


        #endregion

        #region Methods

        public void Dispose()
        {
            thumb_data = null;

            if (memIO != null)
            {
                memIO.Dispose();
            }

            if (this.ThumbImageLoader != null) { this.ThumbImageLoader.Dispose(); }
        }


        #endregion
    }
}
