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
    interface IUserRepository
    {
		IQueryable<User> GetAll();
		Task<IQueryable<User>> GetAllAsync();
		User GetById(int id);
		User GetByIdWithIncludes(int id);
        Task<User> GetByUsernameAsync(string username);
		Task<User> GetByIdAsync(int id);
		Task<User> GetByIdWithIncludesAsync(int id);
		bool Remove(int id);
		User Add(in User sender);
		User Update(in User sender);
        void Save();
		Task<int> SaveAsync();
	}
    internal class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;
        public UserRepository()
        {
            _context = Program.ServiceProvider.GetRequiredService<ApplicationContext>();

		}

        public User GetUserByName(string name)
        {
            return _context.Users.FirstOrDefault(user => user.Username == name);
        }
        public User Add(in User sender)
        {
           return _context.Users.Add(sender).Entity;
        }

        public IQueryable<User> GetAll()
        {
            return _context.Users.AsQueryable();
        }

        public Task<IQueryable<User>> GetAllAsync()
        {
            return _context.Users.ToListAsync().ContinueWith(task => task.Result.AsQueryable());
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public User GetByIdWithIncludes(int id)
        {
			return _context.Users.Include(x => x.Roles).FirstOrDefault(x => x.Id == id);
		}

        public Task<User> GetByIdWithIncludesAsync(int id)
        {
			return _context.Users.Include(x => x.Roles).Include(x => x.MemberRestrictions).FirstOrDefaultAsync(x => x.Id == id);
		}

        public bool Remove(int id)
        {
            var user = _context.Users.Find(id);
            if(user == null)
                return false;
            _context.Users.Remove(user);
            return true;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public User Update(in User sender)
        {
            return _context.Users.Update(sender).Entity;
        }

		public async  Task<User> GetByUsernameAsync(string username)
		{
            User user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
            return user;
		}
	}
}
