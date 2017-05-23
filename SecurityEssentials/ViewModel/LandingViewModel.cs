using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
	public class LandingViewModel
	{

		public string FirstName { get; set; }

		public string LastAccountActivity { get; set; }

		public int UserId { get; set; }

		public LandingViewModel(string firstName, UserLog lastAccountActivity, int userId)
		{

			FirstName = firstName;
			LastAccountActivity = (lastAccountActivity != null ? lastAccountActivity.CreatedDateUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "Never logged in");
			UserId = userId;

		}
	}
}