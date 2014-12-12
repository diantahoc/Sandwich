using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Sandwich
{
    public class Cornelia : IFileEncoderDecoder
    {

        private static string[][] archives = new string[][] 
        {
            new string[] { "Rar!", "rar" },
            new string[] { "7z", "7z" },
            new string[] {"PK", "zip"}
        };

        /// <summary>
        /// Return {file_extension:data}. file_extension is null when no file is detected
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public KeyValuePair<string, byte[]> DecodeFile(byte[] data)
        {
            byte[] decoded = null;

            using (Image i = Image.FromStream(new MemoryStream(data)))
            {
                decoded = SaveAs24Bit(i);
            }

            if (decoded != null)
            {
                for (int i = 0; i < archives.Length; i++)
                {
                    KeyValuePair<bool, int> a = detect_seqence(decoded, archives[i][0]);

                    if (a.Key)
                    {
                        return new KeyValuePair<string, byte[]>(archives[i][1], decoded.Skip(a.Value).ToArray());
                    }
                }
            }

            return new KeyValuePair<string, byte[]>(null, null);
        }

        public byte[] EncodeFile(byte[] data)
        {
            return null;
        }

        string IFileEncoderDecoder.Name { get { return "Cornelia"; } }

        public static byte[] SaveAs24Bit(Image i)
        {
            using (MemoryStream bitmap_stream = new MemoryStream())
            {
                ImageCodecInfo bmpCodec = Helpers.ImageManipulation.GetEncoderInfo("image/bmp");

                using (Bitmap blankImage = new Bitmap(i.Width, i.Height, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(blankImage))
                    {
                        g.DrawImageUnscaledAndClipped(i, new Rectangle(Point.Empty, i.Size));
                    }
                    blankImage.Save(bitmap_stream, bmpCodec, null);
                }
                return bitmap_stream.ToArray();
            }
        }

        private static KeyValuePair<bool, int> detect_seqence(byte[] file, string sequence)
        {
            byte[] s = Encoding.UTF8.GetBytes(sequence);

            for (int i = 0; i < file.Length; i ++)
            {
                bool ok = true;
                for (int _i = 0; _i < s.Length; _i++)
                {
                    if (file[i + _i] != s[_i])
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                {
                    return new KeyValuePair<bool, int>(true, i);
                }
            }

            return new KeyValuePair<bool, int>(false, 0);
        }

    }
}
