using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Chat;
using System.ComponentModel.DataAnnotations;

namespace Server.Db
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            Database.EnsureCreated(); 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat.Channel> Channels { get; set; }
        public DbSet<Role> Roles { get; set; }
		public DbSet<MemberRestriction> MemberRestrictions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=chat_app;Trusted_Connection=True;trustservercertificate=true;");
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasData(
				new User { Id = 1, Username = "", HashedPassword = ""});
			base.OnModelCreating(modelBuilder);
		}
	}
}
