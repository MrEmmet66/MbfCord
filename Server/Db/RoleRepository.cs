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
	interface IRoleRepository
	{
		 Role Add(Role sender);

		 IQueryable<Role> GetAll();

		Task<IQueryable<Role>> GetAllAsync();

		Role Update(Role sender);

		Role GetById(int id);
		Role GetByName(string name);

		int Save();

		Task<int> SaveAsync();
	}
	internal class RoleRepository : IRoleRepository
	{
		private readonly ApplicationContext context;
		public RoleRepository(ApplicationContext context)
		{
			this.context = context;
		}

		public Role Add(Role sender)
		{
			return context.Roles.Add(sender).Entity;
		}

		public IQueryable<Role> GetAll()
		{
			return context.Roles.AsQueryable();
		}

		public async Task<IQueryable<Role>> GetAllAsync()
		{
			return await context.Roles.ToListAsync().ContinueWith(task => task.Result.AsQueryable());
		}

		public Role GetById(int id)
		{
			return context.Roles.Find(id);
		}

		public async Task<Role> GetByIdAsync(int id)
		{
			return await context.Roles.FindAsync(id);
		}

		public Role GetByName(string name)
		{
			return context.Roles.FirstOrDefault(role => role.Name == name);
		}

		public bool Remove(int id)
		{
			return context.Roles.Remove(context.Roles.Find(id)).Entity != null;
		}

		public int Save()
		{
			return context.SaveChanges();
		}

		public async Task<int> SaveAsync()
		{
			return await context.SaveChangesAsync();
		}

		public Role Update(Role sender)
		{
			return context.Roles.Update(sender).Entity;
		}
	}
}
