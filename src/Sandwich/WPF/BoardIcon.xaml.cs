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
using System.Windows.Media.Effects;

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for BoardIcon.xaml
    /// </summary>
    public partial class BoardIcon : UserControl
    {

        private string _l;
   
        public BoardIcon(string board_letter)
        {
            InitializeComponent();
            _l = board_letter;

            this.Desc.Content = BoardInfo.GetBoardTitle(board_letter);
            this.Icon.Source = BoardInfo.GetBoardIcon(board_letter);
            this.Icon.Stretch = Stretch.Fill;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IconClicked != null) 
            {
                IconClicked(_l);
            }
        }

        public event Common.BoardIconClicked IconClicked;

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Desc.FontSize = 15d;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Desc.FontSize = 11d;
        }

        DateTime a;

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) { a = DateTime.Now; }
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IconClicked != null)
            {
                if (e.ButtonState == MouseButtonState.Released)
                {
                    TimeSpan b = DateTime.Now - a;
                    if (b.TotalMilliseconds < Common.click_duration) 
                    {
                        IconClicked(_l);
                        a = DateTime.Now;
                    }
                }
            }
        }

    }
}
