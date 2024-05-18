using Client.MVVM.Core;
using Client.MVVM.Model;
using Client.Net;
using Client.Net.Event;
using Infrastructure.C2S.Chat;
using Infrastructure;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Infrastructure.C2S.Role;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client.MVVM.ViewModel
{
    class ChatSettingsViewModel : BaseViewModel
    {
        private ServerConnection serverConnection;
        private ChatRoleClientModel selectedRole;
        public Chat TargetChat { get; set; }
        public ObservableCollection<ChatRoleClientModel> Roles { get; set; }
        public ChatRoleClientModel SelectedRole
        {
            get => selectedRole;
            set
            {
                selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
            }
        }
        public RelayCommand SaveRoleCommand { get; set; }
		public RelayCommand DeleteRoleCommand { get; set; }
        public RelayCommand AddRoleCommand { get; set; }
		public RelayCommand SaveChatCommand { get; set; }
		public RelayCommand DeleteChatCommand { get; set; }



		public ChatSettingsViewModel(Chat chat)
        {
            serverConnection = ServerConnection.GetInstance();
			TargetChat = chat;

			serverConnection.ChatRolesResult += OnChatRolesResult;
			serverConnection.RoleAddResponse += (sender, args) => MessageBox.Show(args.Message, "Role create result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);
			serverConnection.RoleEditResponse += (sender, args) => MessageBox.Show(args.Message, "Role edit result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);
			serverConnection.RoleRemoveResponse += (sender, args) => MessageBox.Show(args.Message, "Role remove result", MessageBoxButton.OK, args.Status ? MessageBoxImage.Information : MessageBoxImage.Error);



			SaveChatCommand = new RelayCommand(o => SaveChat());
			SaveRoleCommand = new RelayCommand(o => SaveRole(), o => SelectedRole != null);
			DeleteRoleCommand = new RelayCommand(o => DeleteRole(), o => SelectedRole != null);
			AddRoleCommand = new RelayCommand(o => AddRole());
			DeleteChatCommand = new RelayCommand(o => DeleteChat());

			RequestRoles();
		}

		private void DeleteChat()
		{
			BaseChatRequestClientPacket chatDeleteRequest = new BaseChatRequestClientPacket(PacketType.ChatRemoveRequest, TargetChat.Id);
			serverConnection.SendPacket(chatDeleteRequest);
		}

		private void AddRole()
		{
			ChatRoleClientModel newRole = new ChatRoleClientModel();
			newRole.Name = "New Role";
			Roles.Add(newRole);
			AddRoleRequestClientPacket addRoleRequestClientPacket = new AddRoleRequestClientPacket(newRole, TargetChat.Id);
			serverConnection.SendPacket(addRoleRequestClientPacket);
		}

		private void DeleteRole()
		{
			RemoveRoleRequestClientPacket removeRoleRequestClientPacket = new RemoveRoleRequestClientPacket(SelectedRole.Id);
			serverConnection.SendPacket(removeRoleRequestClientPacket);
		}

		private void SaveRole()
		{
			EditRoleRequestClientPacket editRoleRequestClientPacket = new EditRoleRequestClientPacket(SelectedRole);
			serverConnection.SendPacket(editRoleRequestClientPacket);

		}

		private void SaveChat()
		{
			ChatEditRequestClientPacket chatEditPacket = new ChatEditRequestClientPacket(TargetChat.Id, TargetChat.Name, TargetChat.Description);
			serverConnection.SendPacket(chatEditPacket);
		}

		private void RequestRoles()
		{
			BaseChatRequestClientPacket rolesRequest = new BaseChatRequestClientPacket(PacketType.ChatRolesRequest, TargetChat.Id);
			serverConnection.SendPacket(rolesRequest);

		}

		private void OnChatRolesResult(object? sender, ChatRolesResultEventArgs e)
		{
            Roles = new ObservableCollection<ChatRoleClientModel>(e.Roles);
			OnPropertyChanged(nameof(Roles));
		}
	}
}
