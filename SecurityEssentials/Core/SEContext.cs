using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using SecurityEssentials.Model;

namespace SecurityEssentials.Core
{
	public class SEContext : DbContext
	{
		public SEContext()
			: base("DefaultConnection")
		{
			Database.SetInitializer<SEContext>(new SEDatabaseIntialiser());
		}

		public DbSet<LookupItem> LookupItem { get; set; }
		public DbSet<LookupType> LookupType { get; set; }
		public DbSet<Role> Role { get; set; }
		public DbSet<User> User { get; set; }
		public DbSet<UserLog> UserLog { get; set; }
		
	}

}
