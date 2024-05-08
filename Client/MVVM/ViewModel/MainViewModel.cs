using Client.MVVM.Core;
using Client.MVVM.Model;
using Client.Net;
using Client.Net.Event;
using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Chat;
using Infrastructure.C2S.MemberAction;
using Infrastructure.C2S.Message;
using Infrastructure.C2S.Role;
using Infrastructure.S2C.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Schema;

namespace Client.MVVM.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        ServerConnection serverConnection;
        private Chat selectedChat;
        private Chat selectedUserChat;
        private ChatMemberClientModel selectedMember;
        private ChatRoleClientModel selectedRole;
        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand CreateChatCommand { get; set; }
        public RelayCommand JoinChatCommand { get; set; }
        public RelayCommand LeaveChatCommand { get; set; }
        public RelayCommand KickMemberCommand { get; set; }
        public RelayCommand MuteMemberCommand { get; set; }
        public RelayCommand BanMemberCommand { get; set; }
        public RelayCommand AddRoleCommand { get; set; }
        public RelayCommand EditRoleCommand { get; set; }
        public RelayCommand AssignRoleCommand { get; set; }

		public RelayCommand RemoveRoleCommand { get; set; }
        public ObservableCollection<Chat> Chats { get; set; }
        public ObservableCollection<Chat> UserChats { get; set; }
        public ObservableCollection<Message> ChatMessages { get; set; }
        public ObservableCollection<ChatMemberClientModel> ChatMembers { get; set; }
        public ObservableCollection<ChatRoleClientModel> ChatRoles { get; set; }

        public ChatMemberClientModel ClientMember { get; set; }

        public string Message { get; set; }
        public string Name { get; set; }
        public string ChatName { get; set; }
        public string ChatDesc { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public DateTime RestrictionDate { get; set; }
        public ChatRoleClientModel CreatedRole { get; set; }

        public ChatMemberClientModel SelectedMember
        {
            get
            {
                return selectedMember;
            }
            set
            {
                selectedMember = value;
                if(value != null)
                {
					var role = ChatRoles.FirstOrDefault(r => r.Id == value.Role.Id);
				}
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedMember"));
            }
        }
		public Chat SelectedChat
		{
			get
			{
				return selectedChat;
			}
			set
			{
				selectedChat = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedChat"));
				if (value != null)
				{
                    ChatRoles.Clear();
					serverConnection.RequestChatMessages(value.Id);
					serverConnection.RequestChatMembers(value.Id);
                    RequestRoles(value.Id);
                    ClientMember = ChatMembers.FirstOrDefault(m => m.Username == ClientInfo.Name);

				}
				ChatMessages.Clear();
			}
		}

        public ChatRoleClientModel SelectedRole
        {
            get
            {
                return selectedRole;
            }
            set
            {
                selectedRole = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRole"));

			}
        }
		public MainViewModel()
        {
            serverConnection = ServerConnection.GetInstance();
            CreatedRole = new ChatRoleClientModel();

            Chats = new ObservableCollection<Chat>();
            UserChats = new ObservableCollection<Chat>();
            ChatMessages = new ObservableCollection<Message>();
            ChatMembers = new ObservableCollection<ChatMemberClientModel>();
			ChatRoles = new ObservableCollection<ChatRoleClientModel>();

			SendMessageCommand = new RelayCommand(o => SendChatMessage());
			CreateChatCommand = new RelayCommand(o => RequestChat());
            JoinChatCommand = new RelayCommand(o => JoinChat((int)o));
            LeaveChatCommand = new RelayCommand(o => serverConnection.RequestChatLeave((int)o));
            KickMemberCommand = new RelayCommand(o => RequestMemberKick((int)o));
            MuteMemberCommand = new RelayCommand(o => RequestMemberMute(), o => RestrictionDate != DateTime.MinValue);
            BanMemberCommand = new RelayCommand(o => RequestMemberBan(), o => RestrictionDate != DateTime.MinValue);
			AddRoleCommand = new RelayCommand(o => RequestAddRole(), o => SelectedChat != null);
            EditRoleCommand = new RelayCommand(o => RequestEditRole(), o => SelectedRole != null);
			RemoveRoleCommand = new RelayCommand(o => RequestRemoveRole(), o => SelectedRole != null);
            AssignRoleCommand = new RelayCommand(o => RequestAssignRole(), o => (SelectedRole != null && SelectedMember != null));

			serverConnection.NewChat += OnNewChat;
            serverConnection.ChatsResult += OnChatsResult;
            serverConnection.MessageReceived += OnMessageReceived;
            serverConnection.ChatJoinResult += OnChatJoinResult;
            serverConnection.UserChatsResult += OnUserChatsResult;
            serverConnection.ChatMessagesResult += OnChatMessagesResult;
            serverConnection.ChatMembersResult += OnChatMembersResult;
            serverConnection.ChatLeaveResult += OnChatLeaveResult;
            serverConnection.ChatRolesResult += OnChatRolesResult;
            serverConnection.NewChatMember += OnNewChatMember;
            serverConnection.UsernameResult += (sender, args) => ClientInfo = new ClientInfo(args.Id, args.Username);
            serverConnection.ChatMemberKickResult += OnChatMemberKick;
            serverConnection.ChatMemberRemoved += OnChatMemberRemoved;
			serverConnection.ChatMemberActionResponse += (sender, args) => MessageBox.Show(args.Message, "Action result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);
            serverConnection.RoleAddResponse += (sender, args) => MessageBox.Show(args.Message, "Role create result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);
			serverConnection.RoleEditResponse += (sender, args) => MessageBox.Show(args.Message, "Role edit result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);
            serverConnection.RoleRemoveResponse += (sender, args) => MessageBox.Show(args.Message, "Role remove result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);

			serverConnection.RequestChats();
            serverConnection.RequestUserChats();
            RequestUsername();
            
        }

		private void RequestAssignRole()
		{
			RoleAssignRequestClientPacket roleAssignRequest = new RoleAssignRequestClientPacket(SelectedRole.Id, SelectedMember.Id);
			serverConnection.SendPacket(roleAssignRequest);
		}

		private void RequestRemoveRole()
		{
			RemoveRoleRequestClientPacket roleRemoveRequest = new RemoveRoleRequestClientPacket(SelectedRole.Id);
			serverConnection.SendPacket(roleRemoveRequest);
		}

		private void RequestEditRole()
		{
			EditRoleRequestClientPacket roleEditRequest = new EditRoleRequestClientPacket(SelectedRole);
			serverConnection.SendPacket(roleEditRequest);
		}

		private void RequestAddRole()
		{
			AddRoleRequestClientPacket roleAddRequest = new AddRoleRequestClientPacket(CreatedRole, SelectedChat.Id);
			serverConnection.SendPacket(roleAddRequest);
		}

		private void RequestMemberBan()
		{
			ChatMemberBanRequestClientPacket banRequest = new ChatMemberBanRequestClientPacket(SelectedChat.Id, SelectedMember.Id, RestrictionDate);
			serverConnection.SendPacket(banRequest);
		}

		private void OnChatMemberRemoved(object? sender, ChatMemberRemoveEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
                Chat chat = UserChats.FirstOrDefault(c => c.Id == e.ChatId);
				if (e.MemberId == ClientInfo.Id)
                {
                    UserChats.Remove(chat);
                }
                if(SelectedChat != null && SelectedChat.Id == e.ChatId)
                {
					ChatMembers.Remove(ChatMembers.FirstOrDefault(m => m.Id == e.MemberId));
				}
			});
		}

		private void RequestMemberMute()
		{
			ChatMemberMuteRequestClientPacket muteRequest = new ChatMemberMuteRequestClientPacket(SelectedChat.Id, SelectedMember.Id, RestrictionDate);
			serverConnection.SendPacket(muteRequest);
		}

		private void OnNewChatMember(object? sender, NewMemberEventArgs e)
		{
            Application.Current.Dispatcher.Invoke(() =>
            {
                if(SelectedChat != null && e.ChatId == SelectedChat.Id)
                {
					ChatMembers.Add(e.ClientModel);
				}
            });
		}

		private void RequestMemberKick(int o)
		{
            ChatMemberKickRequestClientPacket kickRequest = new ChatMemberKickRequestClientPacket(SelectedChat.Id, o);
			serverConnection.SendPacket(kickRequest);
		}

		private void OnChatMemberKick(object? sender, ChatActionEventArgs e)
		{
            if(!e.Status)
            {
                MessageBox.Show(e.Message, "Kick error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}

		private void RequestUsername()
        {
            BaseClientPacket usernameRequest = new BaseClientPacket(PacketType.ClientInfoRequest);
			serverConnection.SendPacket(usernameRequest);

		}


		private void OnChatRolesResult(object? sender, ChatRolesResultEventArgs e)
		{
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach(ChatRoleClientModel role in e.Roles)
                {
					ChatRoles.Add(role);
				}
            });
		}

		private void RequestRoles(int chatId)
        {
            BaseChatRequestClientPacket rolesRequest = new BaseChatRequestClientPacket(PacketType.ChatRolesRequest, chatId);
            serverConnection.SendPacket(rolesRequest);

        }

		private void OnChatLeaveResult(object? sender, ChatJoinResultEventArgs e)
		{
            if(!e.Status)
            {
				MessageBox.Show(e.Message, "Leave error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OnChatMembersResult(object? sender, ChatMembersEventArgs e)
		{
            Application.Current.Dispatcher.Invoke(() =>
            {
				ChatMembers.Clear();
				foreach (ChatMemberClientModel member in e.Members)
                {
					ChatMembers.Add(member);
				}
			});
		}

		private void OnChatMessagesResult(object? sender, ChatMessagesEventArgs e)
        {
            foreach(ChatMessageClientModel msg in e.MessageData)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChatMessages.Add(new Message(msg));
                });
            }
        }

        private void OnUserChatsResult(object? sender, UserChatsResultEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Chat chat in e.Chats)
                {
                    UserChats.Add(chat);
                }
            });
        }

        private void OnChatJoinResult(object? sender, ChatJoinResultEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if(e.Status)
                {
					UserChats.Add(Chats.FirstOrDefault(c => c.Id == e.ChatId));
				}
				else
				{
					MessageBox.Show(e.Message, "Join error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			});
        }

        private void OnMessageReceived(object? sender, MessageEventArgs e)
        {
            Message message = e.Message;
            if (SelectedChat == null)
                return;
            if(e.Status)
            {
				if (message.ChatId == SelectedChat.Id)
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						ChatMessages.Add(message);
					});
				}
			}
            else
            {
				MessageBox.Show(e.ErrorMessage, "Message error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }




        public void OnNewChat(object sender, NewChatEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Chats.Add(new Chat() { Name = e.Name, Description = e.Description }));
        }

        public void JoinChat(int chatId)
        {
            BaseChatRequestClientPacket packet = new BaseChatRequestClientPacket(PacketType.ChatJoinRequest, chatId);

            serverConnection.SendPacket(packet.Serialize(), PacketType.ChatJoinRequest);
        }

        public void OnChatsResult(object sender, ChatsResultEventArgs e)
        {
            List<Chat> chats = JsonConvert.DeserializeObject<List<Chat>>(e.Data);
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Chat chat in chats)
                {
                    Chats.Add(chat);
                }
            });
        
        }



        private void RequestChat()
        {
            ChatCreateClientPacket packet = new ChatCreateClientPacket(ChatName, ChatDesc);
            serverConnection.SendPacket(packet);
        }

        public void SendChatMessage()
        {
            MessageClientPacket packet = new MessageClientPacket(SelectedChat.Id, Message);
            string jsonPacket = packet.Serialize();
            serverConnection.SendPacket(jsonPacket, PacketType.Message);
        }
    }
}
