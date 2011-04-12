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
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BabelIm.Configuration;
using BabelIm.Contracts;
using BabelIm.Infrastructure;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.InstantMessaging;
using BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery;

namespace BabelIm.ViewModels
{
    /// <summary>
    /// ViewModel for login views
    /// </summary>
    public sealed class LoginViewModel 
        : ViewModelBase
    {
        #region · Fields ·

        private string      login;
        private string      password;
        private Account     selectedAccount;
        private ICommand    loginCommand;

        #endregion

        #region · Commands ·

        public ICommand LoginCommand
        {
            get
            {
                if (this.loginCommand == null)
                {
                    this.loginCommand = new RelayCommand(() => OnLogin());
                }

                return this.loginCommand;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the available accounts
        /// </summary>
        public AccountCollection Accounts
        {
            get { return ServiceFactory.Current.Resolve<IConfigurationManager>().Configuration.Accounts.AccountCollection; }
        }

        /// <summary>
        /// Gets the selected account
        /// </summary>
        public Account SelectedAccount
        {
            get { return this.selectedAccount; }
            set
            {
                if (this.selectedAccount != value)
                {
                    this.selectedAccount    = value;
                    this.Login              = this.SelectedAccount.Login.Username;
                    this.Password           = this.SelectedAccount.Login.Password;

                    ServiceFactory.Current.Resolve<IConfigurationManager>().SelectedAccount = this.SelectedAccount;

                    this.NotifyPropertyChanged(() => SelectedAccount);
                }
            }
        }

        /// <summary>
        /// Gets the login name
        /// </summary>
        public String Login
        {
            get { return this.login; }
            set
            {
                if (this.login != value)
                {
                    this.login = value;
                    this.NotifyPropertyChanged(() => Login);
                }
            }
        }

        /// <summary>
        /// Gets the password
        /// </summary>
        public String Password
        {
            get { return this.password; }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    this.NotifyPropertyChanged(() => Password);
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel" /> class.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="session"></param>
        public LoginViewModel()
            : base()
        {
            this.Subscribe();
        }

        #endregion

        #region · Command Actions ·

        /// <summary>
        /// Performs the user login
        /// </summary>
        private void OnLogin()
        {
            ThreadPool.QueueUserWorkItem
            (
                new WaitCallback
                (
                    delegate
                    {
                        // Build the connection string
                        IXmppSession                session         = ServiceFactory.Current.Resolve<IXmppSession>();
                        XmppConnectionStringBuilder builder         = new XmppConnectionStringBuilder();
                        BabelImConfiguration        configuration   = ServiceFactory.Current.Resolve<IConfigurationManager>().GetConfiguration();

                        if (this.selectedAccount != null &&
                            configuration.Servers[this.selectedAccount.Server] != null)
                        {
                            builder.UserId = String.Format
                            (
                                "{0}@{1}/{2}", 
                                this.Login, 
                                configuration.Servers[this.selectedAccount.Server].DomainName, 
                                this.selectedAccount.Resource
                            );

                            Server serverInfo = configuration.Servers[this.selectedAccount.Server];

                            builder.HostName          = serverInfo.ServerName;
                            builder.Port            = serverInfo.PortNumber;
                            builder.Password        = this.Password;
                            builder.UseProxy        = serverInfo.UseProxy;
                            builder.ProxyServer     = serverInfo.ProxyServer;
                            builder.ProxyPortNumber = serverInfo.ProxyPortNumber;
                            builder.ProxyType       = serverInfo.ProxyType;
                            builder.ProxyUserName   = serverInfo.ProxyUserName;
                            builder.ProxyPassword   = serverInfo.ProxyPassword;
                            builder.ResolveHostName = serverInfo.ResolveHostName;
                            builder.UseHttpBinding  = serverInfo.UseHttpBinding;

                            // Set client capabilities
                            session.Capabilities.Node                  = configuration.General.Capabilities.URI;
                            session.Capabilities.Name                  = configuration.General.Capabilities.Name;
                            session.Capabilities.Version               = configuration.General.Capabilities.Version;
                            session.PersonalEventing.IsUserTuneEnabled = configuration.General.UserTuneEnabled;

                            session.Capabilities.Identities.Add
                            (
                                new XmppServiceIdentity
                                (
                                    configuration.General.Capabilities.Name,
                                    configuration.General.Capabilities.IdentityCategory,
                                    configuration.General.Capabilities.IdentityType
                                )
                            );

                            session.Open(builder.ToString());
                        }
                    }
                )
            );
        }

        #endregion

        #region · Actions Subscriptions ·

        private void Subscribe()
        {
            ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Where(state => state == XmppSessionState.LoggedIn || state == XmppSessionState.LoggedOut)
                .Subscribe(newState => { this.OnSessionStateChanged(newState); });
        }

        #endregion

        #region · Message Handlers ·

        private void OnSessionStateChanged(XmppSessionState newState)
        {
            Application.Current.Dispatcher.Invoke
            (
                (Action)delegate
                {
                    if (newState == XmppSessionState.LoggedIn)
                    {
                        this.Password = String.Empty;
                    }
                    else if (newState == XmppSessionState.LoggedOut)
                    {
                        this.NotifyPropertyChanged(() => Accounts);
                    }
                }
            );
        }

        #endregion
    }
}
