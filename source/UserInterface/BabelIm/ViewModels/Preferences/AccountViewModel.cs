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
    public class AccountViewModel : ViewModel<Account>
    {
        #region · Fields ·

        private ConfigurationManager    configurationManager;
        private BabelImConfiguration    configuration;
        private Account                 selectedAccount;
        private Server                  selectedServer;
        private bool                    isNew;

        #endregion
        
        #region · Properties ·

        public bool IsNew
        {
            get { return this.isNew; }
            set { this.isNew = value; }
        }

        public AccountCollection Accounts
        {
             get { return this.configuration.Accounts.AccountCollection; }
        }

        public ServerCollection Servers
        {
            get { return this.configuration.Servers.ServerCollection; }
        }

        public Account SelectedAccount
        {
            get { return this.selectedAccount; }
            set
            {
                if (this.selectedAccount != value)
                {
                    this.selectedAccount = value;
                    this.selectedServer = this.Servers[this.selectedAccount.Server];
                }
            }
        }

        public Server SelectedServer
        {
            get { return this.selectedServer; }
            set
            {
                if (this.selectedServer!= value)
                {
                    this.selectedServer = value;
                    this.selectedAccount.Server = this.selectedServer.Name;
                }
            }
        }

        public string Name
        {
            get
            {
                if (this.selectedAccount != null)
                {
                    return this.selectedAccount.Name;
                }

                return String.Empty;
            }
            set { this.selectedAccount.Name = value; }
        }

        public Login Login
        {
            get
            {
                if (this.selectedAccount != null)
                {
                    return this.selectedAccount.Login;
                }

                return null;
            }
        }

        public string DisplayName
        {
            get
            {
                if (this.selectedAccount != null)
                {
                    return this.selectedAccount.DisplayName;
                }

                return String.Empty;
            }
            set { this.selectedAccount.DisplayName = value; }
        }

        /*
        public string Avatar
        {
            get { return this.avatar; }
            set { this.avatar = value; }
        }
        */

        public string Presence
        {
            get
            {
                if (this.selectedAccount != null)
                {
                    return this.selectedAccount.Presence;
                }

                return String.Empty;
            }
            set { this.selectedAccount.Presence = value; }
        }

        public string Status
        {
            get
            {
                if (this.selectedAccount != null)
                {
                    return this.selectedAccount.Status;
                }

                return String.Empty;
            }
            set { this.selectedAccount.Status = value; }
        }

        public string Resource
        {
            get
            {
                if (this.selectedAccount != null)
                {
                    return this.selectedAccount.Resource;
                }

                return String.Empty;
            }
            set { this.selectedAccount.Resource = value; }
        }

        #endregion

        #region · Constructors ·

        public AccountViewModel(ConfigurationManager configurationManager)
            : base()
        {
            this.configurationManager   = configurationManager;
            this.configuration          = configurationManager.GetConfiguration();

            if (this.configuration.Accounts.Count == 0)
            {
                this.selectedAccount        = new Account();
                this.selectedAccount.Name   = "New Account";
                this.IsNew                  = true;
            }
        }

        #endregion
    }
}
