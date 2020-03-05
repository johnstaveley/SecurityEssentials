using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.Extensions;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Utility;
using System;
using System.Configuration;
using System.IO;
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
			var timeoutMinutes = int.Parse(ConfigurationManager.AppSettings["TimeoutMinutes"]);
			var waitIntervalSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["WaitIntervalSeconds"]);
			IWebDriver webDriver;
			if (!string.IsNullOrEmpty(webBrowserProxy))
			{
				var firefoxOptions = new FirefoxOptions
				{ 
					Proxy = new Proxy
					{
						HttpProxy = webBrowserProxy,
						FtpProxy = webBrowserProxy,
						SslProxy = webBrowserProxy
					}
				};
				webDriver = new FirefoxDriver(firefoxOptions);
			}
			else
			{
				switch (webBrowserType)
				{
					case "Chrome":
					case "Headless Chrome":
				        ChromeOptions chromeOptions = new ChromeOptions();
						if (webBrowserType == "Headless Chrome")
						{
							chromeOptions.AddArguments("--headless", "--disable-infobars", "--disable-extensions", "--test-type", "--allow-insecure-localhost");
						}
						webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromMinutes(timeoutMinutes));
				        break;
					case "FireFox":
						webDriver = new FirefoxDriver();
						break;
					case "IE":
					case "Internet Explorer":
						webDriver = new InternetExplorerDriver();
						break;
					default:
						throw new Exception($"Unable to set browser type {webBrowserType}");
				}
			}
			webDriver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, waitIntervalSeconds);
			webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, waitIntervalSeconds * 3);
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
			if (Convert.ToBoolean(ConfigurationManager.AppSettings["TakeScreenShotOnFailure"]) && Directory.Exists(ConfigurationManager.AppSettings["TestScreenCaptureDirectory"]))
			{
				var screenshots = Directory.GetFiles(ConfigurationManager.AppSettings["TestScreenCaptureDirectory"], "*.png")
					.Select(a => new FileInfo(a))
					.Where(b => b.CreationTimeUtc < DateTime.UtcNow.AddDays(-1))
					.ToList();
				foreach (var screenshot in screenshots)
				{
					screenshot.Delete();
				}
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
			var appSensorErrors = SeDatabase.GetAppSensorErrors();
			Assert.That(appSensorErrors.Count, Is.EqualTo(0), $"Expected No errors but found appSensor error(s) {string.Join(", ", appSensorErrors.Select(a => a.Message).ToArray())}");
			var cspWarnings = SeDatabase.GetCspWarnings();
			Assert.That(cspWarnings.Count, Is.EqualTo(0), $"Expected No Csp Warnings but found warnings(s) {string.Join(", ", cspWarnings.Select(a => a.Message).ToArray())}");
		}

	}
}