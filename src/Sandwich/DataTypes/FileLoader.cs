using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Sandwich.DataTypes
{
    public class FileLoader : IDisposable
    {

        public string Url { get; private set; }
        public string FileName { get; private set; }
        public string Board { get; private set; }
        public string Extension { get; private set; }

        public BitmapImage Image { get; private set; }
        public byte[] ImageData { get; private set; }

        private bool is_busy = false;

        public FileLoader(PostFile pf)
        {
            this.Url = pf.FullImageLink;
            this.FileName = string.Format("{0}-{1}.{2}", pf.filename, pf.thumbnail_tim, pf.ext);
            this.Board = pf.board;
            this.Extension = pf.ext;
        }

        public void LoadImageAsync()
        {
            if (!is_busy)
            { LoadImageAsync(true); }
        }

        public void LoadImageAsync(bool use_cache)
        {
            if (!is_busy)
            {
                System.Threading.Tasks.Task.Factory.StartNew((Action)delegate
                {
                    is_busy = true;

                    bool wait = true;

                    Core.QueueAction((Action)delegate
                    {
                        try
                        {
                            this.ImageData = Core.LoadURL(this.Url, this.FileProgressChanged, use_cache);
                        }
                        catch (System.Net.WebException wex)
                        {
                            if (wex.Status == System.Net.WebExceptionStatus.Timeout |
                                wex.Status == System.Net.WebExceptionStatus.SendFailure |
                                wex.Status == System.Net.WebExceptionStatus.ConnectFailure)
                            {
                                try
                                {
                                    //try again
                                    this.ImageData = Core.LoadURL(this.Url, this.FileProgressChanged, use_cache);
                                }
                                catch (Exception)
                                {
                                    this.ImageData = null;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            this.ImageData = null;
                        }
                        wait = false;
                    }, Amib.Threading.WorkItemPriority.AboveNormal);

                    while (wait)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                    if (this.ImageData != null)
                    {
                        MemoryStream memio = new MemoryStream(ImageData);
                        try
                        {
                            BitmapImage bbb = new BitmapImage();

                            bbb.BeginInit();

                            bbb.CacheOption = BitmapCacheOption.OnLoad;

                            bbb.StreamSource = memio;

                            bbb.EndInit();

                            bbb.Freeze();

                            this.Image = bbb;
                        }
                        catch (Exception)
                        {
                            this.Image = Common.ErrorImage;
                            memio.Dispose();
                        }
                    }
                    if (FileLoaded != null) { FileLoaded(); }
                    is_busy = false;
                });
            }
        }

        public delegate void FileLoadedEvent();

        public event FileLoadedEvent FileLoaded;

        public event Common.ProgressChanged FileProgressChanged;

        public void Dispose()
        {
            if (this.Image != null)
            {
                if (this.Image.StreamSource != null) { this.Image.StreamSource.Dispose(); }
                this.Image = null;
            }
            this.ImageData = null;
            this.FileProgressChanged = null;
            this.FileLoaded = null;
            GC.Collect();
        }

    }
}
