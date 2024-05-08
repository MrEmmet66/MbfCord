using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ClientInfoResultEventArgs : EventArgs
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public ClientInfoResultEventArgs(int id, string username)
		{
			Id = id;
			Username = username;
		}
	}
}
