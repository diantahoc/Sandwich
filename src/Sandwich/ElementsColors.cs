using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Sandwich
{
    public static class ElementsColors
    {
        public static SolidColorBrush TripCodeColor;
        public static SolidColorBrush NameColor;
        public static SolidColorBrush DateTimeColor;
        public static SolidColorBrush PostTextColor;
        public static SolidColorBrush SubjectTextColor;
        public static SolidColorBrush PostIDColor;

        public static SolidColorBrush GreenTextColor;
        public static SolidColorBrush QuoteTextColor;

        public static SolidColorBrush ThreadBGColor;

        public static System.Windows.Media.Effects.DropShadowEffect FocusedPostEffect;

        static ElementsColors()
        {
            default_colors();
        }

        private static void default_colors() 
        {
            TripCodeColor = new SolidColorBrush(Color.FromRgb(102, 115, 191));
            TripCodeColor.Freeze();

            NameColor = new SolidColorBrush(Color.FromRgb(61, 61, 61));
            NameColor.Freeze();

            DateTimeColor = new SolidColorBrush(Color.FromRgb(111, 111, 111));
            DateTimeColor.Freeze();

            PostTextColor = new SolidColorBrush(Color.FromRgb(47, 47, 47));
            PostTextColor.Freeze();

            SubjectTextColor = new SolidColorBrush(Color.FromRgb(89, 97, 150));
            SubjectTextColor.Freeze();

            PostIDColor = new SolidColorBrush(Color.FromRgb(221, 0, 0));
            PostIDColor.Freeze();

            GreenTextColor = new SolidColorBrush(Color.FromRgb(62, 162, 144));
            GreenTextColor.Freeze();

            QuoteTextColor = new SolidColorBrush(Color.FromRgb(144, 155, 218));
            QuoteTextColor.Freeze();

            ThreadBGColor = new SolidColorBrush(Color.FromRgb(239, 239, 239));
            ThreadBGColor.Freeze();

            FocusedPostEffect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 3d, Color = Color.FromRgb(65, 65, 65) };
            FocusedPostEffect.Freeze();
        }

        private static void _420chan_classic() 
        {
            TripCodeColor = new SolidColorBrush(Color.FromRgb(34, 82, 94));
            TripCodeColor.Freeze();

            NameColor = new SolidColorBrush(Color.FromRgb(34, 82, 94));
            NameColor.Freeze();

            DateTimeColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            DateTimeColor.Freeze();

            PostTextColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            PostTextColor.Freeze();

            SubjectTextColor = new SolidColorBrush(Color.FromRgb(208, 32, 32));
            SubjectTextColor.Freeze();

            PostIDColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            PostIDColor.Freeze();

            GreenTextColor = new SolidColorBrush(Color.FromRgb(0, 94, 44));
            GreenTextColor.Freeze();

            QuoteTextColor = new SolidColorBrush(Color.FromRgb(153, 0, 0));
            QuoteTextColor.Freeze();

            ThreadBGColor = new SolidColorBrush(Color.FromRgb(255, 238, 221));
            ThreadBGColor.Freeze();

            FocusedPostEffect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 3d, Color = Color.FromRgb(0, 0, 0) };
            FocusedPostEffect.Freeze();
        }
    }
}
