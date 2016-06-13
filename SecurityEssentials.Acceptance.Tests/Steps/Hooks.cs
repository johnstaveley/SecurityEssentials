using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Principal;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using TechTalk.SpecFlow;
using SecurityEssentials.Acceptance.Tests.Web.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;

namespace SecurityEssentials.Acceptance.Tests.Web.Steps
{
	[Binding]
	public class Hooks
	{

		[BeforeFeature]
		public static void BeforeFeature()
		{

			var webDriver = new FirefoxDriver();
			webDriver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 5));
			FeatureContext.Current.SetWebDriver(webDriver);

			var baseUri = new Uri(ConfigurationManager.AppSettings["WebServerUrl"]);
			FeatureContext.Current.SetBaseUri(baseUri);

		}

		[AfterFeature]
		public static void AfterFeature()
		{
			if (FeatureContext.Current.HasWebDriver()) FeatureContext.Current.GetWebDriver().Quit();
		}
		
	}
}