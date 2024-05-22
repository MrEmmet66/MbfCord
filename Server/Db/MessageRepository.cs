using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Server.Db
{
    interface IMessageRepository
	{
		public Message Add(Message sender);

		public IQueryable<Message> GetAll();

		public Task<IQueryable<Message>> GetAllAsync();

		public Message GetById(int id);

		public Task<Message> GetByIdAsync(int id);

		public Message GetByIdWithIncludes(int id);

		public Task<Message> GetByIdWithIncludesAsync(int id);

		public bool Remove(int id);

		public int Save();

		public Task<int> SaveAsync();

		public Message Update(Message sender);
	}
	internal class MessageRepository : IMessageRepository
    {
        private readonly ApplicationContext context;
        public MessageRepository(ApplicationContext context)
        {
            this.context = context;
        }
        public Message Add(Message sender)
        {
            return context.Messages.Add(sender).Entity;
        }

        public IQueryable<Message> GetAll()
        {
            return context.Messages.AsQueryable();
        }

        public async Task<IQueryable<Message>> GetAllAsync()
        {
            return await context.Messages.ToListAsync().ContinueWith(task => task.Result.AsQueryable());
        }

        public Message GetById(int id)
        {
            return context.Messages.Find(id);
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public Message GetByIdWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Message> GetByIdWithIncludesAsync(int id)
        {
            return await context.Messages.Include(c => c.Channel).Include(s => s.Sender).FirstOrDefaultAsync(c => c.Id == id);
        }

        public bool Remove(int id)
        {
            return context.Messages.Remove(context.Messages.Find(id)).Entity != null;
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        public Message Update(Message sender)
        {
            return context.Messages.Update(sender).Entity;
        }
    }
}
