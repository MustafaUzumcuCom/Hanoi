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
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Hanoi.Xmpp.InstantMessaging
{
    [Serializable]
    [XmlType(Namespace = "")]
    [XmlRootAttribute("storage", Namespace = "", IsNullable = false)]
    public sealed class AvatarStorage
    {
        private static XmlSerializer _serializer = new XmlSerializer(typeof(AvatarStorage));
        private readonly string _avatarsDirectory = "Avatars";
        private string _avatarsFile = "Avatars.xml";
        private object _syncObject = new object();
        private List<Avatar> _avatars = new List<Avatar>();

        public AvatarStorage()
        {

        }

        public AvatarStorage(string jid)
        {
            _avatarsFile = jid + _avatarsFile;
        }
        
        [XmlArray("avatars"), XmlArrayItem("avatar", typeof(Avatar))]
        public List<Avatar> Avatars
        {
            get { return _avatars; }
            set { _avatars = value; }
        }

        private bool ExistsAvatar(string file)
        {
            var filename = _avatarsDirectory + Path.DirectorySeparatorChar + file;

            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.GetDirectoryNames(_avatarsDirectory).Length == 0)
                {
                    storage.CreateDirectory(_avatarsDirectory);
                }

                return (storage.GetFileNames(filename).Length != 0);
            }
        }

        private byte[] ReadFile(string file)
        {
            string filename = _avatarsDirectory + Path.DirectorySeparatorChar + file;
            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.GetDirectoryNames(_avatarsDirectory).Length == 0)
                {
                    storage.CreateDirectory(_avatarsDirectory);
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

        public string GetAvatarHash(string contactId)
        {
            Avatar avatar = Avatars.Where(a => a.Contact == contactId).SingleOrDefault();
            return avatar != null ? avatar.Hash : null;
        }

        public Stream ReadAvatar(string contactId)
        {
            lock (_syncObject)
            {
                Avatar avatar = Avatars.Where(a => a.Contact == contactId).SingleOrDefault();

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
        public void RemoveAvatar(string contactId)
        {
            lock (_syncObject)
            {
                try
                {
                    Avatars.Remove(Avatars.Where(a => a.Contact == contactId).SingleOrDefault());
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
        public void Load()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (!storage.FileExists(_avatarsFile)) 
                    return;

                using (var stream = new IsolatedStorageFileStream(_avatarsFile, FileMode.OpenOrCreate, storage))
                {
                    if (stream.Length <= 0) 
                        return;

                    var avatarStorage = (AvatarStorage)_serializer.Deserialize(stream);
                    Avatars.AddRange(avatarStorage.Avatars);
                }
            }
        }

        public void SaveAvatar(string contactId, string hash, MemoryStream avatarStream)
        {
            lock (_syncObject)
            {
                // The avatar files should be saved only if it's not in use by another user ( several users can share the same avatar )
                var q = from userAvatar in Avatars
                        where userAvatar.Hash == hash
                        select userAvatar;

                try
                {
                    if (q.Count() == 0 && avatarStream.Length > 0)
                    {
                        var avatarFile = string.Format("{0}{1}{2}{3}", _avatarsDirectory, Path.DirectorySeparatorChar, hash, ".avatar");
                        if (!ExistsAvatar(string.Format("{0}{1}", hash, ".avatar")))
                        {
                            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
                            {
                                using (var stream = new IsolatedStorageFileStream(avatarFile, FileMode.Create, storage))
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
                    Avatar avatar = Avatars.Where(a => a.Contact == contactId).SingleOrDefault();

                    if (avatar != null)
                    {
                        // Update the existent avatar information
                        avatar.Hash = hash;
                    }
                    else
                    {
                        // Add the new avatar to the list
                        Avatars.Add(new Avatar(contactId, hash));
                    }
                }
            }
        }

        public void Save()
        {
            using (var xmlWriter = new XmlTextWriter(Path.Combine(FilePath.Directory, _avatarsFile), Encoding.UTF8))
            {
                xmlWriter.QuoteChar = '\'';
                xmlWriter.Formatting = Formatting.Indented;
                _serializer.Serialize(xmlWriter, this);
            }
        }
    }
}