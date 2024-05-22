using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.Model
{
    internal class Chat
    {
        public Chat(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Chat(ChatClientModel chatModel)
        {
            Id = chatModel.Id;
			Name = chatModel.Name;
			Description = chatModel.Description;

		}
		public Chat() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
