using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich
{
    interface IFileEncoderDecoder
    {
         byte[] DecodeFile(byte[] data);

         byte[] EncodeFile(byte[] data);

         string Name { get; }
    }
}
