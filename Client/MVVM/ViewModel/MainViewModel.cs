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
using System.Collections.Specialized;
using System.ComponentModel;
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
        public RelayCommand AssignRoleCommand { get; set; }
        public RelayCommand OpenChatsWindowCommand { get; set; }
        public RelayCommand OpenChatSettingsWindowCommand { get; set; }
        public RelayCommand OpenMemberBanWindowCommand { get; set; }
        public RelayCommand OpenMemberMuteWindowCommand { get; set; }
        public RelayCommand ChangeUsernameCommand { get; set; }

        public ObservableCollection<Chat> Chats { get; set; }
        public ObservableCollection<Chat> UserChats { get; set; }
        public ObservableCollection<Message> ChatMessages { get; set; }
        public ObservableCollection<ChatMemberClientModel> ChatMembers { get; set; }
        public ObservableCollection<ChatRoleClientModel> ChatRoles { get; set; }

        public ChatMemberClientModel ClientMember { get; set; }

        public string Message { get; set; }
        public string Name { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public bool CanLeave
        {
            get => ClientMember?.Role.CanKick ?? false;
		}


        public ChatMemberClientModel SelectedMember
        {
            get
            {
                return selectedMember;
            }
            set
            {
                selectedMember = value;
                OnPropertyChanged(nameof(SelectedMember));
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
                OnPropertyChanged(nameof(SelectedChat));
				if (value != null)
				{
                    ChatRoles.Clear();
					serverConnection.RequestChatMessages(value.Id);
					serverConnection.RequestChatMembers(value.Id);
                    ClientMember = ChatMembers.FirstOrDefault(m => m.Id == ClientInfo.Id);

				}
				ChatMessages.Clear();
			}
		}
		public MainViewModel()
        {
            serverConnection = ServerConnection.GetInstance();

            Chats = new ObservableCollection<Chat>();
            UserChats = new ObservableCollection<Chat>();
            ChatMessages = new ObservableCollection<Message>();
            ChatMembers = new ObservableCollection<ChatMemberClientModel>();
			ChatRoles = new ObservableCollection<ChatRoleClientModel>();

			SendMessageCommand = new RelayCommand(o => SendChatMessage());
            LeaveChatCommand = new RelayCommand(o => serverConnection.RequestChatLeave((int)o));
            KickMemberCommand = new RelayCommand(o => RequestMemberKick());
            OpenChatsWindowCommand = new RelayCommand(o => new ChatsWindow().ShowDialog());
            OpenMemberBanWindowCommand = new RelayCommand(o => OpenBanWindow(), o => SelectedMember != null);
			OpenMemberMuteWindowCommand = new RelayCommand(o => OpenMuteWindow(), o => SelectedMember != null);
            OpenChatSettingsWindowCommand = new RelayCommand(o => OpenChatSettings());
            ChangeUsernameCommand = new RelayCommand(o => RequestUsernameEdit(), o => !String.IsNullOrWhiteSpace(ClientInfo?.Name));

			serverConnection.NewChat += OnNewChat;
            serverConnection.MessageReceived += OnMessageReceived;
            serverConnection.ChatJoinResult += OnChatJoinResult;
            serverConnection.UserChatsResult += OnUserChatsResult;
            serverConnection.ChatMessagesResult += OnChatMessagesResult;
            serverConnection.ChatMembersResult += OnChatMembersResult;
            serverConnection.ChatLeaveResult += OnChatLeaveResult;
            serverConnection.NewChatMember += OnNewChatMember;
            serverConnection.UsernameResult += (sender, args) => ClientInfo = new ClientInfo(args.Id, args.Username);
            serverConnection.ChatMemberKickResult += OnChatMemberKick;
            serverConnection.ChatMemberRemoved += OnChatMemberRemoved;
			serverConnection.ChatMemberActionResponse += (sender, args) => MessageBox.Show(args.Message, "Action result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);
            serverConnection.RoleUpdated += OnRoleUpdate;
            serverConnection.RoleRemoved += OnRoleRemove;
            serverConnection.ChatMemberUpdated += OnChatMemberUpdate;
            serverConnection.ChatRemove += OnChatRemove;
            serverConnection.ChatEdit += OnChatUpdate;

            serverConnection.RequestUserChats();
            RequestUsername();
            
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
		}

		private void OnRoleRemove(object? sender, ChatEventArgs e)
		{
            if(SelectedChat?.Id == e.ChatId)
                serverConnection.RequestChatMembers(SelectedChat.Id);
		}

		private void OnRoleUpdate(object? sender, RoleUpdateEventArgs e)
        {
            if(e.ChatId == SelectedChat.Id)
            {
                serverConnection.RequestChatMembers(e.ChatId);
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




        public void OnNewChat(object sender, NewChatEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Chats.Add(new Chat() { Name = e.Name, Description = e.Description }));
        }
        public void SendChatMessage()
        {
            MessageClientPacket packet = new MessageClientPacket(SelectedChat.Id, Message);
            string jsonPacket = packet.Serialize();
            serverConnection.SendPacket(jsonPacket, PacketType.Message);
        }
    }
}
