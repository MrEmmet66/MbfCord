using Client.MVVM.Model;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.Core
{
    interface IMemberRestrictionViewModel
    {
        int ChatId { get; set; }
        ChatMemberClientModel TargetMember { get; set; }
		DateTime RestrictionDate { get; set; }
		RelayCommand RestrictCommand { get; set; }
	}
}
