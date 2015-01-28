using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityEssentials.Model
{

    public class LookupItem
    {

		[DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
		public int Id { get; set; }

        public int LookupTypeId { get; set; }

        public string Description { get; set; }
        public int Ordinal { get; set; }
        public bool IsOther { get; set; }
        public bool IsHidden { get; set; }
        public int? GroupId { get; set; }

        // Foreign keys
//        public virtual ICollection<EmailQueue> EntityTypeEmailQueues { get; set; }
        public virtual LookupType LookupType { get; set; }
                                           
        public LookupItem()
        {
            IsOther = false;
            IsHidden = false;

            //EntityTypeEmailQueues = new List<EmailQueue>();
        }

    }

}
