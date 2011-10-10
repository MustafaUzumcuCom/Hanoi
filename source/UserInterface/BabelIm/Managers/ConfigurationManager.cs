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

using System.Windows.Input;
using BabelIm.Configuration;
using BabelIm.Contracts;
using BabelIm.Infrastructure;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.InstantMessaging;

namespace BabelIm {
    /// <summary>
    ///   Configuration manager class
    /// </summary>
    public sealed class ConfigurationManager
        : IConfigurationManager {
        private BabelImConfiguration configuration;
        private RelayCommand openAccountPreferencesViewCommand;
        private RelayCommand openPreferencesViewCommand;
        private RelayCommand openServerPreferencesViewCommand;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ConfigurationManager" /> class.
        /// </summary>
        public ConfigurationManager() {
            configuration = BabelImConfiguration.GetConfiguration();
#warning TODO: Receive Session chage messages
        }

        #region IConfigurationManager Members

        /// <summary>
        ///   Gets or sets the open preferences view command
        /// </summary>
        /// <value>The add new command.</value>
        public ICommand OpenPreferencesViewCommand {
            get {
                if (openPreferencesViewCommand == null)
                {
                    openPreferencesViewCommand = new RelayCommand
                        (
                        () => OnOpenPreferencesView(),
                        () => CanOpenPreferencesView()
                        );
                }

                return openPreferencesViewCommand;
            }
        }

        /// <summary>
        ///   Gets or sets the open server preferences view command
        /// </summary>
        /// <value>The add new command.</value>
        public ICommand OpenServerPreferencesViewCommand {
            get {
                if (openServerPreferencesViewCommand == null)
                {
                    openServerPreferencesViewCommand = new RelayCommand
                        (
                        () => OnOpenServerPreferencesView(),
                        () => CanOpenServerPreferencesView()
                        );
                }

                return openServerPreferencesViewCommand;
            }
        }

        /// <summary>
        ///   Gets or sets the open account preferences view command
        /// </summary>
        /// <value>The add new command.</value>
        public ICommand OpenAccountPreferencesViewCommand {
            get {
                if (openAccountPreferencesViewCommand == null)
                {
                    openAccountPreferencesViewCommand = new RelayCommand
                        (
                        () => OnOpenAccountPreferencesView(),
                        () => CanOpenAccountPreferencesView()
                        );
                }

                return openServerPreferencesViewCommand;
            }
        }

        /// <summary>
        ///   Gets the account information selected for login
        /// </summary>
        public Account SelectedAccount { get; set; }

        /// <summary>
        ///   Gets the application configuration information
        /// </summary>
        public BabelImConfiguration Configuration {
            get { return configuration; }
        }

        /// <summary>
        ///   Gets the configuration by reading it from the configuration file.
        /// </summary>
        /// <returns></returns>
        public BabelImConfiguration GetConfiguration() {
            configuration = BabelImConfiguration.GetConfiguration();

            return Configuration;
        }

        #endregion

        /// <summary>
        ///   Gets a value that indicates if the view can be closed
        /// </summary>
        /// <param name = "obj"></param>
        /// <returns></returns>
        private bool CanOpenPreferencesView() {
            return (ServiceFactory.Current.Resolve<IXmppSession>().State == XmppSessionState.LoggedIn);
        }

        /// <summary>
        ///   Closes the view
        /// </summary>
        /// <param name = "obj"></param>
        private void OnOpenPreferencesView() {
#warning TODO: Open and Show the preferences view
        }

        /// <summary>
        ///   Gets a value that indicates if the view can be closed
        /// </summary>
        /// <param name = "obj"></param>
        /// <returns></returns>
        private bool CanOpenServerPreferencesView() {
            return (ServiceFactory.Current.Resolve<IXmppSession>().State == XmppSessionState.LoggedOut);
        }

        /// <summary>
        ///   Opens the Server Preferences View
        /// </summary>
        /// <param name = "obj"></param>
        private void OnOpenServerPreferencesView() {
#warning TODO: Open and Show the preferences view
        }

        /// <summary>
        ///   Gets a value that indicates if the view can be closed
        /// </summary>
        /// <param name = "obj"></param>
        /// <returns></returns>
        private bool CanOpenAccountPreferencesView() {
            return (ServiceFactory.Current.Resolve<IXmppSession>().State == XmppSessionState.LoggedOut);
        }

        /// <summary>
        ///   Opens the Account preferences view
        /// </summary>
        /// <param name = "obj"></param>
        private void OnOpenAccountPreferencesView() {
#warning TODO: Open and Show the preferences view
        }
        }
}