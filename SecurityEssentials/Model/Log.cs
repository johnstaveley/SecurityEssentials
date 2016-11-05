using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml;

namespace SecurityEssentials.Model
{
	/// <summary>
	/// Table for serilog
	/// </summary>
	public class Log
	{

		public int Id { get; set; }
		
		public string Message { get; set; }

		public string MessageTemplate { get; set; }

		[StringLength(128)]
		public string Level { get; set; }
		
		public DateTimeOffset TimeStamp { get; set; }

		public string Exception { get; set; }

		[Column(TypeName = "xml")]
		public string Properties { get; set; }
		
		public string LogEvent { get; set; }

	}
}