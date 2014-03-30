using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sandwich.WPF;

namespace Sandwich
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string choice = clean(textBox1.Text.ToLower());
        //    string[] unsupported = { "lgbt", "cm", "pol", "mlp", "hm", "soc", "r9k", "t", "f" };

        //    if (!textBox1.AutoCompleteCustomSource.Contains(choice)) { MessageBox.Show("Invalid board", "Error"); return; }

        //    if (Array.IndexOf(unsupported, choice) >= 0)
        //    {
        //        MessageBox.Show(String.Format("Unsupported board /{0}/", choice));
        //    }
        //    else
        //    {
        //        OpenBoard(choice);
        //    }
        //}

        //private void OpenBoard(string choice)
        //{
        //    bool found = false;
        //    foreach (Form openfrm in Application.OpenForms)
        //    {
        //        if (openfrm.GetType() == typeof(GenericBrowser))
        //        {
        //            GenericBrowser s = (GenericBrowser)(openfrm);
        //            if (s.Board == choice)
        //            {
        //                openfrm.Show();
        //                openfrm.Focus();
        //                found = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (!found)
        //    {
        //        //start new instance
        //        //GenericBrowser gc = new GenericBrowser(choice);
        //        //gc.Show();
        //        //Properties.Settings.Default.opened_boards.Add("/" + choice + "/");
        //        //Properties.Settings.Default.Save();
        //        //auto_hide();
        //        //textBox1.Text = "";

        //        BoardBrowserWPF gc = new BoardBrowserWPF(choice);
        //        gc.Show();
        //        Properties.Settings.Default.opened_boards.Add("/" + choice + "/");
        //        Properties.Settings.Default.Save();
        //        auto_hide();
        //        textBox1.Text = "";
        //    }
        //}

        //private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("Sandwich yum yum");
        //}

        //public string clean(string d)
        //{
        //    string[] allowedChars = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "4", "9", "3" };

        //    StringBuilder sb = new StringBuilder();

        //    foreach (char c in d)
        //    {
        //        if (Array.IndexOf(allowedChars, c.ToString()) >= 0) { sb.Append(c); }
        //    }

        //    string result = sb.ToString();
        //    return result;
        //}

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    init_cloudcontrol();
        //}

        //private void init_cloudcontrol()
        //{
        //    if (Properties.Settings.Default.opened_boards.Count > 1)
        //    {
        //        //526, 458
        //        this.Size = new Size(526, 458);
        //        //setup cloud control

        //        List<string> il = new List<string>();

        //        foreach (string s in Properties.Settings.Default.opened_boards) { il.Add(s); }

        //        IEnumerable<IWord> words = (il)
        //                                       .CountOccurences()
        //                                       .SortByOccurences()
        //                                       .Cast<IWord>();


        //        cloudControl1.WeightedWords = words;

        //        il.Clear(); il = null;
        //    }
        //    else
        //    {
        //        this.cloudControl1.Dispose();
        //        this.groupBox2.Hide();
        //        this.Size = new Size(526, 224);
        //    }
        //}

        //private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    Core.Shutdown(); Properties.Settings.Default.Save();
        //}

        //private void auto_hide()
        //{
        //    if (autoHideToolStripMenuItem.Checked) { this.Hide(); }
        //}

        //private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    Properties.Settings.Default.opened_boards.Clear();
        //    Properties.Settings.Default.Save();

        //    cloudControl1.WeightedWords = null;
        //}

        //private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    this.Show();
        //}

        //private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}

        //private void cloudControl1_Click(object sender, EventArgs e)
        //{
        //    Gma.CodeCloud.Controls.Geometry.LayoutItem itemUderMouse;
        //    Point mousePositionRelativeToControl =
        //       cloudControl1.PointToClient(new Point(MousePosition.X, MousePosition.Y));
        //    if (!cloudControl1.TryGetItemAtLocation(
        //             mousePositionRelativeToControl, out itemUderMouse))
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        OpenBoard(itemUderMouse.Word.Text.Replace("/", ""));
        //    }
        //}

        //private void cloudControl1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    Gma.CodeCloud.Controls.Geometry.LayoutItem itemUderMouse;
        //    Point mousePositionRelativeToControl =
        //       cloudControl1.PointToClient(new Point(MousePosition.X, MousePosition.Y));
        //    if (!cloudControl1.TryGetItemAtLocation(
        //             mousePositionRelativeToControl, out itemUderMouse))
        //    {
        //        board_stats.Text = "";
        //        return;
        //    }
        //    else
        //    {
        //        board_stats.Text = String.Format("{0} visited {1} times", itemUderMouse.Word.Text, itemUderMouse.Word.Occurrences.ToString());
        //    }
        //}

        //private void decodeCorneliaToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog a = new OpenFileDialog();
        //    a.Filter = "PNG |*.png";
        //    if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
        //    {
        //        SaveFileDialog sa = new SaveFileDialog();
        //        sa.Filter = "7z|*.7z";
        //        if (sa.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
        //        {
        //            Cornelia decoder = new Cornelia();
        //            System.IO.File.WriteAllBytes(sa.FileName, decoder.DecodeFile(System.IO.File.ReadAllBytes(a.FileName)));
        //        }
        //    }
        //}
    }
}
