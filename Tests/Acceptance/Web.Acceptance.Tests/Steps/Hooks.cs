using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Utility;
using System;
using System.Configuration;
using System.Linq;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class Hooks
	{

		[BeforeFeature]
		public static void BeforeFeature()
		{

			var webBrowserProxy = ConfigurationManager.AppSettings["WebBrowserProxy"];
			var webBrowserType = ConfigurationManager.AppSettings["WebBrowserType"];
			IWebDriver webDriver;
			if (!string.IsNullOrEmpty(webBrowserProxy))
			{
				FirefoxProfile profile = new FirefoxProfile();
				var proxy = new Proxy
				{
					HttpProxy = webBrowserProxy,
					FtpProxy = webBrowserProxy,
					SslProxy = webBrowserProxy
				};
				profile.SetProxyPreferences(proxy);
				webDriver = new FirefoxDriver(profile);
			}
			else
			{
				switch (webBrowserType)
				{
					case "Chrome":
						webDriver = new ChromeDriver();
						break;
					case "FireFox":
						webDriver = new FirefoxDriver();
						break;
					case "IE":
					case "Internet Explorer":
						webDriver = new InternetExplorerDriver();
						break;
					case "PhantomJS":
						webDriver = new PhantomJSDriver();
						break;
					default:
						throw new Exception($"Unable to set browser type {webBrowserType}");
				}
			}
			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
			FeatureContext.Current.SetWebDriver(webDriver);

			var baseUri = new Uri(ConfigurationManager.AppSettings["WebServerUrl"]);
			FeatureContext.Current.SetBaseUri(baseUri);

		}

		[AfterFeature]
		public static void AfterFeature()
		{
			if (FeatureContext.Current.HasWebDriver()) FeatureContext.Current.GetWebDriver().Quit();
		}

		[AfterTestRun]
		public static void AfterTestRun()
		{
			if (bool.Parse(ConfigurationManager.AppSettings["RestoreDatabaseAfterTests"]))
			{
				DatabaseCommand.Execute("SecurityEssentials.Acceptance.Tests.Resources.DatabaseTeardown.sql");
				DatabaseCommand.Execute("SecurityEssentials.Acceptance.Tests.Resources.LookupRestore.sql");
				DatabaseCommand.Execute("SecurityEssentials.Acceptance.Tests.Resources.DatabaseRestore.sql");
			}
		}

		[AfterScenario]
		public static void TakeScreenShotIfInError()
		{
			if (ScenarioContext.Current.TestError != null && Convert.ToBoolean(ConfigurationManager.AppSettings["TakeScreenShotOnFailure"]))
			{
				string fileName = $"{ConfigurationManager.AppSettings["TestScreenCaptureDirectory"]}TestFailure-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.png";
				FeatureContext.Current.GetWebDriver().TakeScreenshot().SaveAsFile(fileName, ScreenshotImageFormat.Png);
			}
		}		

		[AfterScenario("CheckForErrors")]
		public static void CheckForErrors()
		{
			var errors = SeDatabase.GetSystemErrors();
			Assert.That(errors.Count, Is.EqualTo(0), $"Expected No errors but found error(s) {string.Join(", ", errors.Select(a => a.Message).ToArray())}");
		}

	}
}