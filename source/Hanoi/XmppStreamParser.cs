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

namespace Hanoi
{
    /// <summary>
    ///   A simple XMPP XML message parser
    /// </summary>
    internal sealed class XmppStreamParser : IDisposable
    {
        private StringBuilder currentTag;
        private long depth;
        private bool isDisposed;
        private StringBuilder node;
        private string nodeName;
        private string nodeNamespace;
        private BinaryReader reader;
        private XmppMemoryStream stream;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:XmppStreamParser" /> class.
        /// </summary>
        /// <param name = "stream">The stream.</param>
        public XmppStreamParser(XmppMemoryStream stream)
        {
            this.stream = stream;
            reader = new BinaryReader(this.stream, Encoding.UTF8);
            node = new StringBuilder();
            currentTag = new StringBuilder();
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "T:XmppStreamParser" /> is EOF.
        /// </summary>
        /// <value><c>true</c> if EOF; otherwise, <c>false</c>.</value>
        public bool EOF
        {
            get { return stream.EOF; }
        }

        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion

        private static string GetXmlNamespace(string tag)
        {
            return null;
        }

        private static string GetTagName(string tag)
        {
            var tagName = new StringBuilder();
            int index = 1;

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
            return (tag.Equals(StreamCodes.XmppStreamClose));
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
            return tag.StartsWith(StreamCodes.XmppStreamOpen, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///   Disposes the specified disposing.
        /// </summary>
        /// <param name = "disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                    depth = 0;
                    currentTag = null;
                    node = null;
                    nodeName = null;
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                    if (stream != null)
                    {
                        stream.Dispose();
                        stream = null;
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
            }

            isDisposed = true;
        }

        /// <summary>
        ///   Reads the next node.
        /// </summary>
        /// <returns></returns>
        public XmppStreamElement ReadNextNode()
        {
            if (node.Length == 0)
            {
                depth = -1;
                nodeName = null;
                nodeNamespace = null;
            }

            while (!EOF && depth != 0)
            {
                SkipWhiteSpace();

                int next = Peek();
                if (next == '<' || currentTag.Length > 0)
                {
                    if (!ReadTag())
                    {
                        break;
                    }

                    string tag = currentTag.ToString();

                    if (!XmppStreamParser.IsProcessingInstruction(tag))
                    {
                        if (node.Length == 0 && XmppStreamParser.IsXmppStreamOpen(tag))
                        {
                            nodeName = XmppStreamParser.GetTagName(tag);
                            depth = 0;
                        }
                        else if (node.Length == 0 && XmppStreamParser.IsEndStreamTag(tag))
                        {
                            nodeName = XmppStreamParser.GetTagName(tag);
                            depth = 0;
                        }
                        else
                        {
                            if (!XmppStreamParser.IsCharacterDataAndMarkup(tag))
                            {
                                if (XmppStreamParser.IsStartTag(tag))
                                {
                                    if (depth == -1)
                                    {
                                        nodeName = XmppStreamParser.GetTagName(tag);
                                        nodeNamespace = XmppStreamParser.GetXmlNamespace(tag);

                                        depth++;
                                    }

                                    depth++;
                                }

                                if (XmppStreamParser.IsEndTag(tag))
                                {
                                    depth--;
                                }
                            }
                        }

                        node.Append(tag);
                    }

                    currentTag.Length = 0;
                }
                else if (next != -1)
                {
                    if (!ReadText()) // Element Text
                    {
                        break;
                    }
                }
            }

            XmppStreamElement result = null;

            if (depth == 0)
            {
                result = new XmppStreamElement(nodeName, nodeNamespace, node.ToString());
                node.Length = 0;
                currentTag.Length = 0;
                depth = -1;
                nodeName = null;
                nodeNamespace = null;
            }

            return result;
        }

        private bool ReadTag()
        {
            SkipWhiteSpace();

            int next = Peek();
            if (next != '<' && currentTag.Length == 0)
            {
                throw new IOException();
            }

            while (true)
            {
                next = Peek();
                if (next == -1)
                {
                    return false;
                }
                else
                {
                    currentTag.Append(Read());

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
                int next = Peek();
                if (next == -1)
                {
                    break;
                }
                else if (Char.IsWhiteSpace((char)next))
                {
                    Read();
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
                if (Peek() == -1)
                {
                    return false;
                }

                if (Peek() != '<')
                {
                    node.Append(Read());
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
            return reader.PeekChar();
        }

        private char Read()
        {
            return reader.ReadChar();
        }
    }
}