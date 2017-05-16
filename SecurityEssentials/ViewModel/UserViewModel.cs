using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
    public class UserViewModel
    {
        #region Constructor

        public UserViewModel(int currentUserId, bool isAdministrator, User user)
        {
            if (currentUserId == user.Id) IsOwnProfile = true;
            else IsOwnProfile = false;
            IsAdministrator = isAdministrator;
            User = user;
        }

        #endregion

        #region Declarations

        public bool IsAdministrator { get; set; }
        public User User { get; set; }
        public bool IsOwnProfile { get; set; }

        #endregion
    }
}