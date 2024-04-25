using Client.MVVM.Core;
using Client.MVVM.Model;
using Client.Net;
using Client.Net.Event;
using Client.Net.IO;
using Client.Net.IO.Packet.Chat;
using Infrastructure;
using Infrastructure.C2S.Chat;
using Infrastructure.C2S.Message;
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
        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand CreateChatCommand { get; set; }
        public RelayCommand JoinChatCommand { get; set; }
        public RelayCommand LeaveChatCommand { get; set; }
        public ObservableCollection<Chat> Chats { get; set; }
        public ObservableCollection<Chat> UserChats { get; set; }
        public ObservableCollection<Message> ChatMessages { get; set; }
        public ObservableCollection<ChatMember> ChatMembers { get; set; }
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
                if(value != null)
                {
					serverConnection.RequestChatMessages(value.Id);
					serverConnection.RequestChatMembers(value.Id);
				}
                ChatMessages.Clear();
            }
        }
        public Chat SelectedUserChat
        {
            get
            {
                return selectedUserChat;
            }
            set
            {
                selectedUserChat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedUserChat"));
                

            }
        }

        public string Message { get; set; }

        public string Name { get; set; }

        public string ChatName { get; set; }
        public string ChatDesc { get; set;  }
        public MainViewModel()
        {
            serverConnection = ServerConnection.GetInstance();
            SendMessageCommand = new RelayCommand(o => SendChatMessage());
            Chats = new ObservableCollection<Chat>();
            UserChats = new ObservableCollection<Chat>();
            ChatMessages = new ObservableCollection<Message>();
            ChatMembers = new ObservableCollection<ChatMember>();
            CreateChatCommand = new RelayCommand(o => RequestChat());
            JoinChatCommand = new RelayCommand(o => JoinChat((int)o));
            LeaveChatCommand = new RelayCommand(o => serverConnection.RequestChatLeave((int)o));
            serverConnection.NewChat += OnNewChat;
            serverConnection.ChatsResult += OnChatsResult;
            serverConnection.MessageReceived += OnMessageReceived;
            serverConnection.ChatJoinResult += OnChatJoinResult;
            serverConnection.UserChatsResult += OnUserChatsResult;
            serverConnection.ChatMessagesResult += OnChatMessagesResult;
            serverConnection.ChatMembersResult += OnChatMembersResult;
            serverConnection.ChatLeaveResult += OnChatLeaveResult;

            serverConnection.RequestChats();
            serverConnection.RequestUserChats();
        }

		private void OnChatLeaveResult(object? sender, ChatJoinResultEventArgs e)
		{
            Application.Current.Dispatcher.Invoke(() =>
            {
				if (e.Status)
                {
					UserChats.Remove(UserChats.FirstOrDefault(c => c.Id == e.ChatId));
                    MessageBox.Show(e.ChatId.ToString());
                    SelectedChat = null;
				}
			});
		}

		private void OnChatMembersResult(object? sender, ChatMembersEventArgs e)
		{
            Application.Current.Dispatcher.Invoke(() =>
            {
				ChatMembers.Clear();
				foreach (ChatMemberClientModel member in e.Members)
                {
					ChatMembers.Add(new ChatMember(member));
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
                    UserChats.Add(Chats.FirstOrDefault(c => c.Id == e.ChatId));
            });
        }

        private void OnMessageReceived(object? sender, MessageEventArgs e)
        {
            Message message = e.Message;
            if(message.ChatId == SelectedChat.Id)
            {
				Application.Current.Dispatcher.Invoke(() =>
				{
					ChatMessages.Add(message);
				});
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
            string jsonPacket = packet.Serialize();
            serverConnection.SendPacket(jsonPacket, PacketType.ChatCreate);
        }

        public void SendChatMessage()
        {
            MessageClientPacket packet = new MessageClientPacket(SelectedChat.Id, Message);
            string jsonPacket = packet.Serialize();
            serverConnection.SendPacket(jsonPacket, PacketType.Message);
        }
    }
}
