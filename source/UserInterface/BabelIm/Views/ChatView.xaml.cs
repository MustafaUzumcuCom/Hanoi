using System.Windows.Controls;
using BabelIm.Infrastructure;
using BabelIm.Net.Xmpp.Core;
using BabelIm.ViewModels;

namespace BabelIm.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView 
        : UserControl
    {
        #region · Constructors ·

        public ChatView()
        {
            InitializeComponent();
        }

        #endregion

        #region · Event Handlers ·

        private void Message_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ChatViewModel vm = this.DataContext as ChatViewModel;

            if (e.Key == System.Windows.Input.Key.Return)
            {
                e.Handled = true;
                vm.SendMessage(this.Message.GetText());
                this.Message.Document.Blocks.Clear();
            }
            else
            {
                vm.SendChatStateNotification(XmppChatStateNotification.Active);
            }
        }

        #endregion
    }
}
