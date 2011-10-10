using System;
using System.Collections.Generic;

namespace BabelIm {
    /// <summary>
    ///   http://www.fishbowlclient.com/
    /// </summary>
    public sealed class SingleInstanceEventArgs : EventArgs {
        public IList<string> Args { get; internal set; }
    }
}