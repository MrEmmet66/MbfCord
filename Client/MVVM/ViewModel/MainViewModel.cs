using Client.MemberActionWindows;
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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Schema;

namespace Client.MVVM.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        ServerConnection serverConnection;
        private Chat selectedChat;
        private Chat selectedUserChat;
        private ChatMemberClientModel selectedMember;
        private ChatRoleClientModel selectedRole;
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand LeaveChatCommand { get; set; }
        public RelayCommand KickMemberCommand { get; set; }
        public RelayCommand UnmuteMemberCommand { get; set; }
        public RelayCommand OpenChatsWindowCommand { get; set; }
        public RelayCommand OpenChatSettingsWindowCommand { get; set; }
        public RelayCommand OpenMemberBanWindowCommand { get; set; }
        public RelayCommand OpenMemberMuteWindowCommand { get; set; }
        public RelayCommand OpenRoleAssignWindowCommand { get; set; }
        public RelayCommand ChangeUsernameCommand { get; set; }
        public ObservableCollection<Chat> UserChats { get; set; }
        public ObservableCollection<Message> ChatMessages { get; set; }
        public ObservableCollection<ChatMemberClientModel> ChatMembers { get; set; }
        public ObservableCollection<ChatRoleClientModel> ChatRoles { get; set; }
        public string EditedUsername { get; set; }

        public ChatMemberClientModel ClientMember { get; set; }

        public string Message { get; set; }
        public string Name { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public bool CanLeave { get; set; }

		public ChatMemberClientModel SelectedMember { get; set; }
		public Chat SelectedChat
		{
			get
			{
				return selectedChat;
			}
			set
			{
				selectedChat = value;
				if (value != null)
				{
                    ChatRoles.Clear();
					RequestChatMessages(value.Id);
					RequestChatMembers(value.Id);

				}
				ChatMessages.Clear();
				OnPropertyChanged(nameof(SelectedChat));
			}
		}
		public MainViewModel()
        {
            serverConnection = ServerConnection.GetInstance();

            UserChats = new ObservableCollection<Chat>();
            ChatMessages = new ObservableCollection<Message>();
            ChatMembers = new ObservableCollection<ChatMemberClientModel>();
			ChatRoles = new ObservableCollection<ChatRoleClientModel>();

			SendMessageCommand = new RelayCommand(o => SendChatMessage(Message));
            LeaveChatCommand = new RelayCommand(o => RequestChatLeave(SelectedChat.Id));
            KickMemberCommand = new RelayCommand(o => RequestMemberKick());
            OpenChatsWindowCommand = new RelayCommand(o => new ChatsWindow().ShowDialog());
            OpenMemberBanWindowCommand = new RelayCommand(o => OpenBanWindow(), o => (SelectedMember != null && ClientMember.Role.CanBan));
			OpenMemberMuteWindowCommand = new RelayCommand(o => OpenMuteWindow(), o => SelectedMember != null);
            OpenChatSettingsWindowCommand = new RelayCommand(o => OpenChatSettings());
            ChangeUsernameCommand = new RelayCommand(o => RequestUsernameEdit(), o => !String.IsNullOrWhiteSpace(ClientInfo?.Name));
            OpenRoleAssignWindowCommand = new RelayCommand(o => OpenAssignRoleWindow(), o => SelectedMember != null);
            UnmuteMemberCommand = new RelayCommand(o => RequestMemberUnmute(), o => SelectedMember != null);

            serverConnection.MessageReceived += OnMessageReceived;
            serverConnection.ChatJoinResult += OnChatJoinResult;
            serverConnection.UserChatsResult += OnUserChatsResult;
            serverConnection.ChatMessagesResult += OnChatMessagesResult;
            serverConnection.ChatMembersResult += OnChatMembersResult;
            serverConnection.ChatLeaveResult += OnChatLeaveResult;
            serverConnection.NewChatMember += OnNewChatMember;
            serverConnection.UsernameResult += OnClientInfoResult;
            serverConnection.ChatMemberKickResult += OnChatMemberKick;
            serverConnection.ChatMemberRemoved += OnChatMemberRemoved;
			serverConnection.ChatMemberActionResponse += (sender, args) => MessageBox.Show(args.Message, "Result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);
            serverConnection.RoleUpdated += OnRoleUpdate;
            serverConnection.RoleRemoved += OnRoleRemove;
            serverConnection.ChatMemberUpdated += OnChatMemberUpdate;
            serverConnection.ChatRemove += OnChatRemove;
            serverConnection.ChatEdit += OnChatUpdate;

            RequestUserChats();
            RequestUsername();
            
        }

		private void OnClientInfoResult(object? sender, ClientInfoResultEventArgs e)
		{
            ClientInfo = new ClientInfo(e.Id, e.Username);
			EditedUsername = e.Username;
			OnPropertyChanged(nameof(EditedUsername));
		}

		private void RequestChatLeave(int chatId)
		{
			BaseChatRequestClientPacket chatLeaveRequest = new BaseChatRequestClientPacket(PacketType.ChatLeaveRequest, chatId);
			serverConnection.SendPacket(chatLeaveRequest);
		}

		private void RequestChatMembers(int chatId)
		{
            BaseChatRequestClientPacket chatMembersRequest = new BaseChatRequestClientPacket(PacketType.ChatMembersRequest, chatId);
			serverConnection.SendPacket(chatMembersRequest);
		}

		private void RequestChatMessages(int id)
		{
			BaseChatRequestClientPacket chatMessagesRequest = new BaseChatRequestClientPacket(PacketType.ChatMessagesRequest, id);
			serverConnection.SendPacket(chatMessagesRequest);
		}


		private void RequestUserChats()
		{
			BaseClientPacket userChatsRequest = new BaseClientPacket(PacketType.UserChatsRequest);
			serverConnection.SendPacket(userChatsRequest);
		}

		private void RequestMemberUnmute()
		{
            BaseChatMemberActionClientPacket unmutePacket = new BaseChatMemberActionClientPacket(PacketType.ChatMemberUnmuteRequest, SelectedChat.Id, SelectedMember.Id);
			serverConnection.SendPacket(unmutePacket);
		}

		private void OpenAssignRoleWindow()
		{
			RoleAssignWindow roleAssignWindow = new RoleAssignWindow();
			roleAssignWindow.DataContext = new RoleAssignViewModel(SelectedChat.Id, SelectedMember);
			((RoleAssignViewModel)roleAssignWindow.DataContext).TargetMember = SelectedMember;
            ((RoleAssignViewModel)roleAssignWindow.DataContext).ChatId = SelectedChat.Id;
			roleAssignWindow.ShowDialog();
		}

		private void RequestUsernameEdit()
		{
            UsernameEditRequestClientPacket usernameEditRequest = new UsernameEditRequestClientPacket(ClientInfo.Name);
			serverConnection.SendPacket(usernameEditRequest);
		}

		private void OnChatUpdate(object? sender, ChatUpdateEventArgs e)
		{
            int index = UserChats.IndexOf(UserChats.FirstOrDefault(c => c.Id == e.Chat.Id));
            Application.Current.Dispatcher.Invoke(() => UserChats[index] = new Chat(e.Chat.Name, e.Chat.Description));
		}

		private void OnChatRemove(object? sender, ChatEventArgs e)
		{
            Application.Current.Dispatcher.Invoke(() => UserChats.Remove(UserChats.FirstOrDefault(c => c.Id == e.ChatId)));

		}

		private void OnChatMemberUpdate(object? sender, ChatMemberUpdateEventArgs e)
		{
            if(e.ChatId == SelectedChat?.Id)
            {
                int index = ChatMembers.IndexOf(ChatMembers.FirstOrDefault(m => m.Id == e.Member.Id));
                Application.Current.Dispatcher.Invoke(() => ChatMembers[index] = e.Member);
			}
            if(e.Member.Id == ClientInfo.Id)
            {
				ClientMember = e.Member;
				CanLeave = ClientMember.Role.IsOwner != true;
                EditedUsername = ClientMember.Username;
				OnPropertyChanged(nameof(ClientMember));
				OnPropertyChanged(nameof(CanLeave));
				OnPropertyChanged(nameof(EditedUsername));


			}
		}

		private void OnRoleRemove(object? sender, ChatEventArgs e)
		{
            if(SelectedChat?.Id == e.ChatId)
                RequestChatMembers(SelectedChat.Id);
		}

		private void OnRoleUpdate(object? sender, RoleUpdateEventArgs e)
        {
            if(e.ChatId == SelectedChat.Id)
            {
                RequestChatMembers(e.ChatId);
            }
        }

		private void OpenChatSettings()
		{
			ChatSettingsWindow chatSettingsWindow = new ChatSettingsWindow();
            chatSettingsWindow.DataContext = new ChatSettingsViewModel(SelectedChat);
			((ChatSettingsViewModel)chatSettingsWindow.DataContext).TargetChat = SelectedChat;
			chatSettingsWindow.ShowDialog();
		}

		private void OpenMuteWindow()
        {
            MemberMuteWindow memberMuteWindow = new MemberMuteWindow();
			memberMuteWindow.DataContext = new MuteMemberWindowViewModel();
			((MuteMemberWindowViewModel)memberMuteWindow.DataContext).RestrictionDate = DateTime.Now.AddDays(1);
			((MuteMemberWindowViewModel)memberMuteWindow.DataContext).ChatId = SelectedChat.Id;
			((MuteMemberWindowViewModel)memberMuteWindow.DataContext).TargetMember = SelectedMember;
            memberMuteWindow.ShowDialog();
		}

        private void OpenBanWindow()
        {
            MemberBanWindow memberBanWindow = new MemberBanWindow();
			memberBanWindow.DataContext = new MemberBanWindowViewModel(SelectedMember, SelectedChat.Id);
            ((MemberBanWindowViewModel)memberBanWindow.DataContext).RestrictionDate = DateTime.Now.AddDays(1);
            ((MemberBanWindowViewModel)memberBanWindow.DataContext).ChatId = SelectedChat.Id;
			((MemberBanWindowViewModel)memberBanWindow.DataContext).TargetMember = SelectedMember;

			memberBanWindow.ShowDialog();
		}

		private void OnChatMemberRemoved(object? sender, ChatMemberRemoveEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
                Chat chat = UserChats.FirstOrDefault(c => c.Id == e.ChatId);
				if (e.MemberId == ClientInfo.Id)
                {
					if (SelectedChat.Id == e.ChatId)
						ChatMembers.Clear();
					UserChats.Remove(chat);
                }
                if(SelectedChat != null && SelectedChat.Id == e.ChatId)
                {
					ChatMembers.Remove(ChatMembers.FirstOrDefault(m => m.Id == e.MemberId));
				}
			});
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

		private void RequestMemberKick()
		{
            ChatMemberKickRequestClientPacket kickRequest = new ChatMemberKickRequestClientPacket(SelectedChat.Id, SelectedMember.Id);
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
				ClientMember = ChatMembers.FirstOrDefault(m => m.Id == ClientInfo.Id);
                OnPropertyChanged(nameof(ClientMember));
                CanLeave = ClientMember.Role.IsOwner != true;
                OnPropertyChanged(nameof(CanLeave));
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
                    Chat chat = new Chat(e.ChatModel.Name, e.ChatModel.Description);
                    chat.Id = e.ChatModel.Id;
					UserChats.Add(chat);
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

        public void SendChatMessage(string content)
        {
            MessageClientPacket packet = new MessageClientPacket(SelectedChat.Id, content);
            string jsonPacket = packet.Serialize();
            serverConnection.SendPacket(jsonPacket, PacketType.Message);
        }
    }
}
