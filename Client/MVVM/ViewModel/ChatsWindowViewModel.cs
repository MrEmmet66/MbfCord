using Client.MVVM.Core;
using Client.MVVM.Model;
using Client.Net;
using Infrastructure.C2S;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Net.Event;
using System.Windows;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Model;

namespace Client.MVVM.ViewModel
{
    class ChatsWindowViewModel : BaseViewModel

	{
        public ChatsWindowViewModel()
        {
            serverConnection = ServerConnection.GetInstance();

			Chats = new ObservableCollection<Chat>();
			JoinChatCommand = new RelayCommand(o => RequestChatJoin(), o => SelectedChat != null);
			CreateChatCommand = new RelayCommand(o => new ChatCreateWindow().ShowDialog());

			serverConnection.ChatsResult += OnChatsResult;
			serverConnection.ChatJoinResult += OnChatJoinResult;
			serverConnection.NewChat += OnNewChat;

			RequestChats();
		}
		private ServerConnection serverConnection;
        private Chat selectedChat;


		public ObservableCollection<Chat> Chats { get; set; }
		public RelayCommand JoinChatCommand { get; set; }
		public RelayCommand CreateChatCommand { get; set; }


		public Chat SelectedChat
		{
			get => selectedChat;
			set
			{
				selectedChat = value;
				OnPropertyChanged(nameof(SelectedChat));

			}
		}


		private void OnNewChat(object? sender, NewChatEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				Chat chat = new Chat(e.Name, e.Description);
				chat.Id = e.ChatId;
				
				Chats.Add(chat);
			});
		}


		private void OnChatJoinResult(object? sender, ChatJoinResultEventArgs e)
		{
			if(!e.Status)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OnChatsResult(object? sender, ChatsResultEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				foreach (Chat chat in e.Chats)
				{
					Chats.Add(chat);
				}
			});
		}

		private void RequestChatJoin()
		{
			BaseChatRequestClientPacket packet = new BaseChatRequestClientPacket(PacketType.ChatJoinRequest, SelectedChat.Id);
			serverConnection.SendPacket(packet);
		}

		private void RequestChats()
		{
			BaseClientPacket packet = new BaseClientPacket(PacketType.ChatsRequest);
			serverConnection.SendPacket(packet);
		}


	}
}
