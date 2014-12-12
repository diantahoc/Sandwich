using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich.Chans
{
    public interface IPostSenderResponse
    {
        Core.PostSendingStatus ResponseStatus { get; }

        string ResponseHTML { get; }

        /// <summary>
        /// Thread ID is the Key, Post# is the value
        /// </summary>
        KeyValuePair<int, int> AdditionalData { get; }
    }
}
