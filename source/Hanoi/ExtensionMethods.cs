/*
    Copyright (c) 2008 - 2010, Carlos Guzmán Álvarez

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
using System.Security.Cryptography;
using System.Text;

namespace Hanoi
{
    /// <summary>
    ///   Hash extension methods
    /// </summary>
    public static class HashExtensionMethods
    {
        /// <summary>
        ///   Converts a given byte array to a base-64 string
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        ///   Computes the SHA1 hash of a given array of strings
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        public static byte[] ComputeSHA1Hash(this string value)
        {
            using (SHA1 hashAlgorithm = SHA1.Create())
            {
                return hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        /// <summary>
        ///   Computes the SHA1 hash of a given array of strings
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        public static byte[] ComputeSHA1Hash(this StringBuilder value)
        {
            return value.ToString().ComputeSHA1Hash();
        }

        /// <summary>
        ///   Computes the MD5 hash of a given byte array
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        public static byte[] ComputeMD5Hash(this byte[] buffer)
        {
            using (MD5 md5 = MD5.Create())
            {
                md5.TransformFinalBlock(buffer, 0, buffer.Length);

                return md5.Hash;
            }
        }

        /// <summary>
        ///   Computes the SHA1 hash of a given byte array
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        public static byte[] ComputeSHA1Hash(this byte[] buffer)
        {
            using (SHA1 hashAlgorithm = SHA1.Create())
            {
                hashAlgorithm.TransformFinalBlock(buffer, 0, buffer.Length);

                return hashAlgorithm.Hash;
            }
        }

        /// <summary>
        ///   Computes the MD5 hash of a given array of strings
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        public static byte[] ComputeMD5Hash(this string[] values)
        {
            using (MD5 hashAlgorithm = MD5.Create())
            {
                foreach (string value in values)
                {
                    if (value != null)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(value);
                        var output = new byte[buffer.Length];
                        int count = hashAlgorithm.TransformBlock(buffer, 0, buffer.Length, output, 0);
                    }
                }

                hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);

                return hashAlgorithm.Hash;
            }
        }

        /// <summary>
        ///   Computes the SHA1 hash of a given array of strings
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        private static byte[] ComputeSHA1Hash(this string[] values)
        {
            using (SHA1 hashAlgorithm = SHA1.Create())
            {
                foreach (string value in values)
                {
                    if (value != null)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(value);
                        var output = new byte[buffer.Length];
                        int count = hashAlgorithm.TransformBlock(buffer, 0, buffer.Length, output, 0);
                    }
                }

                hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);

                return hashAlgorithm.Hash;
            }
        }

        /// <summary>
        ///   Convert a byte array to an hex string
        /// </summary>
        /// <param name = "buffer"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] buffer)
        {
            var hex = new StringBuilder();

            for (int i = 0; i < buffer.Length; i++)
            {
                hex.Append(buffer[i].ToString("x2"));
            }

            return hex.ToString();
        }
    }
}