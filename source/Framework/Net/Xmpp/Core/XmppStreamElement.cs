/*
    Copyright (c) 2007 - 2010, Carlos Guzm�n �lvarez

    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, 
          this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, 
          this list of conditions and the following disclaimer in the documentation and/or 
          other materials provided with the distribution.
        * Neither the name of the author nor the names of its contributors may be used to endorse or 
          promote products derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// An element of an XMPP XML message
    /// </summary>
    internal sealed class XmppStreamElement
    {
        #region � Fields �

        private string  name;
        private string  xmlNamespace;
        private string  node;

        #endregion

        #region � Properties �

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the XML namespace.
        /// </summary>
        /// <value>The XML namespace.</value>
        public string XmlNamespace
        {
            get { return this.xmlNamespace; }
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>The node.</value>
        public string Node
        {
            get { return this.node; } 
        }

        /// <summary>
        /// Gets a value indicating whether [opens XMPP stream].
        /// </summary>
        /// <value><c>true</c> if [opens XMPP stream]; otherwise, <c>false</c>.</value>
        public bool OpensXmppStream
        {
            get 
            {
                return (this.name == XmppCodes.XmppStreamName &&
                        this.node.StartsWith(XmppCodes.XmppStreamOpen)); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether [closes XMPP stream].
        /// </summary>
        /// <value><c>true</c> if [closes XMPP stream]; otherwise, <c>false</c>.</value>
        public bool ClosesXmppStream
        {
            get { return this.node.StartsWith(XmppCodes.XmppStreamClose); }
        }


        #endregion

        #region � Constructors �

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppStreamElement"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <param name="node">The node.</param>
        public XmppStreamElement(string name, string xmlNamespace, string node)
        {
            this.name			= name;
            this.xmlNamespace	= xmlNamespace;
            this.node			= node;
        }

        #endregion

        #region � Overriden Methods �

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.node;
        }

        #endregion
    }
}