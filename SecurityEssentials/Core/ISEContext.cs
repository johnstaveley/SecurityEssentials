using System.Data.Entity;
using SecurityEssentials.Model;
using System.Threading.Tasks;

namespace SecurityEssentials.Core
{
    public interface ISEContext
    {
        DbSet<LookupItem> LookupItem { get; set; }
        DbSet<LookupType> LookupType { get; set; }
        DbSet<Role> Role { get; set; }
        DbSet<User> User { get; set; }
        DbSet<UserLog> UserLog { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
		void SetModified(object entity);
		void SetConfigurationValidateOnSaveEnabled(bool isValidated);

        void Dispose();
    }
}