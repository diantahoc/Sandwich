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
using MahApps.Metro.Controls;

namespace Sandwich.WPF
{
    /// <summary>
    /// Interaction logic for TileTab.xaml
    /// </summary>
    public partial class TileTab : UserControl
    {
        private List<TabElement> elements = new List<TabElement>();

        private int selected_index = 0;

        public TileTab()
        {
            InitializeComponent();
        }

        public void AddElement(TabElement element)
        {
            if (this.elements.Contains(element))
            {
                return;
            }
            else
            {
                this.elements.Add(element);
            }
        }

        private void add_tile(TabElement element) 
        {
            TabTile t = new TabTile() { Header = element.Title };
            this.tiles.Children.Add(t);
        }

        private void focus(int index)
        {
            this.con.Children.Clear();
            this.con.Children.Add((UIElement)this.elements[index]);
        }

        private void activate_tile(int index)
        {
            TabTile e = (TabTile)this.tiles.Children[index];
            e.Activate();
        }

    }
}
