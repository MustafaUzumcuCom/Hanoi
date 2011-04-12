using System;
using System.Collections.Generic;

namespace BabelIm
{
    /// <summary>
    /// http://www.fishbowlclient.com/
    /// </summary>
    public sealed class SingleInstanceEventArgs : EventArgs
    {
        #region · Properties ·

        public IList<string> Args
        {
            get;
            internal set;
        }

        #endregion

        #region · Constructors ·

        public SingleInstanceEventArgs()
        {
        }

        #endregion
    }
}
