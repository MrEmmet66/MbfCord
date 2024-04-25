using Client.Net.Event;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.Model
{
    internal class ChatMember
    {
        public ChatMember() { }
        public ChatMember(ChatMemberClientModel model)
        {
            Id = model.Id;
            Username = model.Username;
            Status = model.Status;
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public bool Status { get; set; }
    }
}
