using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Sandwich.WPF
{
    public class ClickableLabel : Label
    {
        private bool _is_clicking = false;

        public ClickableLabel()
        {
            this.MouseLeftButtonDown += (s, e) =>
            {
                this._is_clicking = true;
            };

            this.MouseLeave += (s, e) =>
            {
                this._is_clicking = false;
            };

            this.MouseLeftButtonUp += (s, e) =>
            {
                if (_is_clicking & this.Click != null)
                {
                    Click(this, null);
                    _is_clicking = false;
                }
            };
        }

        public event System.Windows.RoutedEventHandler Click;
    }
}
