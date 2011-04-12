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
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BabelIm.Configuration
{
    [XmlType(TypeName = "accounts"), Serializable]
    public sealed class Accounts
    {
        #region · Fields ·

        private AccountCollection accountCollection;

        #endregion

        #region · Indexers ·

        [XmlIgnore]
        public Account this[int index]
        {
            get { return (Account)this.AccountCollection[index]; }
        }

        #endregion

        #region · Properties ·

        [XmlIgnore]
        public int Count
        {
            get { return AccountCollection.Count; }
        }

        [XmlElement(Type = typeof(Account), ElementName = "account", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public AccountCollection AccountCollection
        {
            get
            {
                if (this.accountCollection == null)
                {
                    this.accountCollection = new AccountCollection();
                }
                return this.accountCollection;
            }
            set { this.accountCollection = value; }
        }

        #endregion

        #region · Constructors ·

        public Accounts()
        {
        }

        #endregion

        #region · Methods ·

        [DispIdAttribute(-4)]
        public IEnumerator GetEnumerator()
        {
            return AccountCollection.GetEnumerator();
        }

        public Account Add(Account obj)
        {
            return this.AccountCollection.Add(obj);
        }

        public void Clear()
        {
            this.AccountCollection.Clear();
        }

        public Account Remove(int index)
        {
            Account obj = this.AccountCollection[index];
            this.AccountCollection.Remove(obj);

            return obj;
        }

        #endregion
    }
}
