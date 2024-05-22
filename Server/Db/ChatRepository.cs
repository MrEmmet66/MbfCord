using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Chat;

namespace Server.Db
{
    interface IChatRepository
    {
        public Chat.Channel Add(Channel sender);

        public IQueryable<Chat.Channel> GetAll();

        public Task<IQueryable<Chat.Channel>> GetAllAsync();

        public Chat.Channel GetById(int id);

        public Task<Chat.Channel> GetByIdAsync(int id);

        public Chat.Channel GetByIdWithIncludes(int id);


        public Task<Chat.Channel> GetByIdWithIncludesAsync(int id);


        public bool Remove(int id);


        public int Save();


        public Task<int> SaveAsync();


        public Chat.Channel Update(Channel sender);
	}
    internal class ChatRepository : IChatRepository
    {
        public ChatRepository(ApplicationContext _context)
        {
            context = _context;
        }

        private readonly ApplicationContext context;
        public Chat.Channel Add(Chat.Channel sender)
        {
            return context.Add(sender).Entity;
        }

        public IQueryable<Chat.Channel> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<Chat.Channel>> GetAllAsync()
        {
            return await context.Channels.ToListAsync().ContinueWith(task => task.Result.AsQueryable());
        }

        public Chat.Channel GetById(int id)
        {
			return context.Channels.Find(id);
		}

        public async Task<Chat.Channel> GetByIdAsync(int id)
        {
            return await context.Channels.FindAsync(id);
        }

        public Chat.Channel GetByIdWithIncludes(int id)
        {
            return context.Channels.Include(x => x.Members).Include(c => c.Messages).FirstOrDefault(x => x.Id == id);
        }

        public async Task<Chat.Channel> GetByIdWithIncludesAsync(int id)
        {
            return await context.Channels.Include(x => x.Members).Include(c => c.Messages).Include(r => r.Roles).FirstOrDefaultAsync(x => x.Id == id);
        }

        public bool Remove(int id)
        {
            return context.Channels.Remove(context.Channels.Find(id)) != null;
		}

        public int Save()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }

        public Chat.Channel Update(Channel sender)
        {
            return context.Channels.Update(sender).Entity;
        }
    }
}
