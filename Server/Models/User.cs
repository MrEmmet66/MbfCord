using Newtonsoft.Json;
using Server.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
    public class User : IEntity
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
        public List<Message>? Messages { get; set; }
        public List<Channel>? Channels { get; set; }
        public List<Role>? Roles { get; set; }
        public List<MemberRestriction>? MemberRestrictions { get; set; }

        public List<MemberRestriction>? GivedMutes { get; set; }
		public List<MemberRestriction>? GivedBans { get; set; }


	}
}
