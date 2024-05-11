using Client.MVVM.Core;
using Client.Net;
using Infrastructure.C2S.MemberAction;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.ViewModel
{
	class MuteMemberWindowViewModel : IMemberRestrictionViewModel
	{
		public int ChatId { get; set; }
		public ChatMemberClientModel TargetMember { get; set; }
		public DateTime RestrictionDate { get; set; }
		public RelayCommand RestrictCommand { get; set; }

		public MuteMemberWindowViewModel()
		{
			RestrictCommand = new RelayCommand(o => RequestMemberMute(), o => !(RestrictionDate == DateTime.MinValue && RestrictionDate > DateTime.Now));
		}

		private void RequestMemberMute()
		{
			ChatMemberMuteRequestClientPacket muteRequest = new ChatMemberMuteRequestClientPacket(ChatId, TargetMember.Id, RestrictionDate);
			ServerConnection.GetInstance().SendPacket(muteRequest);
		}
	}
}
