using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandwich.WPF;

namespace Sandwich
{
    public interface TabElement
    {
        Common.ElementType Type { get; }

        int ID { get; }

        string Board { get; }

        BoardBrowserWPF TParent { get; }

        string Title { get; }

        void CleanUp();
    }
}
