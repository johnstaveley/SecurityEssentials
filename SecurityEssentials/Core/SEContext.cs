using System.Data.Entity;
using SecurityEssentials.Model;

namespace SecurityEssentials.Core
{
    public class SEContext : DbContext, ISEContext
    {
        public SEContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new SEDatabaseIntialiser());
        }

        public DbSet<Log> Log { get; set; }

        public DbSet<LookupItem> LookupItem { get; set; }
        public DbSet<LookupType> LookupType { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserLog> UserLog { get; set; }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void SetConfigurationValidateOnSaveEnabled(bool isValidated)
        {
            Configuration.ValidateOnSaveEnabled = isValidated;
        }
    }
}