using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using BabelIm.Infrastructure;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.InstantMessaging;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;
using BabelIm.ViewModels;

namespace BabelIm
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
        : Window
    {
        #region · Constructors ·

        // http://weblogs.asp.net/jdanforth/archive/2006/06/21/454219.aspx
        public Shell()
        {
            InitializeComponent();

            this.DataContext = new ShellViewModel();

            ServiceFactory.Current.Resolve<IXmppSession>()
                .MessageReceived
                .Where(m => m.Type == MessageType.Chat)
                .Subscribe(message => { OnChatMessageReceived(message); });
        }
        
        #endregion
        
        #region · Event Handlers ·

        private void WindowHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                this.DragMove();
            }
            else if (e.ClickCount > 1)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.ResizeMode = ResizeMode.CanResize;
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.ResizeMode = ResizeMode.NoResize;
                    this.WindowState = WindowState.Maximized;
                }
            }

            e.Handled = true;
        }
        
        private void ShellWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.Space && e.Key == Key.System)
            {
                // Disable Window's ControlBox Menu
                e.Handled = true;
            }
        }

        private void OnChatMessageReceived(XmppMessage message)
        {
            this.Dispatcher.BeginInvoke
            (
                DispatcherPriority.Normal,
                new ThreadStart
                (
                    delegate
                    {
                        WindowInteropHelper helper = new WindowInteropHelper(this);

                        if (!this.IsVisible)
                        {
                            this.ShowActivated  = false;
                            this.Visibility     = Visibility.Visible;
                        }

                        if (!this.IsActive ||
                            this.WindowState == WindowState.Minimized)
                        {
                            Win32NativeMethods.FlashWindow(helper.Handle);
                        }
                    }
                )
            );
        }

        #endregion
    }
}