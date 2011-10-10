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

namespace BabelIm.Views {
    /// <summary>
    ///   Session view
    /// </summary>
    public partial class SessionView
        : UserControl {
        public SessionView() {
            InitializeComponent();

            DataContext = new SessionViewModel();

            ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Where(state => state == XmppSessionState.LoggingIn || state == XmppSessionState.LoggingOut)
                .Subscribe(newState => OnSessionStateChanged(newState));
        }

        private void Subscribe() {
            ServiceFactory.Current.Resolve<IXmppSession>().Roster.ContactPresenceChanged += OnContactPresenceChanged;
        }

        private void Unsubscribe() {
            ServiceFactory.Current.Resolve<IXmppSession>().Roster.ContactPresenceChanged -= OnContactPresenceChanged;
        }

        private void OnSessionStateChanged(XmppSessionState newState) {
            if (newState == XmppSessionState.LoggingIn)
            {
                Subscribe();
            }
            else if (newState == XmppSessionState.LoggingOut)
            {
                Unsubscribe();
            }
        }

        private void ActiveChatsZone_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
            PivotControl.Width = e.NewSize.Width;
            PivotControl.Height = e.NewSize.Height;

            PivotControl.Invalidate();
        }

        private void OnContactPresenceChanged(object sender, EventArgs args) {
            Dispatcher.BeginInvoke
                (
                    DispatcherPriority.ApplicationIdle,
                    new ThreadStart
                        (
                        delegate
                            {
                                var viewSource = Resources["ContactsViewSource"] as CollectionViewSource;
                                viewSource.View.Refresh();
                            }
                        )
                );
        }

        private void UserImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
#warning TODO: Move this to the ViewModel using commands
            // this.ViewModel.SelectAvatarImage();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e) {
            var contact = (XmppContact) e.Item;

            e.Accepted = (contact.Presence.PresenceStatus != XmppPresenceState.Offline);
        }

        private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e) {
            if (ContactsListBox.SelectedItem != null)
            {
                var contact = (XmppContact) ContactsListBox.SelectedItem;
                var viewManager = ServiceFactory.Current.Resolve<IChatViewManager>();

                // Create the chatView
                viewManager.OpenChatView(contact.ContactId);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
            var session = ServiceFactory.Current.Resolve<IXmppSession>();

            if (session != null
                && session.ServiceDiscovery.SupportsMultiuserChat)
            {
                XmppService service = session.ServiceDiscovery.GetService(XmppServiceCategory.Conference);

                if (service != null)
                {
                    session.EnterChatRoom("babelimtest")
                        .Invite(
                            session.Roster.Where(c => c.ContactId.BareIdentifier.Equals("babelimtest@neko.im")).
                                SingleOrDefault());
                }
            }
        }
        }
}