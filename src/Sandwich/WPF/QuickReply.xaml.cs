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
    /// Interaction logic for QuickReply.xaml
    /// </summary>
    public partial class QuickReply : UserControl, IDisposable
    {
        public string Board { get; private set; }

        public QuickReply(string board)
        {
            InitializeComponent();
            this.Board = board;
        }

        /// <summary>
        /// Dummy initializer for WPF designer
        /// </summary>
        public QuickReply()
        {
            InitializeComponent();
            this.Board = null;
        }

        public QuickReplyTab AddThreadReply(int tid)
        {
            QuickReplyTab t = get_tab(tid);

            if (t == null)
            {
                QuickReplyTab a = new QuickReplyTab(Common.PostingMode.Reply, this.Board, tid);
                this.tabs.AddTab(new ChromeTabs.ChromeTabItem() { Header = string.Format("Thread {0}", tid), Content = a }, true);
                return a;
            }
            else
            {
                //focus it
                this.tabs.SelectedItem = t;
                return t;
            }
        }

        private int get_tab_index(int tid)
        {
            for (int index = 0; index < this.tabs.Items.Count; index++)
            {
                object t = this.tabs.Items[index];
                if (t.GetType() == typeof(ChromeTabs.ChromeTabItem))
                {
                    ChromeTabs.ChromeTabItem cti = (ChromeTabs.ChromeTabItem)t;
                    QuickReplyTab a = (QuickReplyTab)cti.Content;
                    if (a.ID == tid)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        public QuickReplyTab get_tab(int tid)
        {
            int i = get_tab_index(tid);
            if (i < 0)
            {
                return null;

            }
            else { return (QuickReplyTab)((ChromeTabs.ChromeTabItem)this.tabs.Items[i]).Content; }
        }

        public void AddNewThread()
        {
            int index = get_tab_index(0);

            if (index < 0)
            {
                QuickReplyTab a = new QuickReplyTab(Common.PostingMode.NewThread, this.Board, 0);
                a.OpenThread += this.OpenThread;
                this.tabs.AddTab(new ChromeTabs.ChromeTabItem() { Header = "New thread", Content = a }, true);
            }
            else
            {
                //focus it
                this.tabs.SelectedIndex = index;
            }
        }

        public void FocusThreadID(int id)
        {
            int t = get_tab_index(id);

            if (t >= 0)
            {
                this.tabs.SelectedIndex = t;
            }
        }

        public void RemoveTab(int tid)
        {
            QuickReplyTab t = get_tab(tid);
            if (t == null)
            {
                return;
            }
            else
            {
                this.tabs.Items.Remove(t);
            }
        }

        public void Dispose() { }

        public QuickReplyTab.OpenThreadEvent OpenThread;
    }
}
