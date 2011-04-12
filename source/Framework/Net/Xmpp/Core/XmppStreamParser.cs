/*
    Copyright (c) 2007 - 2010, Carlos Guzmán Álvarez

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
using System.IO;
using System.Text;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// A simple XMPP XML message parser
    /// </summary>
    internal sealed class XmppStreamParser
        : IDisposable
    {
        #region · Static Members ·

        private static string GetXmlNamespace(string tag)
        {
            return null;
        }

        private static string GetTagName(string tag)
        {
            StringBuilder   tagName = new StringBuilder();
            int             index   = 1;

            while (true)
            {
                char c = tag[index++];

                if (!Char.IsWhiteSpace(c) && c != '>' && c != '/')
                {
                    tagName.Append(c);
                }
                else
                {
                    break;
                }
            }

            return tagName.ToString();
        }

        private static bool IsStartTag(string tag)
        {
            return (tag.StartsWith("<", StringComparison.OrdinalIgnoreCase) &&
                    !tag.StartsWith("</", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsEndStreamTag(string tag)
        {
            return (tag.Equals(XmppCodes.XmppStreamClose));
        }

        private static bool IsEndTag(string tag)
        {
            return (tag.StartsWith("</", StringComparison.OrdinalIgnoreCase) ||
                    tag.EndsWith("/>", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsProcessingInstruction(string tag)
        {
            return (tag.StartsWith("<?", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsCharacterDataAndMarkup(string tag)
        {
            return (tag.StartsWith("<![", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsXmppStreamOpen(string tag)
        {
            return tag.StartsWith(XmppCodes.XmppStreamOpen, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region · Fields ·

        private XmppMemoryStream    stream;
        private BinaryReader        reader;
        private StringBuilder       node;
        private StringBuilder       currentTag;
        private long                depth;
        private string              nodeName;
        private string              nodeNamespace;
        private bool                isDisposed;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:XmppStreamParser"/> is EOF.
        /// </summary>
        /// <value><c>true</c> if EOF; otherwise, <c>false</c>.</value>
        public bool EOF
        {
            get { return this.stream.EOF; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppStreamParser"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public XmppStreamParser(XmppMemoryStream stream)
        {
            this.stream     = stream;
            this.reader     = new BinaryReader(this.stream, Encoding.UTF8);
            this.node       = new StringBuilder();
            this.currentTag = new StringBuilder();
        }

        #endregion

        #region · IDisposable Methods ·

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                    this.depth      = 0;
                    this.currentTag = null;                    
                    this.node       = null;
                    this.nodeName   = null;
                    if (this.reader != null)
                    {
                        this.reader.Close();
                        this.reader = null;
                    }
                    if (this.stream != null)
                    {
                        this.stream.Dispose();
                        this.stream = null;
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
            }

            this.isDisposed = true;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Reads the next node.
        /// </summary>
        /// <returns></returns>
        public XmppStreamElement ReadNextNode()
        {
            if (this.node.Length == 0)
            {
                this.depth          = -1;
                this.nodeName       = null;
                this.nodeNamespace  = null;
            }
            
            while (!this.EOF && this.depth != 0)
            {
                this.SkipWhiteSpace();

                int next = this.Peek();
                if (next == '<' || this.currentTag.Length > 0)
                {
                    if (!this.ReadTag())
                    {
                        break;
                    }

                    string tag = this.currentTag.ToString();

                    if (!XmppStreamParser.IsProcessingInstruction(tag))
                    {
                        if (this.node.Length == 0 && XmppStreamParser.IsXmppStreamOpen(tag))
                        {
                            this.nodeName   = XmppStreamParser.GetTagName(tag);
                            this.depth      = 0;
                        }
                        else if (this.node.Length == 0 && XmppStreamParser.IsEndStreamTag(tag))
                        {
                            this.nodeName   = XmppStreamParser.GetTagName(tag);
                            this.depth      = 0;
                        }
                        else
                        {
                            if (!XmppStreamParser.IsCharacterDataAndMarkup(tag))
                            {
                                if (XmppStreamParser.IsStartTag(tag))
                                {
                                    if (this.depth == -1)
                                    {
                                        this.nodeName = XmppStreamParser.GetTagName(tag);
                                        this.nodeNamespace = XmppStreamParser.GetXmlNamespace(tag);

                                        this.depth++;
                                    }

                                    depth++;
                                }

                                if (XmppStreamParser.IsEndTag(tag))
                                {
                                    this.depth--;
                                }
                            }
                        }

                        this.node.Append(tag);
                    }

                    this.currentTag.Length = 0;
                }
                else if (next != -1)
                {
                    if (!this.ReadText())   // Element Text
                    {
                        break;
                    }
                }
            }

            XmppStreamElement result = null;

            if (this.depth == 0)
            {
                result                  = new XmppStreamElement(this.nodeName, this.nodeNamespace, this.node.ToString());
                this.node.Length        = 0;
                this.currentTag.Length  = 0;
                this.depth              = -1;
                this.nodeName           = null;
                this.nodeNamespace      = null;
            }

            return result;
        }

        #endregion

        #region · Private Methods ·

        private bool ReadTag()
        {
            this.SkipWhiteSpace();

            int next = this.Peek();
            if (next != '<' && this.currentTag.Length == 0)
            {
                throw new IOException();
            }

            while (true)
            {
                next = this.Peek();
                if (next == -1)
                {
                    return false;
                }
                else
                {
                    this.currentTag.Append((char)this.Read());

                    if (next == '>')
                    {
                        return true;
                    }
                }
            }
        }

        private void SkipWhiteSpace()
        {
            while (true)
            {
                int next = this.Peek();
                if (next == -1)
                {
                    break;
                }
                else if (Char.IsWhiteSpace((char)next))
                {
                    this.Read();
                }
                else
                {
                    break;
                }
            }
        }

        private bool ReadText()
        {
            while (true) 
            {
                if (this.Peek() == -1)
                {
                    return false;
                }

                if (this.Peek() != '<')
                {
                    this.node.Append(this.Read());
                }
                else
                {
                    break;
                }
            }

            return true;
        }

        private int Peek()
        {			
            return this.reader.PeekChar();
        }

        private char Read()
        {
            return this.reader.ReadChar();
        }

        #endregion
    }
}
