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
    internal class UserRepository : IRepository<User>
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
            throw new NotImplementedException();
        }

        public Task<User> GetByIdWithIncludesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            var user = _context.Users.Find(id);
            if(user == null)
                return false;
            _context.Users.Remove(user);
            return true;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public User Select(Expression<Func<User, bool>> predicate)
        {
            return _context.Users.FirstOrDefault(predicate);
        }

        public Task<User> SelectAsync(Expression<Func<User, bool>> predicate)
        {
            return _context.Users.FirstOrDefaultAsync(predicate);
        }

        public User Update(in User sender)
        {
            return _context.Users.Update(sender).Entity;
        }
    }
}
