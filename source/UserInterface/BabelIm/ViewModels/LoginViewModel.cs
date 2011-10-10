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

namespace BabelIm.ViewModels {
    /// <summary>
    ///   ViewModel for login views
    /// </summary>
    public sealed class LoginViewModel
        : ViewModelBase {
        private string login;
        private ICommand loginCommand;
        private string password;
        private Account selectedAccount;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoginViewModel" /> class.
        /// </summary>
        /// <param name = "container"></param>
        /// <param name = "session"></param>
        public LoginViewModel() {
            Subscribe();
        }

        public ICommand LoginCommand {
            get {
                if (loginCommand == null)
                {
                    loginCommand = new RelayCommand(() => OnLogin());
                }

                return loginCommand;
            }
        }

        /// <summary>
        ///   Gets the available accounts
        /// </summary>
        public AccountCollection Accounts {
            get { return ServiceFactory.Current.Resolve<IConfigurationManager>().Configuration.Accounts.AccountCollection; }
        }

        /// <summary>
        ///   Gets the selected account
        /// </summary>
        public Account SelectedAccount {
            get { return selectedAccount; }
            set {
                if (selectedAccount != value)
                {
                    selectedAccount = value;
                    Login = SelectedAccount.Login.Username;
                    Password = SelectedAccount.Login.Password;

                    ServiceFactory.Current.Resolve<IConfigurationManager>().SelectedAccount = SelectedAccount;

                    NotifyPropertyChanged(() => SelectedAccount);
                }
            }
        }

        /// <summary>
        ///   Gets the login name
        /// </summary>
        public String Login {
            get { return login; }
            set {
                if (login != value)
                {
                    login = value;
                    NotifyPropertyChanged(() => Login);
                }
            }
        }

        /// <summary>
        ///   Gets the password
        /// </summary>
        public String Password {
            get { return password; }
            set {
                if (password != value)
                {
                    password = value;
                    NotifyPropertyChanged(() => Password);
                }
            }
        }

        /// <summary>
        ///   Performs the user login
        /// </summary>
        private void OnLogin() {
            ThreadPool.QueueUserWorkItem
                (
                    delegate
                        {
                            // Build the connection string
                            var session = ServiceFactory.Current.Resolve<IXmppSession>();
                            var builder = new XmppConnectionStringBuilder();
                            BabelImConfiguration configuration =
                                ServiceFactory.Current.Resolve<IConfigurationManager>().GetConfiguration();

                            if (selectedAccount != null &&
                                configuration.Servers[selectedAccount.Server] != null)
                            {
                                builder.UserId = String.Format
                                    (
                                        "{0}@{1}/{2}",
                                        Login,
                                        configuration.Servers[selectedAccount.Server].DomainName,
                                        selectedAccount.Resource
                                    );

                                Server serverInfo = configuration.Servers[selectedAccount.Server];

                                builder.HostName = serverInfo.ServerName;
                                builder.Port = serverInfo.PortNumber;
                                builder.Password = Password;
                                builder.UseProxy = serverInfo.UseProxy;
                                builder.ProxyServer = serverInfo.ProxyServer;
                                builder.ProxyPortNumber = serverInfo.ProxyPortNumber;
                                builder.ProxyType = serverInfo.ProxyType;
                                builder.ProxyUserName = serverInfo.ProxyUserName;
                                builder.ProxyPassword = serverInfo.ProxyPassword;
                                builder.ResolveHostName = serverInfo.ResolveHostName;
                                builder.UseHttpBinding = serverInfo.UseHttpBinding;

                                // Set client capabilities
                                session.Capabilities.Node = configuration.General.Capabilities.URI;
                                session.Capabilities.Name = configuration.General.Capabilities.Name;
                                session.Capabilities.Version = configuration.General.Capabilities.Version;
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
                );
        }

        private void Subscribe() {
            ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Where(state => state == XmppSessionState.LoggedIn || state == XmppSessionState.LoggedOut)
                .Subscribe(newState => { OnSessionStateChanged(newState); });
        }

        private void OnSessionStateChanged(XmppSessionState newState) {
            Application.Current.Dispatcher.Invoke
                (
                    (Action) delegate
                                 {
                                     if (newState == XmppSessionState.LoggedIn)
                                     {
                                         Password = String.Empty;
                                     }
                                     else if (newState == XmppSessionState.LoggedOut)
                                     {
                                         NotifyPropertyChanged(() => Accounts);
                                     }
                                 }
                );
        }
        }
}