using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Sandwich
{
    public class Cornelia : IFileEncoderDecoder
    {
        //takes a byte[] array of the image file
        public byte[] DecodeFile(byte[] data) 
        {
            //temporary memory stream
            MemoryStream mem = new MemoryStream();

            //temporary image data
            MemoryStream imageData = new MemoryStream();
            
            imageData.Write(data, 0, data.Count());

            //initialize an image from the give data
            Image b = Image.FromStream(imageData, true);

            for (int x = 0; x < b.Width; x++) 
            {
                for (int y = 0; y < b.Height; y++) 
                {

                }
            }

            //save it to the temporary memory stream, in BMP format.
            b.Save(mem, System.Drawing.Imaging.ImageFormat.Png);

            b.Dispose();
            imageData.Dispose();

            byte[] result = mem.ToArray();

            char[] res = System.Text.Encoding.UTF8.GetChars(result);

            mem.Close(); mem.Dispose();

            //this should be the decoded file.
            return result;
        }

        public byte[] EncodeFile(byte[] data) 
        {
            return null;
        }

        string IFileEncoderDecoder.Name { get { return "Cornelia"; } }

    }
}
