using Client.MVVM.Core;
using Client.Net;
using Client.Net.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.MVVM.ViewModel
{
    class RegisterViewModel : INotifyPropertyChanged
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
                OnPropertyChanged("Status");
            }
        }
        public RelayCommand RegisterRequestCommand { get; set; }
        private ServerConnection serverConnection;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RegisterViewModel()
        {
            serverConnection = ServerConnection.GetInstance();
            RegisterRequestCommand = new RelayCommand(o => TryRegister());
            serverConnection.RegisterResult += OnRegisterResult;
        }

        private void OnRegisterResult(object? sender, AuthEventArgs e)
        {
            Status = e.Message;
            MessageBox.Show(Status);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TryRegister()
        {
            if (Password.Equals(ConfirmPassword))
            {
                Status = "Passwords do not match";
                return;
            }
            serverConnection.EstablishConnection();
            serverConnection.SendRegisterPacket(Username, Password);
            Status = "Registering...";
        }
    }
}
