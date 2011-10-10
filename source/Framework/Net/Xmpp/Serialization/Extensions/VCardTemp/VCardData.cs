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
using System.Xml.Serialization;
using BabelIm.Net.Xmpp.Serialization.Extensions.VCardAvatars;

namespace BabelIm.Net.Xmpp.Serialization.Extensions.VCardTemp {
    /// <summary>
    ///   XEP-0054: vcard-temp
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "vcard-temp")]
    [XmlRoot("x", Namespace = "vcard-temp", IsNullable = false)]
    public class VCardData {
        private string jabberId;
        private string nickName;
        private VCardPhoto photo;

        [XmlElement("NICKNAME")]
        public string NickName {
            get { return nickName; }
            set { nickName = value; }
        }

        [XmlElement("JABBERID")]
        public string JabberId {
            get { return jabberId; }
            set { jabberId = value; }
        }

        [XmlElement("PHOTO")]
        public VCardPhoto Photo {
            get {
                if (photo == null)
                {
                    photo = new VCardPhoto();
                }
                return photo;
            }
            set { photo = value; }
        }
    }
}