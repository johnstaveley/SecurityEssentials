using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using SecurityEssentials.Model;

namespace SecurityEssentials.Core
{
	public class SeContext : DbContext, ISeContext
    {
		public SeContext()
			: base("DefaultConnection")
		{
			Database.SetInitializer(new SeDatabaseIntialiser());
		}

	    public DbSet<Log> Log { get; set; }
		public DbSet<LookupItem> LookupItem { get; set; }
		public DbSet<LookupType> LookupType { get; set; }
	    public DbSet<PreviousPassword> PreviousPassword { get; set; }
		public DbSet<Role> Role { get; set; }
		public DbSet<User> User { get; set; }
		public DbSet<UserLog> UserLog { get; set; }
	    public DbSet<UserRole> UserRole { get; set; }

		public void SetDeleted(object entity)
	    {
		    Entry(entity).State = EntityState.Deleted;
	    }
	    public void SetModified(object entity)
	    {
		    Entry(entity).State = EntityState.Modified;
	    }

	    public IEnumerable<DbValidationError> GetValidationErrors(object entity)
	    {
		    return Entry(entity).GetValidationResult().ValidationErrors;
	    }

	    public void SetConfigurationValidateOnSaveEnabled(bool isValidated)
	    {
		    Configuration.ValidateOnSaveEnabled = isValidated;
	    }


	}

}
