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
using System.IO;
using WpfAnimatedGif;

namespace Sandwich
{
    /// <summary>
    /// Interaction logic for PictureBoxWPF.xaml
    /// </summary>
    public partial class PictureBoxWPF : UserControl
    {
        public PictureBoxWPF()
        {
            InitializeComponent();
        }

        private Image im;

        public void SetImage(string ext, BitmapImage image)
        {
            im = new Image();
            im.Stretch = Stretch.Uniform;

            if (ext == "gif")
            {
                ImageBehavior.SetAnimatedSource(im, image);
                ImageBehavior.SetAutoStart(im, true);
                ImageBehavior.SetRepeatBehavior(im, System.Windows.Media.Animation.RepeatBehavior.Forever);

                ContextMenu pMenu = new ContextMenu();
                MenuItem item1 = new MenuItem();

                item1.Header = "Play/Pause";

                item1.Click += (s, e) =>
                {
                    MenuItem sender = (MenuItem)s;

                    ImageAnimationController controller = ImageBehavior.GetAnimationController(im);
                    if (controller != null)
                    {
                        if (controller.IsPaused)
                        {
                            controller.Play();
                            sender.Header = "Pause";
                        }
                        else
                        {
                            controller.Pause();
                            sender.Header = "Play";
                        }
                    }
                };

                pMenu.Items.Add(item1);

                im.ContextMenu = pMenu;
            }
            else
            {
                im.Source = image;
            }

            this.Content = im;
        }

        public void ClearImage() 
        {
            if (im != null) 
            {
                im.ClearValue(Image.SourceProperty);
            }
        }

        public void SetSize(Size s)
        {
            if (im != null)
            {
                im.SetValue(MaxHeightProperty, s.Height);
                im.SetValue(MaxWidthProperty, s.Width);
            }
        }

        private GenericPost _gp;

        public void ThumnailMode(GenericPost gp)
        {
            im.ContextMenu = null;

            im.MaxHeight = Convert.ToDouble(gp.file.thumbH);
            im.MaxWidth = Convert.ToDouble(gp.file.thumbW);

            if (gp.file.FullImageLink != "nofile")
            {
                this.Cursor = Cursors.Hand;
                im.MouseLeftButtonDown += new MouseButtonEventHandler(q_MouseDown);
                im.MouseLeftButtonUp += new MouseButtonEventHandler(q_MouseUp);
            }

            _gp = gp;
        }

        DateTime left;
        // DateTime right;

        void q_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ImageClicked != null)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    if ((DateTime.Now - left).TotalMilliseconds <= 150)
                    {
                        ImageClicked(_gp, false);
                    }
                }
                if (e.MiddleButton == MouseButtonState.Released)
                {
                    if ((DateTime.Now - left).TotalMilliseconds <= 150)
                    {
                        ImageClicked(_gp, false);
                    }
                }
                // if (e.RightButton == MouseButtonState.Pressed) { }
            }
        }

        void q_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                left = DateTime.Now;
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                left = DateTime.Now;
            }
            // if (e.RightButton == MouseButtonState.Pressed) { }
        }


        public event Common.ImageClickEvent ImageClicked;

    }
}
