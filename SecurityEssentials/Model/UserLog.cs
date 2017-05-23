using System;
using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.Model
{

	/// <summary>
	/// Table to store all account activity
	/// </summary>
	public class UserLog
    {

		public int Id { get; set; }
		
		[Required]
		public int UserId { get; set; }

		[Required, Display(Name="Date Created")]
		public DateTime CreatedDateUtc {get; set; }

		[Required, MinLength(2), Display(Name = "Description")]
		public string Description { get; set; }

		// Foreign Keys
		public virtual User User { get; set; }

        public UserLog()
        {
			CreatedDateUtc = DateTime.UtcNow;
        }


    }
}
