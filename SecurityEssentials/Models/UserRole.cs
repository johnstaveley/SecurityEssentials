using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEssentials.Models
{
    public class UserRole
    {
		public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Foreign keys
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }

    }
}
