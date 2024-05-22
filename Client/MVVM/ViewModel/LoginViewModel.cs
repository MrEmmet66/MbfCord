using Client.MVVM.Core;
using Client.Net;
using Client.Net.Event;
using Infrastructure;
using Infrastructure.C2S.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.MVVM.ViewModel
{
    internal class LoginViewModel : BaseViewModel
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }

        public RelayCommand TryLoginCommand { get; set; }
        public RelayCommand OpenRegisterWindowCommand { get; set; }
        private ServerConnection serverConnection;

        public LoginViewModel()
        {
            serverConnection = ServerConnection.GetInstance();
            TryLoginCommand = new RelayCommand(o => TryLogin(Username, Password), o => (!string.IsNullOrEmpty(Username) &&  !string.IsNullOrEmpty(Password)));
            OpenRegisterWindowCommand = new RelayCommand(o => new RegisterWindow().Show());
            serverConnection.LoginResult += OnLoginResult;
        }

        private void OnLoginResult(object? sender, AuthEventArgs e)
        {
            Status = e.Message;
            OnPropertyChanged(nameof(Status));
            if (e.Status)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new MainWindow().Show();
                    foreach(Window window in Application.Current.Windows)
                    {

                        if (window is LoginWindow)
                        {
                            window.Close();
                        }
                        if(window is RegisterWindow)
                        {
                            window.Close();
                        }
                            
                    }
                });
            }
            else
            {
                MessageBox.Show(e.Message);
            }
        }

		public void TryLogin(string username, string password)
		{
			serverConnection.EstablishConnection();
			AuthClientPacket packet = new AuthClientPacket(PacketType.LoginRequest, username, password);
			serverConnection.SendPacket(packet);
            Status = "Logging in...";
            OnPropertyChanged(nameof(Status));
		}
	}
}
