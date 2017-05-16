using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
    public class LandingViewModel
    {
        public LandingViewModel(string firstName, UserLog lastAccountActivity, int userId)
        {
            FirstName = firstName;
            LastAccountActivity = lastAccountActivity?.DateCreated.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ??
                                  "Never logged in";
            UserId = userId;
        }

        public string FirstName { get; set; }

        public string LastAccountActivity { get; set; }

        public int UserId { get; set; }
    }
}