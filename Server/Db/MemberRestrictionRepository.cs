﻿using Microsoft.EntityFrameworkCore;
using Server.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.Db
{
	interface IMemberRestrictionRepository
	{
		public MemberRestriction Add(MemberRestriction sender);

		public IQueryable<MemberRestriction> GetAll();

		public  Task<IQueryable<MemberRestriction>> GetAllAsync();

		public MemberRestriction GetById(int id);

		public  Task<MemberRestriction> GetByIdAsync(int id);

		public MemberRestriction GetByIdWithIncludes(int id);


		public bool Remove(int id);

		public int Save();

		public Task<int> SaveAsync();

		public MemberRestriction Update(in MemberRestriction sender);
	}
	internal class MemberRestrictionRepository : IMemberRestrictionRepository
	{
		private readonly ApplicationContext context;
		public MemberRestrictionRepository(ApplicationContext context)
		{
			this.context = context;
		}

		public MemberRestriction Add(MemberRestriction sender)
		{
			return context.MemberRestrictions.Add(sender).Entity;
		}

		public IQueryable<MemberRestriction> GetAll()
		{
			return context.MemberRestrictions.AsQueryable();
		}

		public async Task<IQueryable<MemberRestriction>> GetAllAsync()
		{
			return await context.MemberRestrictions.Include(m => m.Member).ToListAsync().ContinueWith(task => task.Result.AsQueryable());
		}

		public MemberRestriction GetById(int id)
		{
			return context.MemberRestrictions.Find(id);
		}

		public async Task<MemberRestriction> GetByIdAsync(int id)
		{
			return await context.MemberRestrictions.FindAsync(id);
		}

		public MemberRestriction GetByIdWithIncludes(int id)
		{
			return context.MemberRestrictions.Include(x => x.Member).Include(c => c.Chat).FirstOrDefault(x => x.Id == id);
		}

		public async Task<MemberRestriction> GetByIdWithIncludesAsync(int id)
		{
			return await context.MemberRestrictions.Include(x => x.Member).Include(c => c.Chat).FirstOrDefaultAsync(x => x.Id == id);
		}

		public bool Remove(int id)
		{
			context.MemberRestrictions.Remove(context.MemberRestrictions.Find(id));
			return true;
		}

		public int Save()
		{
			return context.SaveChanges();
		}

		public Task<int> SaveAsync()
		{
			return context.SaveChangesAsync();
		}

		public MemberRestriction Select(Expression<Func<MemberRestriction, bool>> predicate)
		{
			throw new NotImplementedException();
		}

		public Task<MemberRestriction> SelectAsync(Expression<Func<MemberRestriction, bool>> predicate)
		{
			throw new NotImplementedException();
		}

		public MemberRestriction Update(in MemberRestriction sender)
		{
			return context.MemberRestrictions.Update(sender).Entity;
		}
	}
}
