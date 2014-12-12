using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich.Chans
{
    public interface IChanArchiveDataProvider
    {
        void UpdateData();
        bool BoardHasArchive(string board);
        bool BoardHasFileArchive(string board);

        ThreadContainer GetThread(string board, int id);
    }
}
