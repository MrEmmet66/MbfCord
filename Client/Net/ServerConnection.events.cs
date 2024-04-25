using Client.Net.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net
{
    partial class ServerConnection
    {
		public event EventHandler<AuthEventArgs> LoginResult;
		public event EventHandler<AuthEventArgs> RegisterResult;
		public event EventHandler<NewChatEventArgs> NewChat;
		public event EventHandler<ChatsResultEventArgs> ChatsResult;
		public event EventHandler<MessageEventArgs> MessageReceived;
		public event EventHandler<ChatJoinResultEventArgs> ChatJoinResult;
		public event EventHandler<UserChatsResultEventArgs> UserChatsResult;
		public event EventHandler<ChatMessagesEventArgs> ChatMessagesResult;
		public event EventHandler<ChatMembersEventArgs> ChatMembersResult;
		public event EventHandler<ChatJoinResultEventArgs> ChatLeaveResult;
	}
}
