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
using BabelIm.Configuration;
using BabelIm.Infrastructure;

namespace BabelIm.ViewModels.Preferences
{
    public sealed class ConnectionViewModel : ViewModel<Server>
    {
        #region · Fields ·

        private ConfigurationManager    configurationManager;
        private BabelImConfiguration    configuration;
        private Server                  selectedServer;

        #endregion

        #region · Properties ·

        public string Name
        {
            get { return this.selectedServer.Name; }
            set { this.selectedServer.Name = value; }
        }

        public string ServerName
        {
            get { return this.selectedServer.ServerName; }
            set { this.selectedServer.ServerName = value; }
        }

        public string DomainName
        {
            get { return this.selectedServer.DomainName; }
            set { this.selectedServer.DomainName = value; }
        }

        public int PortNumber
        {
            get { return this.selectedServer.PortNumber; }
            set { this.selectedServer.PortNumber = value; }
        }

        public int Timeout
        {
            get { return this.selectedServer.Timeout; }
            set { this.selectedServer.Timeout = value; }
        }

        public bool UseProxy
        {
            get { return this.selectedServer.UseProxy; }
            set { this.selectedServer.UseProxy = value; }
        }

        public string ProxyType
        {
            get { return this.selectedServer.ProxyType; }
            set { this.selectedServer.ProxyType = value; }
        }

        public string ProxyServer
        {
            get { return this.selectedServer.ProxyServer; }
            set { this.selectedServer.ProxyServer = value; }
        }

        public int ProxyPortNumber
        {
            get { return this.selectedServer.ProxyPortNumber; }
            set { this.selectedServer.ProxyPortNumber = value; }
        }

        public string ProxyUserName
        {
            get { return this.selectedServer.ProxyUserName; }
            set { this.selectedServer.ProxyUserName = value; }
        }

        public string ProxyPassword
        {
            get { return this.selectedServer.ProxyPassword; }
            set { this.selectedServer.ProxyPassword = value; }
        }

        #endregion

        #region · Constructors ·

        public ConnectionViewModel(ConfigurationManager configurationManager)
            : base()
        {
            this.configurationManager   = configurationManager;
            this.configuration          = configurationManager.GetConfiguration();

            if (this.configuration.Servers.Count == 0)
            {
                this.selectedServer = new Server();
            }
        }

        #endregion
    }
}
