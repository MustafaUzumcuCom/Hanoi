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

namespace BabelIm.Configuration
{
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class ServerCollection 
        : CollectionBase
    {
        #region · Indexers ·

        public Server this[int index]
        {
            get { return (Server)this.List[index]; }
            set { this.List[index] = value; }
        }

        public Server this[string name]
        {
            get
            {
                int index = this.IndexOf(name);

                if (index == -1)
                {
                    return null;
                }

                return this[index];
            }

            set { this[name] = value; }
        }

        #endregion

        #region · Constructors ·

        public ServerCollection()
        {
        }

        #endregion

        #region · Methods ·

        public Server Add()
        {
            return Add(new Server());
        }

        public Server Add(Server obj)
        {
            this.List.Add(obj);

            return obj;
        }

        public void Insert(int index, Server obj)
        {
            this.List.Insert(index, obj);
        }

        public void Remove(Server obj)
        {
            this.List.Remove(obj);
        }

        public int IndexOf(Server obj)
        {
            return this.List.IndexOf(obj);
        }

        public int IndexOf(string name)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                if (this[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
