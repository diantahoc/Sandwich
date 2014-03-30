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
    /// Interaction logic for RegexHelp.xaml
    /// </summary>
    public partial class RegexHelp : Window
    {
        public RegexHelp()
        {
            InitializeComponent();

            this.textBlock1.Text = "    .   Matches all characters.\n" +
                                    "   +   Matches the previous expression one or more times.\n" +
                                    "   *   Matches the previous expression zero or more times.\n" +
                                    "   \\  Escapes special characters.\n" +

                                    "   \\Q...\\E  Matches the characters between \\Q and \\E literally, suppressing the meaning of special characters.\n" +
                                    "   [a-c]   Matches a, b and c.\n" +
                                    "   \\d Matches all digits.\n" +
                                    "   \\w Matches all word characters.\n" +
                                    "   \\s Matches all whitespace characters.\n" +
                                    "   ^	Matches the start of a line.\n" +
                                    "   $	Matches the end of a line.\n";
            
            this.textBlock1.TextWrapping = TextWrapping.Wrap;
        }

    }
}
