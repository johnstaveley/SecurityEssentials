using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Extensions
{
	public static class FeatureContextExtensions
	{

		public const string LoginAttempts = "Login_Attempts";

		public static void SetWebDriver(this FeatureContext fc, IWebDriver webDriver)
		{
			fc.Set(webDriver);
		}

		public static bool HasWebDriver(this FeatureContext fc)
		{
            return fc.TryGetValue(out IWebDriver webDriver);
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

		private static List<DateTime> GetLoginAttempts(this FeatureContext featureContext)
		{
            if (!featureContext.TryGetValue(LoginAttempts, out List<DateTime> _))
			{
				SetLoginAttempts(featureContext, new List<DateTime>());
			}
			return featureContext.Get<List<DateTime>>(LoginAttempts);
		}

		public static void WaitForLoginAttempt(this FeatureContext featureContext)
		{
			List<DateTime> loginAttempts;
			do
			{
				Thread.Sleep(5000);
				loginAttempts = GetLoginAttempts(featureContext);
				Trace.WriteLine("Waiting 5 seconds for logon attempt");
			}
			while (loginAttempts.Count(a => a > DateTime.UtcNow.AddSeconds(-62)) >= 2);
			loginAttempts.Add(DateTime.UtcNow);
			SetLoginAttempts(featureContext, loginAttempts);
		}

		private static void SetLoginAttempts(this FeatureContext featureContext, List<DateTime> value)
		{
			featureContext.Set(value, LoginAttempts);
		}

	}

}
