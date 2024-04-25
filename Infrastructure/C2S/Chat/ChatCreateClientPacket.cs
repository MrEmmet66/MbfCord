
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.C2S.Chat
{
	public class ChatCreateClientPacket : BaseClientPacket
	{
		public ChatCreateClientPacket(string name, string description) : base(PacketType.ChatCreate)
		{
			Name = name;
			Description = description;
		}
		
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
