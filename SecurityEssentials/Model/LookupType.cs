using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEssentials.Model
{

    public class LookupType
    {

		public int Id { get; set; } 
        public string Description { get; set; } 

        // Reverse navigation
        public virtual ICollection<LookupItem> LookupItem { get; set; }

        public LookupType()
        {
            LookupItem = new List<LookupItem>();
        }
    }
}
