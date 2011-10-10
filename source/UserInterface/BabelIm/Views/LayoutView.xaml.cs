using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.InstantMessaging;
using BabelIm.ViewModels;

namespace BabelIm.Views {
    /// <summary>
    ///   Interaction logic for MainView.xaml
    /// </summary>
    public partial class LayoutView : UserControl {
        private IDisposable sessionStateSubscription;
        private LayoutViewModel viewModel;

        public LayoutView() {
            InitializeComponent();

            DataContext = new LayoutViewModel();

            Subscribe();
        }

        public LayoutViewModel ViewModel {
            get { return viewModel; }
            private set {
                viewModel = value;
                DataContext = value;
            }
        }

        private void Subscribe() {
            sessionStateSubscription = ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Subscribe(newState => { OnSessionStateChanged(newState); });
        }

        private void OnShowLogin(object sender, RoutedEventArgs e) {
            VisualStateManager.GoToState(this, "ShowLogin", true);
        }

        private void OnSessionStateChanged(XmppSessionState newState) {
            Dispatcher.BeginInvoke
                (
                    DispatcherPriority.ApplicationIdle,
                    new ThreadStart
                        (
                        delegate
                            {
                                switch (newState)
                                {
                                    case XmppSessionState.LoggedIn:
                                        VisualStateManager.GoToState(this, "LoggedIn", true);
                                        break;

                                    case XmppSessionState.LoggingIn:
                                        VisualStateManager.GoToState(this, "LoggingIn", false);
                                        break;

                                    case XmppSessionState.LoggingOut:
                                        VisualStateManager.GoToState(this, "LoggingOut", true);
                                        break;

                                    case XmppSessionState.Error:
                                        VisualStateManager.GoToState(this, "Error", true);
                                        break;
                                }
                            }
                        )
                );
        }
    }
}