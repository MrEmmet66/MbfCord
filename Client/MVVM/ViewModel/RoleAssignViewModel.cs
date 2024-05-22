using Client.MemberActionWindows;
using Client.MVVM.Core;
using Client.Net;
using Client.Net.Event;
using Infrastructure;
using Infrastructure.C2S.Chat;
using Infrastructure.C2S.Role;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.MVVM.ViewModel
{
	internal class RoleAssignViewModel : BaseViewModel
	{
		public ChatMemberClientModel TargetMember { get; set; }
		public int ChatId { get; set; }
		public List<ChatRoleClientModel> Roles { get; set; }
		public ChatRoleClientModel SelectedRole { get; set; }
		public RelayCommand AssignRoleCommand { get; set; }
		private ServerConnection serverConnection = ServerConnection.GetInstance();
		public RoleAssignViewModel(int chatId, ChatMemberClientModel targetMember)
		{
			TargetMember = targetMember;
			ChatId = chatId;
			AssignRoleCommand = new RelayCommand(o => RequestRoleAsssign(), o => SelectedRole != null);
			serverConnection.ChatRolesResult += OnChatRolesResult;
			serverConnection.RoleAssignResponse += OnRoleAssignResponse;
			RequestRoles();
		}

		private void OnRoleAssignResponse(object? sender, ServerResponseEventArgs e)
		{
			if(e.Status)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					foreach (Window window in Application.Current.Windows)
					{
						if (window is RoleAssignWindow)
						{
							window.Close();
						}
					}
				});
			}
		}

		private void OnChatRolesResult(object? sender, ChatRolesResultEventArgs e)
		{
			Roles = e.Roles;
			OnPropertyChanged(nameof(Roles));
		}

		private void RequestRoles()
		{
			BaseChatRequestClientPacket rolesRequest = new BaseChatRequestClientPacket(PacketType.ChatRolesRequest, ChatId);
			serverConnection.SendPacket(rolesRequest);
		}

		private void RequestRoleAsssign()
		{
			RoleAssignRequestClientPacket assignPacket = new RoleAssignRequestClientPacket(SelectedRole.Id, TargetMember.Id);
			serverConnection.SendPacket(assignPacket);
		}
	}
}
