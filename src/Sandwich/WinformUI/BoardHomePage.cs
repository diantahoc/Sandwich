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
    public partial class BoardHomePage : Form
    {
        string board;

        public BoardHomePage(string b)
        {
            InitializeComponent();
            board = b;

            this.elementHost1.Child = new Sandwich.WPF.BoardHomePageWPF(b, null);
        }

    }
}
