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
        public const string RegistrationAttempts = "Registration_Attempts";

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

        private static List<DateTime> GetRegistrationAttempts(this FeatureContext featureContext)
        {
            if (!featureContext.TryGetValue(RegistrationAttempts, out List<DateTime> _))
            {
                SetRegistrationAttempts(featureContext, new List<DateTime>());
            }
            return featureContext.Get<List<DateTime>>(RegistrationAttempts);
        }

        public static void WaitForRegistrationAttempt(this FeatureContext featureContext)
        {
            List<DateTime> registrationAttempts;
            do
            {
                Thread.Sleep(5000);
                registrationAttempts = GetRegistrationAttempts(featureContext);
                Trace.WriteLine("Waiting 5 seconds for registration attempt");
            }
            while (registrationAttempts.Count(a => a > DateTime.UtcNow.AddSeconds(-62)) >= 1);
            registrationAttempts.Add(DateTime.UtcNow);
            SetRegistrationAttempts(featureContext, registrationAttempts);
        }

        private static void SetRegistrationAttempts(this FeatureContext featureContext, List<DateTime> value)
        {
            featureContext.Set(value, RegistrationAttempts);
        }

	}

}
