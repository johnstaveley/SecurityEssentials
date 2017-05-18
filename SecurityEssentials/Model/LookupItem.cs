using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityEssentials.Model
{
    public sealed class LookupItem
    {
        public LookupItem()
        {
            IsOther = false;
            IsHidden = false;
            SecurityQuestionLookupItems = new List<User>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int LookupTypeId { get; set; }

        public string Description { get; set; }
        public int Ordinal { get; set; }
        public bool IsOther { get; set; }
        public bool IsHidden { get; set; }
        public int? GroupId { get; set; }

        // Foreign keys
        public ICollection<User> SecurityQuestionLookupItems { get; set; }

        public LookupType LookupType { get; set; }
    }
}