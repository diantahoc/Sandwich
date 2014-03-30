using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sandwich
{
    public partial class ReplyForm : Form
    {
        private int _mode; // 0 is new thread, 1 is reply
        private int _tid;

        private string captcha_challenge;

        private CaptchaProvider cp;

        private Captcha currentCaptcha = null;

        private RequestPoster RPoster;

        private string selected_file = "";

        private bool is_sending = false;

        public WPF.BoardBrowserWPF BBW { get; set; }


        public ReplyForm(int mode, int tid, string board)
        {
            InitializeComponent();
            _mode = mode;
            _tid = tid;
            if (_mode == 0)
            {
                this.Text = "New thread";
            }
            else
            {
                this.Text = "Thread No. " + _tid.ToString();
            }

            cp = new CaptchaProvider();
            cp.CaptchaLoaded += new CaptchaProvider.CaptchaLoadedEvent(handle_cp_imageload);
            cp.CaptchaError += new CaptchaProvider.CaptchaLoadErrorEvent(cp_CaptchaError);
            RPoster = new RequestPoster(board);
            RPoster.SendCompleted += new RequestPoster.DataSendCompleteEvent(RPoster_SendCompleted);
        }

        void cp_CaptchaError(Exception c)
        {
            return;
        }

        void RPoster_SendCompleted(RequestPosterResponse response)
        {
            if (response.Status == RequestPosterResponse.ResponseStatus.Success)
            {
                com_f.Text = "";
                this.Hide();

                if (_mode == 0)
                {
                    BBW.OpenThread(Convert.ToInt32(response.ResponseBody.Split(':')[1]));
                }
            }
            else 
            {
                System.IO.File.WriteAllText(@"C:\Users\Istan\Desktop\test.html", response.ResponseBody);
            }
            MessageBox.Show(response.Status.ToString());
            is_sending = false;
            unlock_all();
        }

        private void ReplyForm_Shown(object sender, EventArgs e)
        {
            switch (_mode)
            {
                case 0:
                    submit_button.Text = "New thread";
                    break;
                case 1:
                    submit_button.Text = "Reply";
                    break;
                default:
                    submit_button.Text = "Submit";
                    break;
            }
            cp.GetChallenge();
        }

        private void handle_cp_imageload(Captcha c)
        {
            if (currentCaptcha != null)
            {
                pictureBox1.Image = null;
                currentCaptcha.Dispose();
                currentCaptcha = c;
                //pictureBox1.Image = currentCaptcha.CaptchaImage;
                captcha_challenge = currentCaptcha.ChallengeText;
            }
            else
            {
                currentCaptcha = c;
                //pictureBox1.Image = currentCaptcha.CaptchaImage;
                captcha_challenge = currentCaptcha.ChallengeText;
            }
            button3.Enabled = true;
            cap_f.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            refresh_capt();
        }

        private void refresh_capt()
        {
            cp.GetChallenge();
            if (!is_sending)
            {
                button3.Enabled = false;
                cap_f.Enabled = false;
            }
            cap_f.Text = "";
        }

        private void submit_button_Click(object sender, EventArgs e)
        {
            if (is_sending) { MessageBox.Show("Sending is in progress", "Info"); }
            if (!verify_errors()) { return; }

            lock_all();

            RequestPosterData rp = new RequestPosterData();

            rp.name = name_f.Text;
            rp.email = email_f.Text;
            rp.subject = sub_f.Text;
            rp.re_captcha_challenge = captcha_challenge;
            rp.re_captcha_responce = cap_f.Text;
            rp.tid = _tid;
            if (selected_file != "")
            {
                rp.file_path = selected_file;
                rp.file_name = f_name.Text;
            }
            rp.comment = com_f.Text;
            rp.password = password_f.Text;

            rp.is_reply = (_mode == 1);

            RPoster.SendRequest(rp);
            is_sending = true;
            refresh_capt();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileInfo file = Microsoft.VisualBasic.FileIO.FileSystem.GetFileInfo(f.FileName);

                f_name.Text = file.Name;
                selected_file = file.FullName;
            }
        }

        private void lock_all()
        {
            switch_controls(false);
        }

        private void unlock_all()
        {
            switch_controls(true);
        }

        private void switch_controls(bool a)
        {
            submit_button.Enabled = a;
            name_f.Enabled = a;
            email_f.Enabled = a;
            sub_f.Enabled = a;
            com_f.Enabled = a;
            password_f.Enabled = a;
            cap_f.Enabled = a;
            button1.Enabled = a;
            button3.Enabled = a;
            button4.Enabled = a;
            f_name.Enabled = a;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            selected_file = "";
            f_name.Text = "";
        }

        private bool verify_errors()
        {
            if (cap_f.Text.Length < 1) { MessageBox.Show("You forgot to solve the captcha", "Error"); return false; }

            if (selected_file == "" && com_f.Text.Length == 0) { MessageBox.Show("Cannot submit a blank post", "Error"); return false; }

            if (password_f.Text == "") { MessageBox.Show("Password field is empty", "Error"); return false; }

            if (f_name.Text.Length == 0 && selected_file != "") { MessageBox.Show("File name cannot be null", "Error"); return false; }

            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            refresh_capt();
        }

        private void ReplyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.Hide(); }
        }

    }
}
