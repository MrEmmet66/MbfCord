using Server.Chat;
using Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
	internal class RoleService
	{
		private readonly RoleRepository roleRepository;

		public RoleService(RoleRepository roleRepository)
		{
			this.roleRepository = roleRepository;
		}

		public bool IsRoleExists(string roleName)
		{
			return roleRepository.GetAll().Any(r => r.Name == roleName);
		}
	}
}
