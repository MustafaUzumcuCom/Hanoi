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
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Hanoi.Xmpp.InstantMessaging {
    /// <summary>
    ///   <see cref = "Session" /> configuration
    /// </summary>
    [Serializable]
    [XmlTypeAttribute(Namespace = "")]
    [XmlRootAttribute("storage", Namespace = "", IsNullable = false)]
    public sealed class AvatarStorage {
        private static XmlSerializer Serializer = new XmlSerializer(typeof (AvatarStorage));
        private static readonly string AvatarsDirectory = "Avatars";
        private static readonly string AvatarsFile = "Avatars.xml";

        private List<Avatar> avatars;
        private object syncObject;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppSessionConfiguration" /> class.
        /// </summary>
        public AvatarStorage() {
            avatars = new List<Avatar>();
            syncObject = new object();
        }

        [XmlArray("avatars")]
        [XmlArrayItem("avatar", typeof (Avatar))]
        public List<Avatar> Avatars {
            get { return avatars; }
        }

        private static bool ExistsAvatar(string file) {
            string filename = AvatarsDirectory + Path.DirectorySeparatorChar + file;

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.GetDirectoryNames(AvatarsDirectory).Length == 0)
                {
                    storage.CreateDirectory(AvatarsDirectory);
                }

                return (storage.GetFileNames(filename).Length != 0);
            }
        }

        private static byte[] ReadFile(string file) {
            string filename = AvatarsDirectory + Path.DirectorySeparatorChar + file;

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.GetDirectoryNames(AvatarsDirectory).Length == 0)
                {
                    storage.CreateDirectory(AvatarsDirectory);
                }

                using (var stream =
                    new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, storage))
                {
                    var buffer = new byte[stream.Length];

                    stream.Read(buffer, 0, buffer.Length);

                    return buffer;
                }
            }
        }

        /// <summary>
        ///   Gets the avatar hash.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        /// <returns></returns>
        public string GetAvatarHash(string contactId) {
            Avatar avatar = avatars.Where(a => a.Contact == contactId).SingleOrDefault();

            if (avatar != null)
            {
                return avatar.Hash;
            }

            return null;
        }

        /// <summary>
        ///   Reads the avatar.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        /// <returns></returns>
        public Stream ReadAvatar(string contactId) {
            lock (syncObject)
            {
                Avatar avatar = avatars.Where(a => a.Contact == contactId).SingleOrDefault();

                if (avatar != null)
                {
                    if (ExistsAvatar(avatar.Hash + ".avatar"))
                    {
                        return new MemoryStream(ReadFile(avatar.Hash + ".avatar"));
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///   Removes the avatar of the given contact id.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        public void RemoveAvatar(string contactId) {
            lock (syncObject)
            {
                try
                {
                    Avatars.Remove(avatars.Where(a => a.Contact == contactId).SingleOrDefault());
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///   Loads avatars
        /// </summary>
        public void Load() {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.FileExists(AvatarsFile))
                {
                    using (var stream =
                        new IsolatedStorageFileStream(AvatarsFile, FileMode.OpenOrCreate, storage))
                    {
                        if (stream.Length > 0)
                        {
                            var avatarStorage = (AvatarStorage) Serializer.Deserialize(stream);

                            Avatars.AddRange(avatarStorage.Avatars);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Saves the avatar.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        /// <param name = "hash">The hash.</param>
        /// <param name = "avatar">The avatar image.</param>
        public void SaveAvatar(string contactId, string hash, MemoryStream avatarStream) {
            lock (syncObject)
            {
                // The avatar files should be saved only if it's not in use by another user ( several users can share the same avatar )
                var q = from userAvatar in Avatars
                        where userAvatar.Hash == hash
                        select userAvatar;

                try
                {
                    if (q.Count() == 0 && avatarStream.Length > 0)
                    {
                        String avatarFile = String.Format("{0}{1}{2}{3}", AvatarsDirectory, Path.DirectorySeparatorChar,
                                                          hash, ".avatar");

                        if (!ExistsAvatar(String.Format("{0}{1}", hash, ".avatar")))
                        {
                            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                            {
                                using (var stream =
                                    new IsolatedStorageFileStream(avatarFile, FileMode.Create, storage))
                                {
                                    stream.Write(avatarStream.ToArray(), 0, Convert.ToInt32(avatarStream.Length));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    // Check if the avatar is already added to the list
                    Avatar avatar = avatars.Where(a => a.Contact == contactId).SingleOrDefault();

                    if (avatar != null)
                    {
                        // Update the existent avatar information
                        avatar.Hash = hash;
                    }
                    else
                    {
                        // Add the new avatar to the list
                        avatars.Add(new Avatar(contactId, hash));
                    }
                }
            }
        }

        /// <summary>
        ///   Saves the configuration.
        /// </summary>
        public void Save() {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (var stream =
                    new IsolatedStorageFileStream(AvatarsFile, FileMode.OpenOrCreate, storage))
                {
                    using (var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        // Writer settings
                        xmlWriter.QuoteChar = '\'';
                        xmlWriter.Formatting = Formatting.Indented;

                        Serializer.Serialize(xmlWriter, this);
                    }
                }
            }
        }
    }
}