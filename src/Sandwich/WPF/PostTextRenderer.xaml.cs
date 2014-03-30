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
using System.Windows.Shapes;
using AurelienRibon.Ui.SyntaxHighlightBox;
using Sandwich.DataTypes;

namespace Sandwich
{
    /// <summary>
    /// Interaction logic for PostTextRenderer.xaml
    /// </summary>
    public partial class PostTextRenderer : UserControl
    {
        private string qt = "->";

        public PostTextRenderer()
        {
            InitializeComponent();
        }

        public void Render(CommentToken[] data)
        {
            foreach (CommentToken token in data)
            {
                switch (token.Type)
                {
                    case CommentToken.TokenType.Text:
                        this.text.Inlines.Add(token.TokenData);
                        break;
                    case CommentToken.TokenType.GreenText:

                        TextBlock gt = new TextBlock()
                        {
                            Text = token.TokenData,
                            TextWrapping = TextWrapping.Wrap,
                            TextTrimming = TextTrimming.None,
                            Foreground = ElementsColors.GreenTextColor
                        };
                        this.text.Inlines.Add(gt);
                        break;
                    case CommentToken.TokenType.Quote:

                        Hyperlink q = new Hyperlink()
                        {
                            Foreground = ElementsColors.QuoteTextColor,
                            Cursor = Cursors.Hand,
                            Tag = Convert.ToInt32(token.TokenData)
                        };

                        q.Inlines.Add(qt + token.TokenData);

                        q.Click += (hpssender, e) =>
                        {
                            if (QuoteClicked != null) { QuoteClicked(Convert.ToInt32(((Hyperlink)hpssender).Tag)); }
                        };

                        this.text.Inlines.Add(q);
                        break;
                    case CommentToken.TokenType.SpoilerText:

                        TextBlock s = new TextBlock()
                        {
                            TextTrimming = TextTrimming.None,
                            TextWrapping = TextWrapping.Wrap,
                            Background = Brushes.Black,
                            Foreground = Brushes.Black,
                            Text = token.TokenData
                        };

                        s.MouseLeave += (sender, e) =>
                        {
                            ((TextBlock)(sender)).Foreground = Brushes.Black;
                        };
                        s.MouseEnter += (sender, e) =>
                        {
                            ((TextBlock)(sender)).Foreground = Brushes.White;
                        };

                        this.text.Inlines.Add(s);
                        break;
                    case CommentToken.TokenType.CodeBlock:

                        SyntaxHighlightBox shb = new SyntaxHighlightBox()
                        {
                            IsLineNumbersMarginVisible = true,
                            CurrentHighlighter = HighlighterManager.Instance.Highlighters["VHDL"]
                        };

                        shb.AppendText(token.TokenData);

                        this.text.Inlines.Add(shb);

                        break;
                    case CommentToken.TokenType.ColoredFText:

                        TextBlock rere = new TextBlock();
                        int i = token.TokenData.IndexOf('$');
                        string colors = token.TokenData.Split('$')[0].Remove(0, 1);
                        rere.Text = token.TokenData.Substring(i + 1);
                        Color fg = Color.FromRgb(Convert.ToByte(colors[0].ToString() + colors[1].ToString(), 16), Convert.ToByte(colors[2].ToString() + colors[3].ToString(), 16), Convert.ToByte(colors[4].ToString() + colors[5].ToString(), 16));
                        rere.Foreground = new SolidColorBrush(fg);

                        this.text.Inlines.Add(rere);
                        break;
                    case CommentToken.TokenType.Newline:
                        this.text.Inlines.Add("\n"); break;
                    case CommentToken.TokenType.ExifTable:
                        this.text.Inlines.Add(new WPF.ExifRenderer(token.TokenData) { HorizontalAlignment = System.Windows.HorizontalAlignment.Left }); break;
                    default:
                        continue;
                }
            }
        }

        public Common.QuoteClickEvent QuoteClicked;

    }


}
