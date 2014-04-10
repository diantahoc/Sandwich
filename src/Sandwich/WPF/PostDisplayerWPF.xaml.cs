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
using Sandwich.WPF;

namespace Sandwich
{
    /// <summary>
    /// Interaction logic for PostDisplayerWPF.xaml
    /// </summary>
    public partial class PostDisplayerWPF : UserControl
    {
        private GenericPost _gp;
        public int PostID { get { return this._gp.PostNumber; } }

        public PostDisplayerWPF()
        {
            InitializeComponent();
        }

        public void Init(GenericPost gp)
        {
            _gp = gp;

            if (gp.file != null)
            {
                if (gp.file.IsSpoiler) { init_spoiler(); }
                gp.file.FileLoaded += new PostFile.ImageLoadedEvent(file_FileLoaded);
                gp.file.BeginThumbnailLoading();

                // filename.ext, (WxH) - Size
                this.fileinfoPanel.Text = string.Format("{0}.{1}, ({2}x{3}), {4}", gp.file.filename, gp.file.ext, gp.file.width, gp.file.height, Common.format_size_string(gp.file.size));
                this.pictureBoxWPF1.ContextMenu = get_file_context_menu();
            }
            else
            {
                this.content.Children.Remove(this.pictureboxcontainer);
                this.content.ColumnDefinitions.RemoveAt(0);
            }

            //post info loading

            //post menu

            ClickableLabel option_menu = new ClickableLabel()
            {
                Content = "[▼]",
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Black,
                Cursor = Cursors.Hand,
                ContextMenu = new ContextMenu()
            };

            option_menu.Click += (s, e) =>
            {
                ((ClickableLabel)s).ContextMenu.IsOpen = true;
            };

            MenuItem report_menuitem = new MenuItem() { Header = "Report" };
            report_menuitem.Click += (s, e) =>
            {
                if (ReportClicked != null) { ReportClicked(this._gp.PostNumber); }
            };

            MenuItem delete_menuitem = new MenuItem() { Header = "Delete" };
            delete_menuitem.Click += (s, e) =>
            {
                if (DeleteClicked != null) { DeleteClicked(this._gp.PostNumber); }
            };

            option_menu.ContextMenu.Items.Add(report_menuitem);
            option_menu.ContextMenu.Items.Add(delete_menuitem);

            this.postinfoPanel.Children.Add(option_menu);

            if (!String.IsNullOrEmpty(gp.country_flag))
            {
                this.postinfoPanel.Children.Add(new Image()
                {
                    ToolTip = gp.country_name,
                    Height = 11d,
                    Width = 16d,
                    Source = Core.LoadFlag(gp.country_flag, gp.board)
                });
            }

            //subject
            if (!String.IsNullOrEmpty(gp.subject))
            {
                this.postinfoPanel.Children.Add(get_label(gp.subject, ElementsColors.SubjectTextColor));
            }

            //name
            this.postinfoPanel.Children.Add(get_label(gp.name, ElementsColors.NameColor));

            //trip
            if (!String.IsNullOrEmpty(gp.trip))
            {
                this.postinfoPanel.Children.Add(get_label(gp.trip, ElementsColors.TripCodeColor));
            }

            this.postinfoPanel.Children.Add(get_label(gp.time.ToString(), ElementsColors.DateTimeColor));


            ClickableLabel post_number = new ClickableLabel()
            {
                Content = "No. " + gp.PostNumber.ToString(),
                Foreground = ElementsColors.PostIDColor,
                Cursor = Cursors.Hand,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            post_number.Click += (s, e) => { if (PostTitleClicked != null) { PostTitleClicked(_gp); } };
            this.postinfoPanel.Children.Add(post_number);

            if (String.IsNullOrEmpty(gp.comment))
            {
                this.content.Children.Remove(this.postTextRenderer1);
            }
            else
            {
                this.postTextRenderer1.QuoteClicked = this.QuoteClicked;

                postTextRenderer1.Render(gp.CommentTokens);
            }

            //backlinks
            foreach (int i in gp.QuotedBy)
            {
                this.postinfoPanel.Children.Add(get_quoted_by_label(i));
            }
        }

        private ClickableLabel get_quoted_by_label(int i)
        {
            ClickableLabel l = new ClickableLabel()
            {
                Content = "->" + i.ToString(),
                Foreground = ElementsColors.QuoteTextColor,
                Tag = i,
                Cursor = Cursors.Hand,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            l.Click += (s, e) =>
            {
                if (this.QuoteClicked != null)
                {
                    this.QuoteClicked(Convert.ToInt32(((ClickableLabel)s).Tag));
                }
            };
            return l;
        }

        #region File context menu

        private ContextMenu get_file_context_menu()
        {
            ContextMenu pMenu = new ContextMenu();

            MenuItem item1 = new MenuItem();
            item1.Header = "Filter...";

            MenuItem item3 = new MenuItem();
            item3.Header = "By Image Hash";

            MenuItem item4 = new MenuItem();
            item4.Header = "By file name";

            item1.Items.Add(item3);
            item1.Items.Add(item4);

            pMenu.Items.Add(item1);

            return pMenu;
        }

        #endregion

        private void file_FileLoaded(PostFile sender)
        {
            this.pictureBoxWPF1.SetImage(_gp.file.ext, _gp.file.Image);

            this.pictureBoxWPF1.ImageClicked += this.ImageClicked;
            this.pictureBoxWPF1.ThumnailMode(_gp);
        }

        public Common.ImageClickEvent ImageClicked;
        public Common.QuoteClickEvent QuoteClicked;
        public Common.PostTitleClickEvent PostTitleClicked;

        public delegate void MDMenuClickedEvent(int id);

        public MDMenuClickedEvent ReportClicked;
        public MDMenuClickedEvent DeleteClicked;

        private Label get_label(string text, Brush color)
        {
            return new Label()
            {
                Foreground = color,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = text,
            };
        }

        private void init_spoiler()
        {
            this.pictureBoxWPF1.Visibility = System.Windows.Visibility.Collapsed;
            this.spoiler.Visibility = System.Windows.Visibility.Visible;
            this.pictureboxcontainer.MouseEnter += (s, e) =>
            {
                this.pictureBoxWPF1.Visibility = System.Windows.Visibility.Visible;
                this.spoiler.Visibility = System.Windows.Visibility.Collapsed;
            };
            this.pictureboxcontainer.MouseLeave += (s, e) =>
            {
                this.pictureBoxWPF1.Visibility = System.Windows.Visibility.Collapsed;
                this.spoiler.Visibility = System.Windows.Visibility.Visible;
            };
            this.spoiler.Source = BoardInfo.GetSpoilerImage(this._gp.board);
        }

        public void UpdateQuotedBy(int[] i)
        {
            if (i != null)
            {
                foreach (int a in i)
                {
                    if (!this._gp.QuotedBy.Contains(a))
                    {
                        this._gp.MarkAsQuotedBy(a);
                        this.postinfoPanel.Children.Add(get_quoted_by_label(a));
                    }
                }
            }
        }

        public void Unfocus()
        {
            this.Effect = null;
        }

        public new void Focus()
        {
            this.Effect = ElementsColors.FocusedPostEffect;
        }

        public bool HasFile { get { return this._gp.file != null; } }

        public void MarkFileAsDeleted()
        {
            this.postinfoPanel.Children.Insert(0, get_label("[File Deleted]", Brushes.Red));
        }

    }
}
