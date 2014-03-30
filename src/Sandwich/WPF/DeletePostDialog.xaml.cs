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
    /// Interaction logic for DeletePostDialog.xaml
    /// </summary>
    public partial class DeletePostDialog : UserControl
    {
        private string board;
        private int tid;
        private int postid;

        public DeletePostDialog(string _board, int threadID, int postID)
        {
            InitializeComponent();
            this.board = _board;
            tid = threadID;
            postid = postID;
        }

        private void DeletePost()
        {
            this.pro.Visibility = System.Windows.Visibility.Visible;
            this.password.IsEnabled = false;
            this.fileonly.IsEnabled = false;
            this.status.Visibility = System.Windows.Visibility.Hidden;

            string password = this.password.Text;
            bool file_only = this.fileonly.IsChecked == true;

            System.Threading.Tasks.Task.Factory.StartNew((Action)delegate
            {
                try
                {
                    Core.DeleteStatus del_status = Core.DeletePost(this.board, this.tid, this.postid, password, file_only);


                    if (del_status == Core.DeleteStatus.Success)
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            this.status.Content = file_only ? "Delete file successfully" : "Delete post sucessfully";
                            this.status.Foreground = Brushes.Green;
                            this.status.Visibility = System.Windows.Visibility.Visible;
                        });
                    }
                    else
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            this.status.Content = string.Format("Cannot perform action for the following reason: {0}", del_status.ToString());
                            this.status.Foreground = Brushes.Red;
                            this.status.Visibility = System.Windows.Visibility.Visible;
                        });
                    }

                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        this.status.Content = string.Format("An error has been occured {0}", ex.Message);
                        this.status.Foreground = Brushes.Red;
                        this.status.Visibility = System.Windows.Visibility.Visible;
                    });
                }

                this.Dispatcher.Invoke((Action)delegate
                {
                    this.pro.Visibility = System.Windows.Visibility.Collapsed;
                    this.password.IsEnabled = true;
                    this.fileonly.IsEnabled = true;
                });

            });

        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                DeletePost();
            }
        }

    }
}
