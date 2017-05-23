using System.Linq;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{

	public class UserViewModel
    {

		public bool IsAccessingUserAnAdmin { get; set; }
	    public bool IsCurrentUserAnAdmin { get; set; }
	    public User User { get; set; }
	    public bool IsOwnProfile { get; set; }

	    public UserViewModel(int currentUserId, bool isAccessingUserAnAdmin, User user)
	    {
		    IsOwnProfile = currentUserId == user.Id;
		    IsAccessingUserAnAdmin = isAccessingUserAnAdmin;
		    IsCurrentUserAnAdmin = user.UserRoles.Any(a => a.RoleId == Consts.Roles.Admin);
		    User = user;
	    }

	}
}