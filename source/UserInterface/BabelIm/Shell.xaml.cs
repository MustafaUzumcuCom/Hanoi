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

namespace BabelIm {
    /// <summary>
    ///   Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
        : Window {
        // http://weblogs.asp.net/jdanforth/archive/2006/06/21/454219.aspx
        public Shell() {
            InitializeComponent();

            DataContext = new ShellViewModel();

            ServiceFactory.Current.Resolve<IXmppSession>()
                .MessageReceived
                .Where(m => m.Type == MessageType.Chat)
                .Subscribe(message => { OnChatMessageReceived(message); });
        }

        private void WindowHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 1)
            {
                DragMove();
            }
            else if (e.ClickCount > 1)
            {
                if (WindowState == WindowState.Maximized)
                {
                    ResizeMode = ResizeMode.CanResize;
                    WindowState = WindowState.Normal;
                }
                else
                {
                    ResizeMode = ResizeMode.NoResize;
                    WindowState = WindowState.Maximized;
                }
            }

            e.Handled = true;
        }

        private void ShellWindow_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.SystemKey == Key.Space && e.Key == Key.System)
            {
                // Disable Window's ControlBox Menu
                e.Handled = true;
            }
        }

        private void OnChatMessageReceived(XmppMessage message) {
            Dispatcher.BeginInvoke
                (
                    DispatcherPriority.Normal,
                    new ThreadStart
                        (
                        delegate
                            {
                                var helper = new WindowInteropHelper(this);

                                if (!IsVisible)
                                {
                                    ShowActivated = false;
                                    Visibility = Visibility.Visible;
                                }

                                if (!IsActive ||
                                    WindowState == WindowState.Minimized)
                                {
                                    Win32NativeMethods.FlashWindow(helper.Handle);
                                }
                            }
                        )
                );
        }
        }
}