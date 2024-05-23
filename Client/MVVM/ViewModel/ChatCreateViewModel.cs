using Client.MVVM.Core;
using Client.Net;
using Infrastructure.C2S.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.MVVM.ViewModel
{
	internal class ChatCreateViewModel : BaseViewModel
	{
		private ServerConnection serverConnection;
		public RelayCommand CreateChatCommand { get; set; }
		public string ChatName { get; set; }
		public string? ChatDescription { get; set; }

		public ChatCreateViewModel()
		{
			serverConnection = ServerConnection.GetInstance();
			CreateChatCommand = new RelayCommand(o => RequestChatCreate(), o => !string.IsNullOrWhiteSpace(ChatName));
		}

		private void RequestChatCreate()
		{
			ChatCreateClientPacket chatCreateClientPacket = new ChatCreateClientPacket(ChatName, ChatDescription);
			serverConnection.SendPacket(chatCreateClientPacket);
			Application.Current.Dispatcher.Invoke(() =>
			{
				foreach (Window window in Application.Current.Windows)
				{
					if (window is ChatCreateWindow)
					{
						window.Close();
					}
				}
			});
		}
	}
}
