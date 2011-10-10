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

namespace BabelIm.ViewModels.Preferences {
    public class AccountViewModel : ViewModel<Account> {
        private readonly BabelImConfiguration configuration;
        private ConfigurationManager configurationManager;
        private Account selectedAccount;
        private Server selectedServer;

        public AccountViewModel(ConfigurationManager configurationManager) {
            this.configurationManager = configurationManager;
            configuration = configurationManager.GetConfiguration();

            if (configuration.Accounts.Count == 0)
            {
                selectedAccount = new Account();
                selectedAccount.Name = "New Account";
                IsNew = true;
            }
        }

        public bool IsNew { get; set; }

        public AccountCollection Accounts {
            get { return configuration.Accounts.AccountCollection; }
        }

        public ServerCollection Servers {
            get { return configuration.Servers.ServerCollection; }
        }

        public Account SelectedAccount {
            get { return selectedAccount; }
            set {
                if (selectedAccount != value)
                {
                    selectedAccount = value;
                    selectedServer = Servers[selectedAccount.Server];
                }
            }
        }

        public Server SelectedServer {
            get { return selectedServer; }
            set {
                if (selectedServer != value)
                {
                    selectedServer = value;
                    selectedAccount.Server = selectedServer.Name;
                }
            }
        }

        public string Name {
            get {
                if (selectedAccount != null)
                {
                    return selectedAccount.Name;
                }

                return String.Empty;
            }
            set { selectedAccount.Name = value; }
        }

        public Login Login {
            get {
                if (selectedAccount != null)
                {
                    return selectedAccount.Login;
                }

                return null;
            }
        }

        public string DisplayName {
            get {
                if (selectedAccount != null)
                {
                    return selectedAccount.DisplayName;
                }

                return String.Empty;
            }
            set { selectedAccount.DisplayName = value; }
        }

        /*
        public string Avatar
        {
            get { return this.avatar; }
            set { this.avatar = value; }
        }
        */

        public string Presence {
            get {
                if (selectedAccount != null)
                {
                    return selectedAccount.Presence;
                }

                return String.Empty;
            }
            set { selectedAccount.Presence = value; }
        }

        public string Status {
            get {
                if (selectedAccount != null)
                {
                    return selectedAccount.Status;
                }

                return String.Empty;
            }
            set { selectedAccount.Status = value; }
        }

        public string Resource {
            get {
                if (selectedAccount != null)
                {
                    return selectedAccount.Resource;
                }

                return String.Empty;
            }
            set { selectedAccount.Resource = value; }
        }
    }
}