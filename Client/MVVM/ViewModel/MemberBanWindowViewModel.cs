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
    class MemberBanWindowViewModel : BaseViewModel, IMemberRestrictionViewModel
    {
		private ServerConnection serverConnection = ServerConnection.GetInstance();
		public int ChatId { get; set; }
        public ChatMemberClientModel TargetMember { get; set; }
        public DateTime RestrictionDate { get; set; }
		public string Reason { get; set; }

        public RelayCommand RestrictCommand { get; set; }

        public MemberBanWindowViewModel() { }

        public MemberBanWindowViewModel(ChatMemberClientModel target, int chatId)
        {
			ChatId = chatId;
			target = TargetMember;
			RestrictCommand = new RelayCommand(o => RequestMemberBan(), o => !(RestrictionDate == DateTime.MinValue && RestrictionDate > DateTime.Now && string.IsNullOrWhiteSpace(Reason)));
		}

		private void RequestMemberBan()
		{
			ChatMemberBanRequestClientPacket banRequest = new ChatMemberBanRequestClientPacket(ChatId, TargetMember.Id, RestrictionDate, Reason);
			serverConnection.SendPacket(banRequest);
			Application.Current.Dispatcher.Invoke(() =>
			{
				foreach (Window window in Application.Current.Windows)
				{
					if (window is MemberBanWindow)
					{
						window.Close();
					}
				}
			});
		}
	}
}
