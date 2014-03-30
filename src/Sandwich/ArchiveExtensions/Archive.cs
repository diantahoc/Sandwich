using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich.ArchiveExtensions
{
    public class Archive
    {
        public string Name { get; set; }

        public string Domain { get; set; }

        public string ThreadUrl { get; set; }

        public string FileURL { get; set; }

        public string[] SupportedBoards { get; set; }

        public string[] SupportedFullImages { get; set; }

        public ArchiveType Type { get; set; }

        public enum ArchiveType
        {
            FoolFuuka,
            Fuuka
        }

        public bool IsBoardSupported(string board)
        {
            if (this.SupportedBoards != null)
            {
                return (Array.IndexOf(this.SupportedBoards, board) >= 0);
            }
            else { return false; }
        }

        public bool IsFileSupported(string board)
        {
            if (this.SupportedFullImages != null)
            {
                return (Array.IndexOf(this.SupportedFullImages, board) >= 0);
            }
            else { return false; }
        }

    }
}
