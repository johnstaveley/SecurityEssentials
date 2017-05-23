using SecurityEssentials.Core.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.Model
{
	public class PreviousPassword
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		/// <summary>
		/// The algorithm to use to create the hash
		/// </summary>
		public HashStrategyKind HashStrategy { get; set; }
		/// <summary>
		/// A salt which is used to hash the password
		/// </summary>
		[MaxLength(500)]
		public string Salt { get; set; }
		/// <summary>
		/// A hashed version of the password, uses the salt
		/// </summary>
		[MaxLength(500)]
		public string Hash { get; set; }
		/// <summary>
		/// The date the password was active from (UTC)
		/// </summary>
		[Display(Name = "Active From Date")]
		public DateTime ActiveFromDateUtc { get; set; }

		public virtual User User { get; set; }
	}
}