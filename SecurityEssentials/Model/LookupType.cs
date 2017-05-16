using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityEssentials.Model
{
    public class LookupType
    {
        public LookupType()
        {
            LookupItem = new List<LookupItem>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Description { get; set; }

        // Reverse navigation
        public virtual ICollection<LookupItem> LookupItem { get; set; }
    }
}