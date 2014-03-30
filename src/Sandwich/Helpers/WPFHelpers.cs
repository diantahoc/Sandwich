//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Runtime.InteropServices;

//namespace Sandwich
//{
//    static class WPFHelpers
//    {
//        public static BitmapSource ConvertToBS(System.Drawing.Bitmap source)
//        {
//            IntPtr ip = source.GetHbitmap();
//            BitmapSource bs = null;
//            try
//            {
//                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
//                   IntPtr.Zero, Int32Rect.Empty,
//                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
//            }
//            finally
//            {
//                DeleteObject(ip);
//            }

//            return bs;
//        }

//        public static BitmapSource ConvertToBS(System.Drawing.Image source)
//        {
//            return ConvertToBS((System.Drawing.Bitmap)source);
//        }

//        [DllImport("gdi32")]
//        static extern int DeleteObject(IntPtr o);

//        public static System.Windows.Media.Color DColorToMColor(System.Drawing.Color c)
//        {
//            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
//        }


//        public static BitmapImage BSFromStream(ref System.IO.MemoryStream stream)
//        {
//            BitmapImage bbb = new BitmapImage();
//            bbb.BeginInit();
//            bbb.CacheOption = BitmapCacheOption.Default;
//            bbb.StreamSource = stream;
//            bbb.EndInit();
//            return bbb;
//        }
//    }
//}
