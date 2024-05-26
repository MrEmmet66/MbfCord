using Server.Chat;
using Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    internal class MemberRestrictionService
    {
        private readonly MemberRestrictionRepository memberRestrictionRepository;
        public MemberRestrictionService(MemberRestrictionRepository memberRestrictionRepository)
        {
            this.memberRestrictionRepository = memberRestrictionRepository;
        }

        public async Task<MemberRestriction> GetUserRestrictionAsync(User user, Channel chat)
        {
            var memberRestrictions = await memberRestrictionRepository.GetAllAsync();
            var restriction = memberRestrictions.FirstOrDefault(x => x.Member.Id == user.Id && x.Chat.Id == chat.Id);
            return restriction;
        }

        public async Task UnbanUserAsync(User user, Channel chat)
        {
            var restriction = await GetUserRestrictionAsync(user, chat);
			if (restriction != null)
            {
                restriction.BanEnd = DateTime.Now;
                memberRestrictionRepository.Update(restriction);
                await memberRestrictionRepository.SaveAsync();
            }
        }

        public async Task<List<MemberRestriction>> GetChatRestrictionsAsync(int chatId)
        {
            var restrictions = await memberRestrictionRepository.GetAllAsync();
            var chatRestrictions = restrictions.Where(x => x.Chat.Id == chatId);
            return chatRestrictions.ToList();
        }

        public async Task BanUserAsync(Channel chat, User targetUser, DateTime banEnd, string reason, User bannedBy)
        {
            MemberRestriction restriction = new MemberRestriction
            {
                Chat = chat,
                Member = targetUser,
                BanStart = DateTime.Now,
                BanEnd = banEnd,
                BannedBy = bannedBy,
                BanReason = reason
            };
            MemberRestriction userRestriction = await GetUserRestrictionAsync(targetUser, chat);
            if (userRestriction != null)
            {
                userRestriction.BanStart = DateTime.Now;
                userRestriction.BanEnd = banEnd;
                userRestriction.BannedBy = bannedBy;
                userRestriction.BanReason = reason;
                memberRestrictionRepository.Update(userRestriction);
            }
            else
                memberRestrictionRepository.Add(restriction);
            await memberRestrictionRepository.SaveAsync();
        }

        public async Task MuteUserAsync(Channel chat, User targetUser, DateTime muteEnd, string reason, User mutedBy)
        {
            MemberRestriction memberRestriction = new MemberRestriction
            {
                Chat = chat,
                Member = targetUser,
                MuteStart = DateTime.Now,
                MuteEnd = muteEnd,
                MutedBy = mutedBy,
                MuteReason = reason
            };
            MemberRestriction userRestriction = await GetUserRestrictionAsync(targetUser, chat);
            if (userRestriction != null)
            {
                userRestriction.MuteStart = DateTime.Now;
                userRestriction.MuteEnd = muteEnd;
                memberRestrictionRepository.Update(userRestriction);
            }
            else
                memberRestrictionRepository.Add(memberRestriction);
            await memberRestrictionRepository.SaveAsync();
        }

        public async Task<bool> IsUserMutedAsync(User user, int chatId)
        {
            if (user.MemberRestrictions == null)
                return false;
            var memberRestrictions = await memberRestrictionRepository.GetAllAsync();
            bool result = memberRestrictions.Any(x => x.Member.Id == user.Id && x.Chat.Id == chatId && x.MuteEnd > DateTime.Now);
            return result;
        }

        public bool IsUserBanned(User user, int chatId)
        {
            if (user.MemberRestrictions == null)
                return false;
            bool result = memberRestrictionRepository.GetAll().Any(x => x.Member.Id == user.Id && x.Chat.Id == chatId && x.BanEnd > DateTime.Now);
            return result;
        }

        public async Task<bool> IsUserBannedAsync(User user, int chatId)
        {
            if (user.MemberRestrictions == null)
                return false;
            var memberRestrictions = await memberRestrictionRepository.GetAllAsync();
            bool result = memberRestrictions.Any(x => x.Member.Id == user.Id && x.Chat.Id == chatId && x.BanEnd > DateTime.Now);
            return result;
        }
    }
}
