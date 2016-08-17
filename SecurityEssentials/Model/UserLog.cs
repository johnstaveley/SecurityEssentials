using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
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
		public DateTime DateCreated {get; set; }

		[Required, MinLength(2), Display(Name = "Description")]
		public string Description { get; set; }

		// Foreign Keys
		public virtual User User { get; set; }

        public UserLog()
        {
			DateCreated = DateTime.UtcNow;
        }


    }
}
