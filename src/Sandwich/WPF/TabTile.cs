using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using MahApps.Metro.Controls;
namespace Sandwich.WPF
{
    public class TabTile : Tile
    {
        private DropShadowEffect a;

        public TabTile()
        {
            this.MouseEnter += (s, e) =>
            {
                this.Opacity = 1d;
            };
            this.MouseLeave += (s, e) =>
            {
                this.Opacity = 0.7;
            };

            a = new DropShadowEffect() { Color = Colors.White };
            a.Freeze();
        }

        public void Activate()
        {
            this.Effect = a;
        }

        public void DeActivate()
        {
            this.Effect = null;
        }

        public string Header { get; set; }
    }
}
