using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Chat
{
	public class ChatEditRequestClientPacket : BaseChatRequestClientPacket
	{
		public string NewName { get; set; }
		public string NewDescription { get; set; }
		public ChatEditRequestClientPacket(int chatId, string name, string description) : base(PacketType.ChatEditRequest, chatId)
		{
			NewName = name;
			NewDescription = description;
		}
	}
}
