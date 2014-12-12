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
        public CatalogPresenter Parent { get; private set; }
        public CatalogItem ItemData { get; private set; }


        public CatalogPresenterItem(CatalogItem ci, CatalogPresenter parent)
        {
            InitializeComponent();

            this.Parent = parent;

            this.ItemData = ci;

            this.image1.Click += (s, e) => { this.Parent.Handle_CatalogItemClicked(this.ItemData); };

            if (ci.file != null)
            {
                ci.file.FileLoaded += file_FileLoaded;
                ci.file.BeginThumbnailLoading();

                ContextMenu me = new ContextMenu();

                MenuItem a = new MenuItem();

                a.Header = "Open Image";
                a.Click += (s, e) =>{ this.Parent.TParent.OpenImage(this.ItemData, false); };

                me.Items.Add(a);

                MenuItem b = new MenuItem() { Header = "Copy URL" };
                b.Click += (s, e) =>
                {
                    Clipboard.SetText(this.Parent.TParent.Chan.GetThreadURL(this.ItemData.board, this.ItemData.PostNumber));
                };

                me.Items.Add(b);

                this.image1.ContextMenu = me;
            }
            else
            {
                this.status.Content = Common.NoThumbImage;
                //this.gr.Children.Remove(this.image1);
            }

            this.label1.Content = String.Format("R : {0} / I: {1}", this.ItemData.text_replies, this.ItemData.image_replies);

            if (!string.IsNullOrEmpty(ci.comment))
            {
                this.ToolTip = Common.DecodeHTML(this.ItemData.comment);
            }

        }

        private void file_FileLoaded(PostFile sender)
        {
            this.gr.Children.Remove(this.status);
            this.image1.Source = this.ItemData.file.Image; ;
            this.image1.Visibility = System.Windows.Visibility.Visible;
        }

        public DateTime LastReplyTime
        {
            get
            {
                if (this.ItemData.trails != null)
                {
                    return this.ItemData.trails.Last().time;
                }
                else { return this.ItemData.time; }
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
            if (this.ItemData.file != null)
            {
                this.ItemData.file.FileLoaded -= this.file_FileLoaded;
                this.ItemData.file.Dispose();
            }
        }
    }
}
