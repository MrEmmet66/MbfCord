using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Model
{
	public class BannedMemberClientModel : IClientModel
	{
		public BannedMemberClientModel(int id, string username, string bannedBy, DateTime banStart, DateTime banEnd, string banReason)
		{
			Id = id;
			Username = username;
			BannedBy = bannedBy;
			BanStart = banStart;
			BanEnd = banEnd;
			BanReason = banReason;
		}


		public int Id { get; set; }
		public string Username { get; set; }
		public string BannedBy { get; set; }
		public DateTime BanStart { get; set; }
		public DateTime BanEnd { get; set; }
		public string BanReason { get; set; }
	}
}
