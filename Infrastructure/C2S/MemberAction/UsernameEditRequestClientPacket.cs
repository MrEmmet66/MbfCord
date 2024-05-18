using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.MemberAction
{
	public class UsernameEditRequestClientPacket : BaseClientPacket
	{
		public string NewUsername { get; set; }
		public UsernameEditRequestClientPacket(string newUsername) : base(PacketType.UsernameEdit)
		{
			NewUsername = newUsername;
		}
	}
}
