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

using BabelIm.Configuration;
using BabelIm.Infrastructure;

namespace BabelIm.ViewModels.Preferences {
    public sealed class ConnectionViewModel : ViewModel<Server> {
        private readonly BabelImConfiguration configuration;
        private readonly Server selectedServer;
        private ConfigurationManager configurationManager;

        public ConnectionViewModel(ConfigurationManager configurationManager) {
            this.configurationManager = configurationManager;
            configuration = configurationManager.GetConfiguration();

            if (configuration.Servers.Count == 0)
            {
                selectedServer = new Server();
            }
        }

        public string Name {
            get { return selectedServer.Name; }
            set { selectedServer.Name = value; }
        }

        public string ServerName {
            get { return selectedServer.ServerName; }
            set { selectedServer.ServerName = value; }
        }

        public string DomainName {
            get { return selectedServer.DomainName; }
            set { selectedServer.DomainName = value; }
        }

        public int PortNumber {
            get { return selectedServer.PortNumber; }
            set { selectedServer.PortNumber = value; }
        }

        public int Timeout {
            get { return selectedServer.Timeout; }
            set { selectedServer.Timeout = value; }
        }

        public bool UseProxy {
            get { return selectedServer.UseProxy; }
            set { selectedServer.UseProxy = value; }
        }

        public string ProxyType {
            get { return selectedServer.ProxyType; }
            set { selectedServer.ProxyType = value; }
        }

        public string ProxyServer {
            get { return selectedServer.ProxyServer; }
            set { selectedServer.ProxyServer = value; }
        }

        public int ProxyPortNumber {
            get { return selectedServer.ProxyPortNumber; }
            set { selectedServer.ProxyPortNumber = value; }
        }

        public string ProxyUserName {
            get { return selectedServer.ProxyUserName; }
            set { selectedServer.ProxyUserName = value; }
        }

        public string ProxyPassword {
            get { return selectedServer.ProxyPassword; }
            set { selectedServer.ProxyPassword = value; }
        }
    }
}