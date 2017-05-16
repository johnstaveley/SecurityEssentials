using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
    public static class FeatureContextExtensions
    {
        public const string LOGIN_ATTEMPTS = "Login_Attempts";

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

        private static List<DateTime> GetLoginAttempts(this FeatureContext featureContext)
        {
            List<DateTime> loginAttempts = null;
            if (!featureContext.TryGetValue(LOGIN_ATTEMPTS, out loginAttempts))
                SetLoginAttempts(featureContext, new List<DateTime>());
            return featureContext.Get<List<DateTime>>(LOGIN_ATTEMPTS);
        }

        public static void WaitForLoginAttempt(this FeatureContext featureContext)
        {
            var loginAttempts = GetLoginAttempts(featureContext);
            do
            {
                Thread.Sleep(5000);
                loginAttempts = GetLoginAttempts(featureContext);
                Trace.WriteLine("Waiting 5 seconds for logon attempt");
            } while (loginAttempts.Where(a => a > DateTime.UtcNow.AddSeconds(-62)).Count() >= 2);
            loginAttempts.Add(DateTime.UtcNow);
            SetLoginAttempts(featureContext, loginAttempts);
        }

        private static void SetLoginAttempts(this FeatureContext featureContext, List<DateTime> value)
        {
            featureContext.Set(value, LOGIN_ATTEMPTS);
        }
    }
}