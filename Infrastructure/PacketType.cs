using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
	public enum PacketType
	{
		LoginRequest,
		RegisterRequest,
		LoginResult,
		RegisterResult,
		ChatCreate,
		NewChat,
		ChatsRequest,
		ChatsResult,
		ChatJoinRequest,
		ChatJoinResult,
		UserChatsRequest,
		UserChatsResult,
		ChatMessagesRequest,
		ChatMessagesResult,
		ChatMembersRequest,
		ChatMembersResult,
		ChatLeaveRequest,
		ChatLeaveResult,
		ChatDeleteRequest,
		ChatDeleteResult,
		Message,
		Media,
		UserAction,
		ActionDenied,
		ChatRolesRequest,
		ChatRolesResponse,
		ClientInfoResponse,
		ClientInfoRequest,
		ChatMemberKickRequest,
		ChatMemberKickResult,
		ChatMemberAdded,
		ChatMemberRemoved,
		ChatMemberMuteRequest,
		ChatMemberMuteResponse,
		ChatMemberBanRequest,
		ChatMemberBanResponse,
		RoleAddRequest,
		RoleAddResponse,
		RoleEditRequest,
		RoleEditResponse,
		RoleDeleteRequest,
		RoleDeleteResponse,
		RoleAssignRequest,
		RoleAssignResponse,
	}
}
