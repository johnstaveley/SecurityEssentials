using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class UtilitySteps
	{

		[Given(@"I wait (.*) seconds")]
		public void GivenIWaitSeconds(int waitInSeconds)
		{
			Thread.Sleep(waitInSeconds * 1000);
		}

	}
}
