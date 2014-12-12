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
using FirstFloor.ModernUI.Windows.Controls;

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for MuiWindow.xaml
    /// </summary>
    public partial class MuiWindow : ModernWindow
    {
        public MuiWindow()
        {
            InitializeComponent();

            //this.ContentSource = new Uri("pack://application:,,,/WPF/MainWindow.xaml", UriKind.Absolute);
        }
    }
}
