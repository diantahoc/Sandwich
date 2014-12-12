using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich
{
    interface IFileEncoderDecoder
    {
        KeyValuePair<string, byte[]> DecodeFile(byte[] data);

        byte[] EncodeFile(byte[] data);

        string Name { get; }
    }
}
