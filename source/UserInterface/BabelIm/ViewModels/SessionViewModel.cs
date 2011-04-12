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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using BabelIm.Configuration;
using BabelIm.Contracts;
using BabelIm.Infrastructure;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.InstantMessaging;
using Microsoft.Win32;

namespace BabelIm.ViewModels
{
    /// <summary>
    /// ViewModel for session views
    /// </summary>
    public sealed class SessionViewModel 
        : ViewModel<XmppSession>
    {
        #region · Constants ·

        static readonly int IdleTime = 30;

        #endregion

        #region · Fields ·

        private Account             currentAccount;
        private XmppPresenceState   presenceState;
        private XmppPresenceState   previousPresenceState;
        private XmppContact         userContact;
        private DispatcherTimer		dispatcherTimer;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the user display name
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (ServiceFactory.Current.Resolve<IXmppSession>().State != XmppSessionState.LoggedIn ||
                    this.currentAccount == null)
                {
                    return String.Empty;
                }

                return this.currentAccount.DisplayName;
            }
        }

        /// <summary>
        /// Gets the user <see cref="XmppPresenceState">presence state</see>
        /// </summary>
        public XmppPresenceState PresenceState
        {
            get { return this.presenceState; }
            set 
            {
                if (this.presenceState != value)
                {
                    ServiceFactory.Current.Resolve<IConfigurationManager>().Configuration.Save();

                    this.previousPresenceState      = this.presenceState;
                    this.presenceState              = value;

                    if (this.currentAccount != null)
                    {
                        this.currentAccount.Presence = this.presenceState.ToString();
                    }

                    if (ServiceFactory.Current.Resolve<IXmppSession>().State == XmppSessionState.LoggedIn)
                    {
                        ServiceFactory.Current.Resolve<IXmppSession>().SetPresence(value, this.Status);
                    }

                    this.NotifyPropertyChanged(() => PresenceState);
                }
            }
        }

        /// <summary>
        /// Gets the user avatar
        /// </summary>
        public Stream AvatarImage
        {
            get
            {
                if (ServiceFactory.Current.Resolve<IXmppSession>().State == XmppSessionState.LoggedIn)
                {
                    return ServiceFactory.Current.Resolve<IXmppSession>().AvatarStorage.ReadAvatar(ServiceFactory.Current.Resolve<IXmppSession>().UserId.BareIdentifier);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the user status message
        /// </summary>
        public string Status
        {
            get
            {
                if (ServiceFactory.Current.Resolve<IXmppSession>().State != XmppSessionState.LoggedIn ||
                    this.currentAccount == null)
                {
                    return String.Empty;
                }
                return this.currentAccount.Status;
            }
            set
            {
                if (this.currentAccount != null)
                {
                    if (this.currentAccount.Status != value)
                    {
                        ServiceFactory.Current.Resolve<IConfigurationManager>().Configuration.Save();
                        this.currentAccount.Status = value;
                        ServiceFactory.Current.Resolve<IXmppSession>().SetPresence(this.PresenceState, value);

                        this.NotifyPropertyChanged(() => Status);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the contact list
        /// </summary>
        public XmppRoster Contacts
        {
            get { return ServiceFactory.Current.Resolve<IXmppSession>().Roster; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Inicializes a new instance of the <see cref="SessionViewModel" /> class.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="regionManager"></param>
        /// <param name="viewManager"></param>
        public SessionViewModel()
            : base()
        {
            this.Subscribe();
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Selects a new avatar image for the current user
        /// </summary>
        public void SelectAvatarImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Pictures|*.png;*.gif*;*.jpg;*.bmp|All Files|*.*";

            if (ofd.ShowDialog().Value)
            {
                // Publish the avatar
                ThreadPool.QueueUserWorkItem(new WaitCallback(PublishAvatar), ofd.FileName);
            }
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

        private void OnSessionStateChanged(XmppSessionState newSessionState)
        {
            if (newSessionState == XmppSessionState.LoggedIn)
            {
                this.Initialize();
            }
            else if (newSessionState == XmppSessionState.LoggedOut)
            {
                if (this.dispatcherTimer != null)
                {
                    this.dispatcherTimer.Stop();
                    this.dispatcherTimer = null;
                }
            }

            this.NotifyAllPropertiesChanged();
        }
        
        #endregion

        #region · Private Methods ·

        private void PublishAvatar(object state)
        {
            string          avatarFilePath  = (string)state;
            MemoryStream    stream          = new MemoryStream();
            Image           selectedImage   = Image.FromFile(avatarFilePath);
            Image           avatarImage     = null;

            try
            {
                avatarImage = selectedImage.Resize(new Size(96, 96));
                avatarImage.Save(stream, ImageFormat.Png);

                // Calculate the avatar hash
                string hash = stream.ToArray().ComputeSHA1Hash().ToHexString();

                // Publish the avatar image
                ServiceFactory.Current.Resolve<IXmppSession>().PublishAvatar("image/png", hash, avatarImage);

                this.Dispatcher.BeginInvoke(
                    DispatcherPriority.ApplicationIdle,
                    new ThreadStart(delegate
                    {
                        // Update the configuration
                        this.currentAccount.Avatar = hash;

                        // Save configuration
                        ServiceFactory.Current.Resolve<IConfigurationManager>().Configuration.Save();

                        // Notify avatar change
                        this.NotifyPropertyChanged(() => AvatarImage);
                    }));
            }
            catch
            {
#warning TODO: Handle Exception !!
                throw;
            }
            finally
            {
                if (selectedImage != null)
                {
                    selectedImage.Dispose();
                }

                if (avatarImage != null)
                {
                    avatarImage.Dispose();
                    avatarImage = null;
                }

                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        private void Initialize()
        {
            this.currentAccount     = ServiceFactory.Current.Resolve<IConfigurationManager>().SelectedAccount;
            this.dispatcherTimer    = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);

            if (this.currentAccount != null)
            {
                this.PresenceState = (XmppPresenceState)Enum.Parse(typeof(XmppPresenceState), this.currentAccount.Presence);
                this.userContact   = ServiceFactory.Current.Resolve<IXmppSession>().Roster[ServiceFactory.Current.Resolve<IXmppSession>().UserId.BareIdentifier];
            }
            else
            {
                this.presenceState = XmppPresenceState.Offline;
            }

            this.dispatcherTimer.Interval   = new TimeSpan(0, 0, 5);
            this.dispatcherTimer.Tick       += delegate(object s, EventArgs args)
            {
                if (Win32NativeMethods.GetIdleTime() >= IdleTime)
                {
                    if (this.PresenceState != XmppPresenceState.Idle)
                    {
                        this.PresenceState = XmppPresenceState.Idle;
                    }
                }
                else
                {
                    if (this.PresenceState == XmppPresenceState.Idle)
                    {
                        this.PresenceState = this.previousPresenceState;
                    }
                }
            };

            this.dispatcherTimer.Start();
        }

        #endregion
    }
}
