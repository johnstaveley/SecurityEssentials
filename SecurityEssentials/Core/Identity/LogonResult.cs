using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityEssentials.Core.Identity
{
	public class LogonResult
	{

		public string UserName { get; set; }

		public bool Success { get; set; }

		public int FailedLogonAttemptCount { get; set; }

	}
}