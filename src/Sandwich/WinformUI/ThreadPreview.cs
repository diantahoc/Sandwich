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
    public partial class ThreadPreview : Form
    {
        public ThreadPreview(Image bl , CatalogItem ci)
        {
            InitializeComponent();
            pictureBox1.Image = bl;
            linkLabel1.Text = String.Format("by {0}{1}", ci.trip, ci.name);
            
            this.htmlPanel1.Text = ci.comment;

            if (!String.IsNullOrEmpty(ci.subject))
            {
                this.Text = ci.subject + " - Thread No. " + ci.PostNumber.ToString();
            }
            else 
            {
                this.Text = "Thread No. " + ci.PostNumber.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
