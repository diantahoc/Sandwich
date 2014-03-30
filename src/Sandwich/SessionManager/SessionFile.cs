using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich
{
    public class SessionFile
    {
        public string FileName { get; set; }
        public string Ext { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int ThumbH { get; set; }
        public int ThumbW { get; set; }
        public int Owner { get; set; }
        public string ThumbFileName { get; set; }
        public string Board { get; set; }
        public string Hash { get; set; }
        public int Size { get; set; }
        public bool IsSpoiler { get; set; }
    }
}
