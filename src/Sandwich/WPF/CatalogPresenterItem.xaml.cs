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
        CatalogItem _ci;

        CatalogPresenter my_parent;

        public CatalogPresenterItem(CatalogItem ci, CatalogPresenter parent)
        {
            InitializeComponent();
            
            my_parent = parent;

            if (ci.file != null)
            {
                ci.file.FileLoaded += file_FileLoaded;
                ci.file.BeginThumbnailLoading();

                ContextMenu me = new ContextMenu();
                
                MenuItem a = new MenuItem();
                
                a.Header = "Open Image";
                a.Click += new RoutedEventHandler(a_Click);

                me.Items.Add(a);

                MenuItem b = new MenuItem() { Header = "Copy 4chan URL" };
                b.Click += new RoutedEventHandler(b_Click);

                me.Items.Add(b);

                this.image1.ContextMenu = me;

            }
            else
            {
                this.status.Content = "No image";
                this.gr.Children.Remove(this.image1);
            }

            this.label1.Content = String.Format("R : {0} / I: {1}", ci.text_replies, ci.image_replies);

            if (!string.IsNullOrEmpty(ci.comment))
            {
                this.ToolTip = Common.DecodeHTML(ci.comment);
            }
            _ci = ci;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            string url = "http://boards.4chan.org/{0}/res/{1}#p{1}";
            Clipboard.SetText(String.Format(url, this._ci.board, this._ci.PostNumber));
        }

        void a_Click(object sender, RoutedEventArgs e)
        {
            my_parent.TParent.OpenImage(_ci.ToGenericPost(), false);
        }

        void file_FileLoaded(PostFile sender)
        {
            this.gr.Children.Remove(this.status);
            this.image1.Source = _ci.file.Image; ;
            this.image1.Visibility = System.Windows.Visibility.Visible;
        }

        DateTime a;

        private void image1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                TimeSpan diff = DateTime.Now - a;
                if (diff.TotalMilliseconds <= Common.click_duration)
                {

                    // _
                    if (PictureBoxClicked != null) { PictureBoxClicked(_ci); }
                }
            }
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { a = DateTime.Now; }
        }


        public event Common.ThreadLoadRequest PictureBoxClicked;

        public int PostNumber
        {
            get { return _ci.PostNumber; }
        }

        public string Comment
        {
            get
            {
                if (String.IsNullOrEmpty(_ci.comment))
                {
                    return "";
                }
                else
                {
                    return _ci.comment;
                }
            }
        }

        public string Subject
        {
            get
            {
                if (String.IsNullOrEmpty(_ci.subject))
                {
                    return "";
                }
                else
                {
                    return _ci.subject;
                }
            }
        }



        public DateTime Time { get { return _ci.time; } }

        public int TotalReplies { get { return _ci.TotalReplies; } }

        public DateTime LastReplyTime
        {
            get
            {
                if (_ci.trails != null)
                {
                    return _ci.trails.Last().time;
                }
                else { return _ci.time; }
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
            if (_ci.file != null) 
            {
                _ci.file.Dispose();
            }
        }
    }
}
