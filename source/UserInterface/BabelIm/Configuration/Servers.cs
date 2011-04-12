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
using System.Collections;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BabelIm.Configuration
{
    [XmlType(TypeName = "servers"), Serializable]
    public sealed class Servers
    {
        #region · Indexers ·

        [XmlIgnore]
        public Server this[int index]
        {
            get { return (Server)this.ServerCollection[index]; }
        }

        [XmlIgnore]
        public Server this[string name]
        {
            get { return (Server)this.ServerCollection[name]; }
        }

        #endregion

        #region · Fields ·

        private ServerCollection serverCollection;

        #endregion

        #region · Properties ·

        [XmlElement(Type = typeof(Server), ElementName = "server", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ServerCollection ServerCollection
        {
            get
            {
                if (this.serverCollection == null)
                {
                    this.serverCollection = new ServerCollection();
                }
                return this.serverCollection;
            }
        }

        [XmlIgnore]
        public int Count
        {
            get { return this.ServerCollection.Count; }
        }

        #endregion

        #region · Constructors ·

        public Servers()
        {
        }

        #endregion

        #region · Methods ·

        public void Clear()
        {
            this.ServerCollection.Clear();
        }

        [System.Runtime.InteropServices.DispIdAttribute(-4)]
        public IEnumerator GetEnumerator()
        {
            return this.ServerCollection.GetEnumerator();
        }

        public Server Add(Server obj)
        {
            return this.ServerCollection.Add(obj);
        }

        public Server Remove(int index)
        {
            Server obj = this.ServerCollection[index];
            this.ServerCollection.Remove(obj);

            return obj;
        }

        public void Remove(Server obj)
        {
            this.ServerCollection.Remove(obj);
        }

        #endregion
    }
}
