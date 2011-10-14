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

namespace BabelIm.Net.Xmpp.InstantMessaging.EntityCaps {
    /// <summary>
    ///   Entity capabilities store
    /// </summary>
    [Serializable]
    [XmlTypeAttribute(Namespace = "")]
    [XmlRootAttribute("capabilities", Namespace = "", IsNullable = false)]
    public sealed class XmppClientCapabilitiesStorage {
        private const string ClientCapabilitiesFile = "ClientCapabilities.xml";
        private static XmlSerializer Serializer = new XmlSerializer(typeof (XmppClientCapabilitiesStorage));

        private List<XmppClientCapabilities> clientCapabilities;

        [XmlArray("caps")]
        [XmlArrayItem("client", typeof (XmppClientCapabilities))]
        public List<XmppClientCapabilities> ClientCapabilities {
            get {
                if (clientCapabilities == null)
                {
                    clientCapabilities = new List<XmppClientCapabilities>();
                }

                return clientCapabilities;
            }
        }

        public bool Exists(string node, string verificationString) {
            return (Get(node, verificationString) != null);
        }

        public XmppClientCapabilities Get(string node, string verificationString) {
            return
                (ClientCapabilities.Where(c => c.Node == node && c.VerificationString == verificationString).
                    SingleOrDefault());
        }

        public void Load() {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.FileExists(ClientCapabilitiesFile))
                {
                    using (var stream =
                        new IsolatedStorageFileStream(ClientCapabilitiesFile, FileMode.OpenOrCreate, storage))
                    {
                        if (stream.Length > 0)
                        {
                            var capsstorage = (XmppClientCapabilitiesStorage) Serializer.Deserialize(stream);

                            ClientCapabilities.AddRange(capsstorage.ClientCapabilities);
                        }
                    }
                }
            }
        }

        public void Save() {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (var stream =
                    new IsolatedStorageFileStream(ClientCapabilitiesFile, FileMode.OpenOrCreate, storage))
                {
                    // Save Caps as XML
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