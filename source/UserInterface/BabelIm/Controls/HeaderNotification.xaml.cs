using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BabelIm.Controls {
    /// <summary>
    ///   Based on a control from nRoute (http://nroute.codeplex.com) samples
    /// </summary>
    [TemplateVisualState(Name = HIDDEN_STATENAME, GroupName = NOTIFICATION_STATEGROUP)]
    [TemplateVisualState(Name = VISIBLE_STATENAME, GroupName = NOTIFICATION_STATEGROUP)]
    public partial class HeaderNotification
        : UserControl {
        private const string NOTIFICATION_STATEGROUP = "NotificationStateGroup";
        private const string HIDDEN_STATENAME = "HiddenState";
        private const string VISIBLE_STATENAME = "VisibleState";
        private static readonly TimeSpan TRANSITION_TIMEOUT = TimeSpan.FromMilliseconds(600);

        /// <summary>
        ///   Identifies the MessageText dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof (String), typeof (HeaderNotification),
                                        new FrameworkPropertyMetadata(String.Empty,
                                                                      FrameworkPropertyMetadataOptions.None,
                                                                      OnMessageTextChanged));

        private readonly Queue<InteractiveMessage> messages;
        private readonly DispatcherTimer stateTimer;
        private readonly Object syncObject = new Object();
        private InteractiveMessage currentMessage;

        public HeaderNotification() {
            InitializeComponent();

            // set up
            messages = new Queue<InteractiveMessage>();
            stateTimer = new DispatcherTimer();
            stateTimer.Interval = TRANSITION_TIMEOUT;
            stateTimer.Tick += StateTimer_Tick;
        }

        public string MessageText {
            get { return (String) base.GetValue(HeaderNotification.MessageTextProperty); }
            set { base.SetValue(HeaderNotification.MessageTextProperty, value); }
        }

        private static void OnMessageTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d != null)
            {
                if (e.NewValue != null)
                {
                    var element = d as HeaderNotification;

                    element.ShowNotification(e.NewValue as String);
                }
            }
        }

        public void ShowNotification(string notification) {
            // basic checks
            if (String.IsNullOrWhiteSpace(notification))
            {
                throw new ArgumentNullException("notification");
            }

            // show or enque message
            lock (syncObject)
            {
                // if no items are queued then show the message, else enque
                var message = new InteractiveMessage {Message = notification};

                if (messages.Count == 0 && currentMessage == null)
                {
                    ShowMessage(message);
                }
                else
                {
                    messages.Enqueue(message);
                }
            }
        }

        private void StateTimer_Tick(object sender, EventArgs e) {
            stateTimer.Stop();
            ProcessQueue();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            // basic check
            if (stateTimer.IsEnabled)
            {
                return;
            }

            // we stop the timers and start transitioning
            VisualStateManager.GoToState(this, HIDDEN_STATENAME, true);

            // and transition
            stateTimer.Start();
        }

        private void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            // basic check
            if (stateTimer.IsEnabled)
            {
                return;
            }

            // we stop the timers and start transitioning
            VisualStateManager.GoToState(this, HIDDEN_STATENAME, true);

            // and transition
            stateTimer.Start();
        }

        private void ProcessQueue() {
            lock (syncObject)
            {
                if (messages.Count == 0)
                {
                    currentMessage = null;
                }
                else
                {
                    ShowMessage(messages.Dequeue());
                }
            }
        }

        private void ShowMessage(InteractiveMessage message) {
            HeaderText.Text = message.Message;
            VisualStateManager.GoToState(this, VISIBLE_STATENAME, true);
            currentMessage = message;
        }

        #region Nested type: InteractiveMessage

        private class InteractiveMessage {
            public string Message { get; set; }
        }

        #endregion
        }
}