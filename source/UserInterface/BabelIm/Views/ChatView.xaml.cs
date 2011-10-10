using System.Windows.Controls;
using BabelIm.Infrastructure;
using BabelIm.Net.Xmpp.Core;
using BabelIm.ViewModels;

namespace BabelIm.Views {
    /// <summary>
    ///   Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView
        : UserControl {
        public ChatView() {
            InitializeComponent();
        }

        private void Message_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            var vm = DataContext as ChatViewModel;

            if (e.Key == System.Windows.Input.Key.Return)
            {
                e.Handled = true;
                vm.SendMessage(Message.GetText());
                Message.Document.Blocks.Clear();
            }
            else
            {
                vm.SendChatStateNotification(XmppChatStateNotification.Active);
            }
        }
        }
}