using System.Collections.Generic;

namespace SecurityEssentials.Model
{
    public sealed class Role
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
        }

        public int Id { get; set; }

        /// <summary>
        ///     The name of the role e.g. Installer
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     A description of what it gives you access to
        /// </summary>
        public string PermissionDescription { get; set; }

        // Reverse navigation
        public ICollection<UserRole> UserRoles { get; set; }
    }
}