using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BabelIm.Controls
{
    /// <summary>
    /// Based on a control from nRoute (http://nroute.codeplex.com) samples
    /// </summary>
    [TemplateVisualState(Name = HIDDEN_STATENAME, GroupName = NOTIFICATION_STATEGROUP)]
    [TemplateVisualState(Name = VISIBLE_STATENAME, GroupName = NOTIFICATION_STATEGROUP)]
    public partial class HeaderNotification
        : UserControl
    {
        #region · Constants ·
        
        private const string                NOTIFICATION_STATEGROUP = "NotificationStateGroup";
        private const string                HIDDEN_STATENAME        = "HiddenState";
        private const string                VISIBLE_STATENAME       = "VisibleState";
        private readonly static TimeSpan    TRANSITION_TIMEOUT      = TimeSpan.FromMilliseconds(600);

        #endregion

        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the MessageText dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(String), typeof(HeaderNotification),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnMessageTextChanged)));

        #endregion

        #region · Dependency Properties Callback Handlers ·

        private static void OnMessageTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                if (e.NewValue != null)
                {
                    HeaderNotification element = d as HeaderNotification;

                    element.ShowNotification(e.NewValue as String);
                }
            }
        }

        #endregion

        #region · Fields ·

        private readonly Object             syncObject = new Object();
        private Queue<InteractiveMessage>   messages;
        private InteractiveMessage          currentMessage;
        private DispatcherTimer             stateTimer;

        #endregion

        #region · Properties ·

        public string MessageText
        {
            get { return (String)base.GetValue(HeaderNotification.MessageTextProperty); }
            set { base.SetValue(HeaderNotification.MessageTextProperty, value); }
        }

        #endregion

        #region · Constructors ·

        public HeaderNotification()
        {
            InitializeComponent();

            // set up
            this.messages               = new Queue<InteractiveMessage>();
            this.stateTimer             = new DispatcherTimer();
            this.stateTimer.Interval    = TRANSITION_TIMEOUT;
            this.stateTimer.Tick        += new EventHandler(StateTimer_Tick);
        }

        #endregion

        #region · Methods ·

        public void ShowNotification(string notification)
        {
            // basic checks
            if (String.IsNullOrWhiteSpace(notification))
            {
                throw new ArgumentNullException("notification");
            }

            // show or enque message
            lock (syncObject)
            {
                // if no items are queued then show the message, else enque
                var message = new InteractiveMessage() { Message = notification };

                if (this.messages.Count == 0 && this.currentMessage == null)
                {
                    ShowMessage(message);
                }
                else
                {
                    messages.Enqueue(message);
                }
            }
        }

        #endregion

        #region · Event Handlers ·

        private void StateTimer_Tick(object sender, EventArgs e)
        {
            this.stateTimer.Stop();
            this.ProcessQueue();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // basic check
            if (stateTimer.IsEnabled)
            {
                return;
            }
            
            // we stop the timers and start transitioning
            VisualStateManager.GoToState(this, HIDDEN_STATENAME, true);

            // and transition
            this.stateTimer.Start();
        }

        private void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // basic check
            if (this.stateTimer.IsEnabled)
            {
                return;
            }

            // we stop the timers and start transitioning
            VisualStateManager.GoToState(this, HIDDEN_STATENAME, true);

            // and transition
            this.stateTimer.Start();
        }

        #endregion

        #region · Helpers ·

        private void ProcessQueue()
        {
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

        private void ShowMessage(InteractiveMessage message)
        {
            this.HeaderText.Text = message.Message;
            VisualStateManager.GoToState(this, VISIBLE_STATENAME, true);
            this.currentMessage = message;
        }

        #endregion

        #region · Internal Class ·

        class InteractiveMessage
        {
            #region · Properties ·
            
            public string Message
            {
                get;
                set;
            }

            #endregion
        }

        #endregion
    }
}
