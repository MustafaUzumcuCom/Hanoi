using System;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using BabelIm.Contracts;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.InstantMessaging;
using BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery;
using BabelIm.ViewModels;

namespace BabelIm.Views
{
    /// <summary>
    /// Session view
    /// </summary>
    public partial class SessionView 
        : UserControl
    {
        #region · Constructors ·

        public SessionView()
        {
            InitializeComponent();

            this.DataContext = new SessionViewModel();

            ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Where(state => state == XmppSessionState.LoggingIn || state == XmppSessionState.LoggingOut)
                .Subscribe(newState => this.OnSessionStateChanged(newState));
        }

        #endregion

        #region · Private Methods ·

        private void Subscribe()
        {
            ServiceFactory.Current.Resolve<IXmppSession>().Roster.ContactPresenceChanged += new EventHandler(OnContactPresenceChanged);
        }

        private void Unsubscribe()
        {
            ServiceFactory.Current.Resolve<IXmppSession>().Roster.ContactPresenceChanged -= new EventHandler(OnContactPresenceChanged);
        }

        #endregion

        #region · Message Handlers ·

        private void OnSessionStateChanged(XmppSessionState newState)
        {
            if (newState == XmppSessionState.LoggingIn)
            {
                this.Subscribe();
            }
            else if (newState == XmppSessionState.LoggingOut)
            {
                this.Unsubscribe();
            }
        }

        #endregion

        #region · Event Handlers ·

        private void ActiveChatsZone_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.PivotControl.Width = e.NewSize.Width;
            this.PivotControl.Height = e.NewSize.Height;

            this.PivotControl.Invalidate();
        }

        private void OnContactPresenceChanged(object sender, EventArgs args)
        {
            this.Dispatcher.BeginInvoke
            (
                DispatcherPriority.ApplicationIdle,
                new ThreadStart
                (
                    delegate
                    {
                        CollectionViewSource viewSource = this.Resources["ContactsViewSource"] as CollectionViewSource;
                        viewSource.View.Refresh();
                    }
                )
            );
        }        

        #endregion

        #region · View Event Handlers ·

        private void UserImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
#warning TODO: Move this to the ViewModel using commands
            // this.ViewModel.SelectAvatarImage();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            XmppContact contact = (XmppContact)e.Item;

            e.Accepted = (contact.Presence.PresenceStatus != XmppPresenceState.Offline);
        }

        private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ContactsListBox.SelectedItem != null)
            {
                XmppContact         contact     = (XmppContact)this.ContactsListBox.SelectedItem;
                IChatViewManager    viewManager = ServiceFactory.Current.Resolve<IChatViewManager>();

                // Create the chatView
                viewManager.OpenChatView(contact.ContactId);
            }
        }

        #endregion

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IXmppSession session = ServiceFactory.Current.Resolve<IXmppSession>();

            if (session != null
                && session.ServiceDiscovery.SupportsMultiuserChat)
            {
                XmppService service = session.ServiceDiscovery.GetService(XmppServiceCategory.Conference);

                if (service != null)
                {
                    session.EnterChatRoom("babelimtest")
                           .Invite(session.Roster.Where(c => c.ContactId.BareIdentifier.Equals("babelimtest@neko.im")).SingleOrDefault());
                }
            }
        }
    }
}
