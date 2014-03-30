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

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for QuickReplyTab.xaml
    /// </summary>
    public partial class QuickReplyTab : UserControl
    {

        public int ID { get; private set; }
        public string Board { get; private set; }
        public Common.PostingMode Mode { get; private set; }

        private CaptchaProvider cp;

        private RequestPoster rp;

        private string current_captcha_challenge = "";

        private string selected_file_path = "";

        bool is_sending = false;

        public QuickReplyTab(Common.PostingMode mode, string board, int threadid)
        {
            InitializeComponent();
            this.ID = threadid;
            this.Mode = mode;
            this.Board = board;

            if (mode == Common.PostingMode.NewThread)
            {
                this.actionText.Content = "New thread";
            }

            shb.IsLineNumbersMarginVisible = false;

            try
            {
                shb.CurrentHighlighter = AurelienRibon.Ui.SyntaxHighlightBox.HighlighterManager.Instance.Highlighters["QR"];
            }
            catch (Exception) { }


            cp = new CaptchaProvider();
            cp.CaptchaLoaded += new CaptchaProvider.CaptchaLoadedEvent(cp_CaptchaLoaded);
            cp.CaptchaError += new CaptchaProvider.CaptchaLoadErrorEvent(cp_CaptchaError);
            refresh_captcha();


            rp = new RequestPoster(board);

            rp.SendCompleted += new RequestPoster.DataSendCompleteEvent(rp_SendCompleted);
            rp.SendingProgressChanged += new Common.ProgressChanged(rp_SendingProgressChanged);
            rp.SendError += new RequestPoster.SendErrorEvent(rp_SendError);
        }

        #region Captcha UI

        void cp_CaptchaError(Exception c)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.refreshBtn.IsEnabled = false;
            });
        }

        void cp_CaptchaLoaded(Captcha c)
        {
            this.capthaImage.Source = c.CaptchaImage;
            current_captcha_challenge = c.ChallengeText;
            refreshBtn.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            refresh_captcha();
        }

        private void refresh_captcha()
        {
            refreshBtn.IsEnabled = false;
            cp.GetChallenge();
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.nameBox.Text = "";
            this.emailBox.Text = "";
            this.subBox.Text = "";
            this.shb.Text = "";
            clear_file();
        }

        #region File Selection

        private string get_board_supported_files()
        {
            StringBuilder s = new StringBuilder();
            foreach (string file_ext in BoardInfo.GetBoardFiles(this.Board))
            {
                s.AppendFormat("*.{0};", file_ext);
            }
            return string.Format("/{0}/ supported files|{1}", this.Board, s.ToString());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog())
            {
                of.CheckFileExists = true;
                of.CheckPathExists = true;
                of.Multiselect = false;
                of.Filter = get_board_supported_files();

                if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selected_file_path = of.FileName;
                    this.filename.Text = of.FileName.Split('\\').Last();


                    //try and load image
                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(of.FileName)))
                    {
                        BitmapImage bbb = new BitmapImage();

                        bbb.BeginInit();

                        bbb.CacheOption = BitmapCacheOption.OnLoad;

                        bbb.StreamSource = mem;

                        bbb.EndInit();

                        bbb.Freeze();

                        this.filePreview.Source = CreateResizedImage(bbb, 200, 90, 1);
                        this.filePreview.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.Linear);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            clear_file();
        }

        private void clear_file()
        {
            this.filePreview.ClearValue(Image.SourceProperty);
            this.selected_file_path = "";
            this.filename.Text = "";
            this.filePreview.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion

        #region Post Sending

        private void actionText_Click(object sender, RoutedEventArgs e)
        {
            if (is_sending)
            {
                MessageBox.Show("A request is being sended");
                return;
            }
            else
            {
                if (!verify_errors()) { return; }
                switch_controls(false);

                rp.SendRequest(new RequestPosterData()
                {
                    comment = this.shb.Text,
                    email = this.emailBox.Text,
                    file_name = this.filename.Text,
                    file_path = selected_file_path,
                    is_pass = false,
                    is_reply = (this.Mode == Common.PostingMode.Reply),
                    name = this.nameBox.Text,
                    password = "123456",
                    Captcha = new SolvedCaptcha(current_captcha_challenge, this.captchaResponse.Text.Trim().Split(' ').Length == 1 ? string.Format("{0} {0}", this.captchaResponse.Text.Trim().Split(' ').First()) : this.captchaResponse.Text),
                    subject = this.subBox.Text,
                    tid = this.ID
                });

                this.progress_modal.Visibility = System.Windows.Visibility.Visible;
                is_sending = true;
            }
        }

        void rp_SendCompleted(RequestPosterResponse response)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                is_sending = false;
                switch_controls(true);
                this.progress_modal.Visibility = System.Windows.Visibility.Collapsed;

                if (response.Status == RequestPosterResponse.ResponseStatus.Success)
                {
                    Button_Click(null, null);//reset elements

                    if (OpenThread != null & this.Mode == Common.PostingMode.NewThread)
                    {
                        OpenThread(Convert.ToInt32(response.ResponseBody.Split(':')[1]));
                    }

                }
                else
                {
                    string debug_log = System.IO.Path.Combine(Core.log_dir, this.ID.ToString());

                    System.IO.File.WriteAllText(debug_log, response.ResponseBody);
                }

                MessageBox.Show(response.Status.ToString());

                is_sending = false;
            });
        }

        void rp_SendError(Exception ex)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.progress_modal.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show(string.Format("Cannot send request: '{0}'", ex.Message), "Error");
            });
        }

        void rp_SendingProgressChanged(double p)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.progress_bar.IsIndeterminate = (p <= 0);
                this.progress_bar.Value = p * 100;
            });
        }


        private bool verify_errors()
        {
            if (string.IsNullOrEmpty(this.captchaResponse.Text) | string.IsNullOrWhiteSpace(this.captchaResponse.Text))
            {
                MessageBox.Show("You must type the captcha.");
                return false;
            }

            if (!string.IsNullOrEmpty(this.selected_file_path) & string.IsNullOrEmpty(this.filename.Text))
            {
                MessageBox.Show("Please type a file name.");
                return false;
            }

            if (string.IsNullOrEmpty(this.selected_file_path) & string.IsNullOrEmpty(this.shb.Text))
            {
                MessageBox.Show("You cannot blank post.");
                return false;
            }

            return true;
        }

        private void switch_controls(bool a)
        {
            this.nameBox.IsEnabled = a;
            this.shb.IsEnabled = a;
            this.captchaResponse.IsEnabled = a;
            this.filename.IsEnabled = a;
            this.subBox.IsEnabled = a;
            this.emailBox.IsEnabled = a;
            this.refreshBtn.IsEnabled = a;
            this.resetBtn.IsEnabled = a;
            this.actionText.IsEnabled = a;
            this.selectfileBtn.IsEnabled = a;
            this.clearfileBtn.IsEnabled = a;
        }

        public delegate void OpenThreadEvent(int id);
        public event OpenThreadEvent OpenThread;

        #endregion

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (nameBox.Text.Contains("#"))
            {
                nameBox.Background = Brushes.Red;
            }
            else
            {
                nameBox.Background = Brushes.White;
            }
        }

        private void emailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (emailBox.Text == "sage")
            {
                emailBox.Background = Brushes.Yellow;
            }
            else
            {
                emailBox.Background = Brushes.White;
            }
        }
    }
}
