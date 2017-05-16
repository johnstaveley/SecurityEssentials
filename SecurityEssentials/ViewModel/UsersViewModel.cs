namespace SecurityEssentials.ViewModel
{
    public class UsersViewModel
    {
        #region Constructor

        public UsersViewModel(int currentUserId)
        {
            CurrentUserId = currentUserId;
        }

        #endregion

        #region Declarations

        public int CurrentUserId { get; set; }

        #endregion
    }
}