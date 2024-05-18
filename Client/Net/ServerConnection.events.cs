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
		public event EventHandler<ChatRolesResultEventArgs> ChatRolesResult;
		public event EventHandler<ClientInfoResultEventArgs> UsernameResult;
		public event EventHandler<ChatActionEventArgs> ChatMemberKickResult;
		public event EventHandler<NewMemberEventArgs> NewChatMember;
		public event EventHandler<ChatMemberRemoveEventArgs> ChatMemberRemoved;
		public event EventHandler<ChatActionEventArgs> ChatMemberActionResponse;
		public event EventHandler<ServerResponseEventArgs> RoleAddResponse;
		public event EventHandler<ServerResponseEventArgs> RoleEditResponse;
		public event EventHandler<ServerResponseEventArgs> RoleRemoveResponse;
		public event EventHandler<ServerResponseEventArgs> RoleAssignResponse;
		public event EventHandler<RoleUpdateEventArgs> RoleUpdated;
		public event EventHandler<ChatEventArgs> RoleRemoved;
		public event EventHandler<ChatMemberUpdateEventArgs> ChatMemberUpdated;
		public event EventHandler<ChatEventArgs> ChatRemove;
		public event EventHandler<ChatUpdateEventArgs> ChatEdit;



	}
}
