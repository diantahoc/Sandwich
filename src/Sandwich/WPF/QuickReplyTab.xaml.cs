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
using System.Threading.Tasks;

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for QuickReplyTab.xaml
    /// </summary>
    public partial class QuickReplyTab : UserControl
    {
        public int ID { get; private set; }

        public Chans.IChan Chan { get; private set; }
        public BoardInfo Board { get; private set; }

        public Common.PostingMode Mode { get; private set; }

        private CaptchaProvider cp;

        private string current_captcha_challenge = "";

        private string selected_file_path = "";

        bool is_sending = false;

        public QuickReplyTab(Common.PostingMode mode, BoardInfo board, int threadid)
        {
            InitializeComponent();
            this.ID = threadid;
            this.Mode = mode;
            this.Board = board;
            this.Chan = board.Chan;

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


            if (this.Chan.RequireCaptcha)
            {
                cp = new CaptchaProvider();
                cp.CaptchaLoaded += new CaptchaProvider.CaptchaLoadedEvent(cp_CaptchaLoaded);
                cp.CaptchaError += new CaptchaProvider.CaptchaLoadErrorEvent(cp_CaptchaError);
                refresh_captcha();
            }
            else
            {
                HideCaptchaUI();
            }

        }

        private void HideCaptchaUI()
        {
            this.captcha_row.Visibility = System.Windows.Visibility.Collapsed;
            this.captchaResponse.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ShowCaptchaUI()
        {
            this.captcha_row.Visibility = System.Windows.Visibility.Visible;
            this.captchaResponse.Visibility = System.Windows.Visibility.Visible;
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
            foreach (string file_ext in this.Board.Files)
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

        private void SendRequest()
        {
            if (is_sending)
            {
                Core.ShowMessage("A request is being sended", Core.MessageType.Info);
            }
            else
            {

                if (verify_errors())
                {
                    var data = new Chans.RequestPosterData()
                    {
                        Board = this.Board,
                        CaptchaRequired = this.Chan.RequireCaptcha,
                        Captcha = GetCaptchaResponse(),
                        Comment = this.shb.Text,
                        Email = this.emailBox.Text,
                        IsReply = (this.Mode == Common.PostingMode.Reply),
                        Name = this.nameBox.Text,
                        Subject = this.subBox.Text,
                        PostPassword = "sw123456",
                        ThreadID = this.ID
                    };

                    //send message
                    Task.Factory.StartNew((Action)delegate
                    {
                        try
                        {

                            //Disable the UI
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                switch_controls(false);
                            });

                            //send data synchronously
                            is_sending = true;

                            var response = this.Chan.SendPost(data);

                            is_sending = false;

                            //Enable the UI
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                switch_controls(true);
                            });

                            if (response.ResponseStatus == Core.PostSendingStatus.Success)
                            {
                                this.Dispatcher.Invoke((Action)delegate
                                {
                                    Button_Click(null, null); // Reset fields, this stimulate thte reset button press.

                                    if (OpenThread != null && this.Mode == Common.PostingMode.NewThread)
                                    {
                                        OpenThread(response.AdditionalData.Key);
                                    }

                                    Core.ShowMessage("Post successful!", Core.MessageType.Success);
                                });


                            }
                            else
                            {
                                string debug_log = System.IO.Path.Combine(Core.log_dir, string.Format("RequestPoster-ResponseHTML-{0}-{1}-{2).log",
                                    this.Chan.Name, this.ID, DateTime.Now.Ticks));

                                System.IO.File.WriteAllText(debug_log, response.ResponseHTML);

                                this.Dispatcher.Invoke((Action)delegate
                                {
                                    Core.ShowMessage(string.Format("Cannot send request: '{0}'", response.ResponseStatus), Core.MessageType.Error);
                                });
                            }


                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                Core.ShowMessage(string.Format("Cannot send request: '{0}'", ex.Message), Core.MessageType.Error);
                                switch_controls(true);
                            });
                        }


                    });//Sender thread
                }
            }
        }

        private SolvedCaptcha GetCaptchaResponse()
        {
            return new SolvedCaptcha(current_captcha_challenge, this.captchaResponse.Text.Trim().Split(' ').Length == 1 ? string.Format("{0} {0}", this.captchaResponse.Text.Trim().Split(' ').First()) : this.captchaResponse.Text);
        }

     

        private bool verify_errors()
        {
            if (this.Chan.RequireCaptcha && string.IsNullOrWhiteSpace(this.captchaResponse.Text))
            {
                Core.ShowMessage("You must type the captcha.", Core.MessageType.Error);
                return false;
            }

            if (!string.IsNullOrEmpty(this.selected_file_path) && string.IsNullOrEmpty(this.filename.Text))
            {
                Core.ShowMessage("Please type a file name.", Core.MessageType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(this.selected_file_path) && string.IsNullOrEmpty(this.shb.Text))
            {
                Core.ShowMessage("You cannot blank post.", Core.MessageType.Warning);
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

        private void actionText_Click(object sender, RoutedEventArgs e)
        {
            SendRequest();
        }
    }
}
