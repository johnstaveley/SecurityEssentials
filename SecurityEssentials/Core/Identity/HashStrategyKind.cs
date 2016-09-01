using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityEssentials.Core.Identity
{
	public enum HashStrategyKind
	{
		PBKDF2_5009Iterations = 0,
		PBKDF2_8000Iterations = 1
	}
}