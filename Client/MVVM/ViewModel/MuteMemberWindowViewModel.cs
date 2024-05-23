using Client.MemberActionWindows;
using Client.MVVM.Core;
using Client.Net;
using Infrastructure.C2S.MemberAction;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.MVVM.ViewModel
{
	class MuteMemberWindowViewModel : BaseViewModel, IMemberRestrictionViewModel
	{
		public int ChatId { get; set; }
		public ChatMemberClientModel TargetMember { get; set; }
		public DateTime RestrictionDate { get; set; }
		public RelayCommand RestrictCommand { get; set; }
		public string Reason { get; set; }

		public MuteMemberWindowViewModel()
		{
			RestrictCommand = new RelayCommand(o => RequestMemberMute(), o => !(RestrictionDate == DateTime.MinValue && RestrictionDate > DateTime.Now && string.IsNullOrWhiteSpace(Reason)));
		}

		private void RequestMemberMute()
		{
			ChatMemberMuteRequestClientPacket muteRequest = new ChatMemberMuteRequestClientPacket(ChatId, TargetMember.Id, RestrictionDate, Reason);
			ServerConnection.GetInstance().SendPacket(muteRequest);
			Application.Current.Dispatcher.Invoke(() =>
			{
				foreach (Window window in Application.Current.Windows)
				{
					if (window is MemberMuteWindow)
					{
						window.Close();
					}
				}
			});
		}
	}
}
