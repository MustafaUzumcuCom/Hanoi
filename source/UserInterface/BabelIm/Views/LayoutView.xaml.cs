using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.InstantMessaging;
using BabelIm.ViewModels;

namespace BabelIm.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class LayoutView : UserControl
    {
        #region · Fields ·

        private LayoutViewModel viewModel;

        #region · Subscriptions ·

        private IDisposable sessionStateSubscription;

        #endregion

        #endregion

        #region · Properties ·

        public LayoutViewModel ViewModel
        {
            get { return this.viewModel; }
            private set
            {
                this.viewModel = value;
                this.DataContext = value;
            }
        }

        #endregion

        #region · Constructors ·

        public LayoutView()
        {
            InitializeComponent();

            this.DataContext = new LayoutViewModel();

            this.Subscribe();
        }

        #endregion

        #region · Private Methods ·

        private void Subscribe()
        {
            this.sessionStateSubscription = ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Subscribe(newState => { this.OnSessionStateChanged(newState); });
        }

        #endregion

        #region · Event Handlers ·

        private void OnShowLogin(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "ShowLogin", true);
        }

        private void OnSessionStateChanged(XmppSessionState newState)
        {
            this.Dispatcher.BeginInvoke
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

        #endregion
    }
}
