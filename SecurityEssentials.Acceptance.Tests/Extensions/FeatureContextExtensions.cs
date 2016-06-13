using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	public static class FeatureContextExtensions
	{

		public static void SetWebDriver(this FeatureContext fc, IWebDriver webDriver)
		{
			fc.Set(webDriver);
		}

		public static bool HasWebDriver(this FeatureContext fc)
		{
			IWebDriver webDriver;
			return fc.TryGetValue(out webDriver);
		}

		public static IWebDriver GetWebDriver(this FeatureContext fc)
		{
			return fc.Get<IWebDriver>();
		}

		public static void SetBaseUri(this FeatureContext fc, Uri uri)
		{
			fc.Set(uri);
		}

		public static Uri GetBaseUri(this FeatureContext fc)
		{
			return fc.Get<Uri>();
		}
	}

}
