using Client.MVVM.Core;
using Client.Net;
using Client.Net.Event;
using Infrastructure;
using Infrastructure.C2S.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.MVVM.ViewModel
{
    class RegisterViewModel : BaseViewModel
    {
        private string status;
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
				OnPropertyChanged(nameof(Status));
			}
        }
        public RelayCommand RegisterRequestCommand { get; set; }
        private ServerConnection serverConnection;


        public RegisterViewModel()
        {
            serverConnection = ServerConnection.GetInstance();
            RegisterRequestCommand = new RelayCommand(o => TryRegister(), o => (Password != null && Password.Length >= 6));
            serverConnection.RegisterResult += OnRegisterResult;
        }

        private void OnRegisterResult(object? sender, AuthEventArgs e)
        {
            Status = e.Message;
            MessageBox.Show(Status);
        }

        private void TryRegister()
        {
            if (Password.Equals(ConfirmPassword))
            {
                Status = "Passwords do not match";
                return;
            }
            serverConnection.EstablishConnection();
            AuthClientPacket packet = new AuthClientPacket(PacketType.RegisterRequest, Username, Password);
			serverConnection.SendPacket(packet);

			Status = "Registering...";
        }
    }
}
