using Newtonsoft.Json;
using Server.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
    public class Channel
    {
        public event EventHandler<EventArgs> MessageReceived;
        public event EventHandler<EventArgs> UserJoined;
        public event EventHandler<EventArgs> UserLeft;
        public event EventHandler<UserPermissionChangeEventArgs> UserPermissionChanged;
        public event EventHandler<EventArgs> ChannelDeleted;
        public event EventHandler<EventArgs> ChannelRenamed;
        public event EventHandler<ChannelDescriptionChangeEventArgs> ChannelDescriptionChanged;
        public event EventHandler<EventArgs> ChannelCreated;

        public Channel(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public Channel() { }
        public int Id { get; set; }
        [JsonIgnore]
        public List<Message>? Messages { get; set; }
        [JsonIgnore]
        public List<User> Members { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }



        protected virtual void OnMessageReceived()
        {
            MessageReceived?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUserJoined()
        {
            UserJoined?.Invoke(this, EventArgs.Empty);
        }

    }
}
