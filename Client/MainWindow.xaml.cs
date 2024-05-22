using Client.MVVM.Model;
using Client.MVVM.ViewModel;
using Client.Net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)messagesListbox.Items).CollectionChanged += OnMessagesListboxChanged;
		}

		private void OnMessagesListboxChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (VisualTreeHelper.GetChildrenCount(messagesListbox) > 0)
			{
				Border border = (Border)VisualTreeHelper.GetChild(messagesListbox, 0);
				ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}
		}

		private void changeUsernameButton_Click(object sender, RoutedEventArgs e)
		{
            Button button = sender as Button;
            ContextMenu contextMenu = button.ContextMenu;
			contextMenu.PlacementTarget = button;
			contextMenu.IsOpen = true;
            e.Handled = true;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
            if(e.Key == Key.Enter)
            {
                var viewModel = (MainViewModel)DataContext;
                if(viewModel.SendMessageCommand.CanExecute(null))
                {
                    string message = messageTextBox.Text;
					viewModel.SendMessageCommand.Execute(message);
                    messageTextBox.Clear();
                }
            }
        }
    }
}