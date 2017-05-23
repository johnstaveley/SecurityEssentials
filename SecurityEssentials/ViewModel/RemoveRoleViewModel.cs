using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
	public class RemoveRoleViewModel
	{
		public User User;
		public bool IsOwnProfile;

		public RemoveRoleViewModel(User user, bool isOwnProfile)
		{
			User = user;
			IsOwnProfile = isOwnProfile;
		}
	}
}