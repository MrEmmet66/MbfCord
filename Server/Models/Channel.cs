using Newtonsoft.Json;
using Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
    public class Channel : IEntity
    {

        public Channel(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public Channel() { }
        public int Id { get; set; }
        public List<Message>? Messages { get; set; }
        public List<User> Members { get; set; }
        public List<Role> Roles { get; set; }
        
        public string Name { get; set; }
        public string? Description { get; set; }

    }
}
