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
    /// Interaction logic for ExifRenderer.xaml
    /// </summary>
    public partial class ExifRenderer : UserControl
    {
        public ExifRenderer(string text)
        {
            InitializeComponent();
            this.t.Text = text;
        }

        DateTime a;

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            a = DateTime.Now;
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((DateTime.Now - a).TotalMilliseconds <= Common.click_duration) 
            {
                if (sc.Visibility == System.Windows.Visibility.Collapsed)
                {
                    sc.Visibility = System.Windows.Visibility.Visible;
                }
                else 
                {
                    sc.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
