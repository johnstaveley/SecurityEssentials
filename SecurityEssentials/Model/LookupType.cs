using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityEssentials.Model
{

    public class LookupType
    {

		[DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
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
