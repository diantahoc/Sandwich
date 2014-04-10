using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Sandwich.WPF
{
    public class ClickableImage : Image 
    {
        private bool left_is_clicking = false;
        private bool wheel_is_clicking = false;

        public ClickableImage()
        {
            this.MouseLeave += (s, e) =>
            {
                this.left_is_clicking = false;
                this.wheel_is_clicking = false;
            };

            this.MouseDown += (s, e) => 
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left) { this.left_is_clicking = true; }
                if (e.ChangedButton == System.Windows.Input.MouseButton.Middle) { wheel_is_clicking = true; }
            };

            this.MouseUp += (s, e) => 
            {

                if (e.ChangedButton == System.Windows.Input.MouseButton.Middle) 
                {
                    if (wheel_is_clicking && this.MouseWheelClick != null) 
                    {
                        MouseWheelClick(this, null);
                        wheel_is_clicking = false;
                    }
                }

                if (e.ChangedButton == System.Windows.Input.MouseButton.Left) 
                {
                    if (left_is_clicking & this.Click != null)
                    {
                        Click(this, null);
                        left_is_clicking = false;
                    }
                }

            };
        }

        public event System.Windows.RoutedEventHandler Click;
        public event System.Windows.RoutedEventHandler MouseWheelClick;
    }
}
