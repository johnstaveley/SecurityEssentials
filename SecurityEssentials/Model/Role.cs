using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEssentials.Model
{
    public class Role 
    {

		public int Id { get; set; }

        /// <summary>
        /// The name of the role e.g. Installer
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A description of what it gives you access to
        /// </summary>
        public string PermissionDescription { get; set; }

        // Reverse navigation
        public virtual ICollection<UserRole> UserRoles { get; set; } 

        public Role()
        {
            UserRoles = new List<UserRole>();
        }

    }
}
