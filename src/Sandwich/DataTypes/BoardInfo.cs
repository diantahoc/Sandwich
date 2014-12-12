using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich
{
    public class BoardInfo
    {
        public Chans.IChan Chan { get; set; }

        public string Letter { get; set; }

        public string Description { get; set; }

        public System.Windows.Media.Imaging.BitmapImage Icon { get; set; }

        public int PagesCount { get; set; }

        public int Bumplimit { get; set; }

        public int ImageLimit { get; set; }

        public string[] AllowedFiles { get; set; }

        public string[] ExtraFiles { get; set; }

        public string[] Files
        {
            get
            {
                return AllowedFiles.Union(this.ExtraFiles).ToArray();
            }
        }

        public int MaximumFileSize { get; set; }
        
        public bool WorkSafe { get; set; }
        
        public bool HasSpoilers { get; set; }

        public System.Windows.Media.Imaging.BitmapImage SpoilerImage { get; set; }


        public override string ToString()
        {
            return this.Letter;
        }

    }
}
