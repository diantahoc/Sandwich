using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;

namespace Sandwich
{
    /// <summary>
    /// Interaction logic for CatalogPresenterItem.xaml
    /// </summary>
    public partial class CatalogPresenterItem : UserControl, IDisposable
    {
        CatalogPresenter my_parent;

        static string url = "http://boards.4chan.org/{0}/res/{1}#p{1}";

        public CatalogPresenterItem(CatalogItem ci, CatalogPresenter parent)
        {
            InitializeComponent();

            my_parent = parent;

            this.CatI = ci;

            this.image1.Click += (s, e) => { if (PictureBoxClicked != null) { PictureBoxClicked(this.CatI); } };

            if (ci.file != null)
            {
                ci.file.FileLoaded += file_FileLoaded;
                ci.file.BeginThumbnailLoading();

                ContextMenu me = new ContextMenu();

                MenuItem a = new MenuItem();

                a.Header = "Open Image";
                a.Click += (s, e) => { my_parent.TParent.OpenImage(this.CatI, false); };

                me.Items.Add(a);

                MenuItem b = new MenuItem() { Header = "Copy 4chan URL" };
                b.Click += (s, e) => { Clipboard.SetText(String.Format(url, this.CatI.board, this.CatI.PostNumber)); };

                me.Items.Add(b);

                this.image1.ContextMenu = me;
            }
            else
            {
                this.status.Content = "No image";
                this.gr.Children.Remove(this.image1);
            }

            this.label1.Content = String.Format("R : {0} / I: {1}", this.CatI.text_replies, this.CatI.image_replies);

            if (!string.IsNullOrEmpty(ci.comment))
            {
                this.ToolTip = Common.DecodeHTML(this.CatI.comment);
            }

        }

        private void file_FileLoaded(PostFile sender)
        {
            this.gr.Children.Remove(this.status);
            this.image1.Source = this.CatI.file.Image; ;
            this.image1.Visibility = System.Windows.Visibility.Visible;
        }

        public event Common.ThreadLoadRequest PictureBoxClicked;

        public CatalogItem CatI { get; private set; }

        public DateTime LastReplyTime
        {
            get
            {
                if (this.CatI.trails != null)
                {
                    return this.CatI.trails.Last().time;
                }
                else { return this.CatI.time; }
            }
        }

        public void ChangeSize(double s)
        {
            this.SetValue(HeightProperty, s);
            this.SetValue(WidthProperty, s);

            this.gr.Height = s;
            this.gr.Width = s;

            this.gr.MaxHeight = s;
            this.gr.MaxWidth = s;

            this.label1.Width = s;
            this.label1.Height = 50d;

            this.image1.Width = s;
            this.image1.Height = s - 50;
        }

        public void Dispose()
        {
            if (this.CatI.file != null)
            {
                this.CatI.file.FileLoaded -= this.file_FileLoaded;
                this.CatI.file.Dispose();
            }
        }
    }
}
