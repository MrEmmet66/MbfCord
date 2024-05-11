using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.Core
{
    class BaseViewModel : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
