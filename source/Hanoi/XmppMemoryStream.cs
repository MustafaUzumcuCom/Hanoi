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

namespace Hanoi {
    /// <summary>
    ///   A stream implementation used as input buffer for 
    ///   incoming data in the <see cref = "Connection" /> class.
    /// </summary>
    internal sealed class XmppMemoryStream
        : Stream {
        private MemoryStream buffer;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:XmppMemoryStream" /> class.
        /// </summary>
        public XmppMemoryStream() {
            buffer = new MemoryStream();
        }

        /// <summary>
        ///   When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead {
            get { return buffer.CanRead; }
        }

        /// <summary>
        ///   When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite {
            get { return buffer.CanWrite; }
        }

        /// <summary>
        ///   When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek {
            get { return buffer.CanSeek; }
        }

        /// <summary>
        ///   When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value></value>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref = "T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref = "T:System.NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref = "T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Position {
            get { return buffer.Position; }
            set { buffer.Position = value; }
        }

        /// <summary>
        ///   When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value></value>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref = "T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref = "T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Length {
            get { return buffer.Length; }
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "T:XmppMemoryStream" /> reached EOF.
        /// </summary>
        /// <value><c>true</c> if EOF; otherwise, <c>false</c>.</value>
        public bool EOF {
            get { return (Position >= Length); }
        }

        /// <summary>
        ///   Releases the unmanaged resources used by the <see cref = "T:System.IO.Stream"></see> and optionally releases the managed resources.
        /// </summary>
        /// <param name = "disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            try
            {
                if (disposing)
                {
                    buffer.Dispose();
                    buffer = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        ///   Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>
        ///   The unsigned byte cast to an Int32, or -1 if at the end of the stream.
        /// </returns>
        /// <exception cref = "T:System.NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref = "T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override int ReadByte() {
            return buffer.ReadByte();
        }

        /// <summary>
        ///   When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name = "buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name = "offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name = "count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        ///   The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref = "T:System.ArgumentException">The sum of offset and count is larger than the buffer length. </exception>
        /// <exception cref = "T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref = "T:System.NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref = "T:System.ArgumentNullException">buffer is null. </exception>
        /// <exception cref = "T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref = "T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
        public override int Read(byte[] buffer, int offset, int count) {
            if (!CanRead)
            {
                throw new InvalidOperationException("Read operations are not allowed by this stream");
            }

            return this.buffer.Read(buffer, offset, count);
        }

        /// <summary>
        ///   Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name = "value">The byte to write to the stream.</param>
        /// <exception cref = "T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref = "T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref = "T:System.NotSupportedException">The stream does not support writing, or the stream is already closed. </exception>
        public override void WriteByte(byte value) {
            buffer.WriteByte(value);
        }

        /// <summary>
        ///   When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name = "buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name = "offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name = "count">The number of bytes to be written to the current stream.</param>
        /// <exception cref = "T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref = "T:System.NotSupportedException">The stream does not support writing. </exception>
        /// <exception cref = "T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref = "T:System.ArgumentNullException">buffer is null. </exception>
        /// <exception cref = "T:System.ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
        /// <exception cref = "T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
        public override void Write(byte[] buffer, int offset, int count) {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Write operations are not allowed by this stream");
            }

            this.buffer.Write(buffer, offset, count);
        }

        /// <summary>
        ///   Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
        public override void Close() {
            buffer.Close();
        }

        /// <summary>
        ///   Clears this instance.
        /// </summary>
        public void Clear() {
            SetLength(0);
            Position = 0;
        }

        /// <summary>
        ///   When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref = "T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Flush() {
            buffer.Flush();
        }

        /// <summary>
        ///   Sets the length.
        /// </summary>
        /// <param name = "length">The length.</param>
        public override void SetLength(long length) {
            buffer.SetLength(length);
        }

        /// <summary>
        ///   When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name = "offset">A byte offset relative to the origin parameter.</param>
        /// <param name = "origin">A value of type <see cref = "T:System.IO.SeekOrigin"></see> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        ///   The new position within the current stream.
        /// </returns>
        /// <exception cref = "T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref = "T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
        /// <exception cref = "T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Seek(long offset, SeekOrigin origin) {
            return buffer.Seek(offset, origin);
        }
        }
}