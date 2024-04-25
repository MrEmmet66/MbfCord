using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
    public class User
    {
        public User(string userName, string hashedPassword)
        {
            Username = userName;
            HashedPassword = hashedPassword;
        }
        public User() { }
        public int Id { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        [JsonIgnore]
        public List<Message>? Messages { get; set; }
        [JsonIgnore]
        public List<Channel>? Channels { get; set; }


    }
}
