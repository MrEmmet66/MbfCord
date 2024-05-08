using Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
	public class MemberRestriction : IEntity
	{
		public int Id { get; set; }
		public User Member { get; set; }
		public Channel Chat { get; set; }
		public DateTime MuteStart { get; set; }
		public DateTime MuteEnd { get; set; }
		public DateTime BanStart { get; set; }
		public DateTime BanEnd { get; set; }
	}
}
