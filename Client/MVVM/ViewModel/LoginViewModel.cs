using Client.MVVM.Core;
using Client.Net;
using Client.Net.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.MVVM.ViewModel
{
    internal class LoginViewModel
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public string Error { get; set; }

        public RelayCommand TryLoginCommand { get; set; }
        public RelayCommand OpenRegisterWindowCommand { get; set; }
        private ServerConnection serverConnection;

        public LoginViewModel()
        {
            serverConnection = ServerConnection.GetInstance();
            TryLoginCommand = new RelayCommand(o => serverConnection.TryLogin(Username, Password), o => (!string.IsNullOrEmpty(Username) &&  !string.IsNullOrEmpty(Password)));
            OpenRegisterWindowCommand = new RelayCommand(o => new RegisterWindow().Show());
            serverConnection.LoginResult += OnLoginResult;
        }

        private void OnLoginResult(object? sender, AuthEventArgs e)
        {
            Error = e.Message;
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
    }
}
