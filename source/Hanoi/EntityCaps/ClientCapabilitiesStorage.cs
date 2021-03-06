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

namespace Hanoi.Xmpp.InstantMessaging.EntityCaps
{
    [Serializable]
    [XmlTypeAttribute(Namespace = "")]
    [XmlRootAttribute("capabilities", Namespace = "", IsNullable = false)]
    public sealed class ClientCapabilitiesStorage
    {
        private string _clientCapabilitiesFile = "ClientCapabilities.xml";
        private static XmlSerializer _serializer = new XmlSerializer(typeof(ClientCapabilitiesStorage));
        private List<ClientCapabilities> _clientCapabilities;

        public ClientCapabilitiesStorage(string bareIdentifier)
        {
            _clientCapabilitiesFile = bareIdentifier + _clientCapabilitiesFile;
        }

        public ClientCapabilitiesStorage()
        {
            
        }

        [XmlArray("caps")]
        [XmlArrayItem("client", typeof(ClientCapabilities))]
        public List<ClientCapabilities> ClientCapabilities
        {
            get { return _clientCapabilities ?? (_clientCapabilities = new List<ClientCapabilities>()); }
        }

        public bool Exists(string node, string verificationString)
        {
            return (Get(node, verificationString) != null);
        }

        public ClientCapabilities Get(string node, string verificationString)
        {
            return
                (ClientCapabilities.Where(c => c.Node == node && c.VerificationString == verificationString).
                    SingleOrDefault());
        }

        public void Load()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (!storage.FileExists(_clientCapabilitiesFile))
                    return;

                using (var stream = new IsolatedStorageFileStream(_clientCapabilitiesFile, FileMode.OpenOrCreate, storage))
                {
                    if (stream.Length <= 0) 
                        return;
                    var capsstorage = (ClientCapabilitiesStorage)_serializer.Deserialize(stream);
                    ClientCapabilities.AddRange(capsstorage.ClientCapabilities);
                }
            }
        }

        public void Save()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (var stream = new IsolatedStorageFileStream(_clientCapabilitiesFile, FileMode.OpenOrCreate, storage))
                {
                    using (var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        xmlWriter.QuoteChar = '\'';
                        xmlWriter.Formatting = Formatting.Indented;
                        _serializer.Serialize(xmlWriter, this);
                    }
                }
            }
        }
    }
}