using Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
	internal class MemberRestrictionService
	{
		private readonly MemberRestrictionRepository memberRestrictionRepository;
		private readonly IUserRepository userRepository;
		public MemberRestrictionService(MemberRestrictionRepository memberRestrictionRepository, IUserRepository userRepository)
		{
			this.memberRestrictionRepository = memberRestrictionRepository;
			this.userRepository = userRepository;
		}

		public async Task MuteUserAsync(Channel chat, User targetUser, DateTime muteEnd)
		{
			MemberRestriction memberRestriction = new MemberRestriction
			{
				Chat = chat,
				Member = targetUser,
				MuteStart = DateTime.Now,
				MuteEnd = muteEnd
			};
			memberRestrictionRepository.Add(memberRestriction);
			await memberRestrictionRepository.SaveAsync();
		}

		public async Task<bool> IsUserMutedAsync(User user)
		{
			if (user.MemberRestrictions == null)
				return false;
			var memberRestrictions = await memberRestrictionRepository.GetAllAsync();
			bool result = memberRestrictions.Any(x => x.Member.Id == user.Id && x.MuteEnd > DateTime.Now);
			return result;
		}

		public bool IsUserBanned(User user)
		{
			if(user.MemberRestrictions == null)
				return false;
			bool result = memberRestrictionRepository.GetAll().Any(x => x.Member.Id == user.Id && x.BanEnd > DateTime.Now);
			return result;
		}
	}
}
