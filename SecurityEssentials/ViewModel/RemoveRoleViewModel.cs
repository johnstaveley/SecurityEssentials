using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
	public class RemoveRoleViewModel
	{
		public User User { get; set; }
		public bool IsOwnProfile { get; set; }

		public RemoveRoleViewModel(User user, bool isOwnProfile)
		{
			User = user;
			IsOwnProfile = isOwnProfile;
		}
	}
}