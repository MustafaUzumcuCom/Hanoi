using System.Windows;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.InstantMessaging;

namespace BabelIm.Contracts {
    public interface IChatViewManager {
        void CloseChatView(string jid);
        void OpenChatView(XmppJid jid);
        void OpenChatView(string jid);
        void OpenChatView(XmppChat chat, bool activate);
        void RegisterContainer(DependencyObject d);
    }
}