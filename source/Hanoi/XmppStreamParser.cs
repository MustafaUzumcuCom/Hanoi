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
        private StringBuilder _currentTag = new StringBuilder();
        private long _depth;
        private bool _isDisposed;
        private StringBuilder _node = new StringBuilder();
        private string _nodeName;
        private string _nodeNamespace;
        private BinaryReader _reader;
        private XmppMemoryStream _stream;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppStreamParser" /> class.
        /// </summary>
        /// <param name = "stream">The stream.</param>
        public XmppStreamParser(XmppMemoryStream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(_stream, Encoding.UTF8);
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "XmppStreamParser" /> is EOF.
        /// </summary>
        /// <value><c>true</c> if EOF; otherwise, <c>false</c>.</value>
// ReSharper disable InconsistentNaming
        public bool EOF
// ReSharper restore InconsistentNaming
        {
            get { return _stream.EOF; }
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
            return (tag.Equals(StreamElements.XmppStreamClose));
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
            return tag.StartsWith(StreamElements.XmppStreamOpen, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///   Disposes the specified disposing.
        /// </summary>
        /// <param name = "disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                    _depth = 0;
                    _currentTag = null;
                    _node = null;
                    _nodeName = null;
                    if (_reader != null)
                    {
                        _reader.Close();
                        _reader = null;
                    }
                    if (_stream != null)
                    {
                        _stream.Dispose();
                        _stream = null;
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
            }

            _isDisposed = true;
        }

        /// <summary>
        ///   Reads the next node.
        /// </summary>
        /// <returns></returns>
        public XmppStreamElement ReadNextNode()
        {
            if (_node.Length == 0)
            {
                _depth = -1;
                _nodeName = null;
                _nodeNamespace = null;
            }

            while (!EOF && _depth != 0)
            {
                SkipWhiteSpace();

                int next = Peek();
                if (next == '<' || _currentTag.Length > 0)
                {
                    if (!ReadTag())
                    {
                        break;
                    }

                    string tag = _currentTag.ToString();

                    if (!IsProcessingInstruction(tag))
                    {
                        if (_node.Length == 0 && IsXmppStreamOpen(tag))
                        {
                            _nodeName = GetTagName(tag);
                            _depth = 0;
                        }
                        else if (_node.Length == 0 && IsEndStreamTag(tag))
                        {
                            _nodeName = GetTagName(tag);
                            _depth = 0;
                        }
                        else
                        {
                            if (!IsCharacterDataAndMarkup(tag))
                            {
                                if (IsStartTag(tag))
                                {
                                    if (_depth == -1)
                                    {
                                        _nodeName = GetTagName(tag);
                                        _nodeNamespace = GetXmlNamespace(tag);

                                        _depth++;
                                    }

                                    _depth++;
                                }

                                if (IsEndTag(tag))
                                {
                                    _depth--;
                                }
                            }
                        }

                        _node.Append(tag);
                    }

                    _currentTag.Length = 0;
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

            if (_depth == 0)
            {
                result = new XmppStreamElement(_nodeName, _nodeNamespace, _node.ToString());
                _node.Length = 0;
                _currentTag.Length = 0;
                _depth = -1;
                _nodeName = null;
                _nodeNamespace = null;
            }

            return result;
        }

        private bool ReadTag()
        {
            SkipWhiteSpace();

            var next = Peek();
            if (next != '<' && _currentTag.Length == 0)
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
                
                _currentTag.Append(Read());

                if (next == '>')
                {
                    return true;
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
                if (Char.IsWhiteSpace((char)next))
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
                    _node.Append(Read());
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
            return _reader.PeekChar();
        }

        private char Read()
        {
            return _reader.ReadChar();
        }
    }
}