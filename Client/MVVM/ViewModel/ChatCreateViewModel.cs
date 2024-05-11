using Client.MVVM.Core;
using Client.Net;
using Infrastructure.C2S.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.ViewModel
{
	internal class ChatCreateViewModel
	{
		private ServerConnection serverConnection;
		public RelayCommand CreateChatCommand { get; set; }
		public string ChatName { get; set; }
		public string? ChatDescription { get; set; }

		public ChatCreateViewModel()
		{
			serverConnection = ServerConnection.GetInstance();
			CreateChatCommand = new RelayCommand(o => RequestChatCreate());
		}

		private void RequestChatCreate()
		{
			ChatCreateClientPacket chatCreateClientPacket = new ChatCreateClientPacket(ChatName, ChatDescription);
			serverConnection.SendPacket(chatCreateClientPacket);
		}
	}
}
