using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Model
{
	public class ChatClientModel : IClientModel
	{
		public ChatClientModel() { }
		public ChatClientModel(int id, string name, string? description)
		{
			Id = id;
			Name = name;
			Description = description;
		}

		public ChatClientModel(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
	}
}
