using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace SecurityEssentials.Models
{
	public class UsersContext : DbContext
	{
		public UsersContext()
			: base("DefaultConnection")
		{
		}

        public DbSet<User> User { get; set; }
		public DbSet<Role> Role { get; set; }

	}

}
