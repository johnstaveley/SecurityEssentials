using System;
using System.Configuration;
using System.Drawing.Imaging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using TechTalk.SpecFlow;
using SecurityEssentials.Acceptance.Tests.Web.Extensions;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace SecurityEssentials.Acceptance.Tests.Web.Steps
{
	[Binding]
	public class Hooks
	{

		[BeforeFeature]
		public static void BeforeFeature()
		{

			var webBrowserProxy = ConfigurationManager.AppSettings["WebBrowserProxy"].ToString();
			IWebDriver webDriver = null;
			if (!string.IsNullOrEmpty(webBrowserProxy))
			{
				var proxy = new Proxy();
				proxy.HttpProxy = webBrowserProxy;
				proxy.FtpProxy = webBrowserProxy;
				proxy.SslProxy = webBrowserProxy;
				var capabilities = new DesiredCapabilities();
				capabilities.SetCapability(CapabilityType.Proxy, proxy);
				webDriver = new FirefoxDriver(capabilities);
			}
			else {
				webDriver = new FirefoxDriver();
			}
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

		[AfterScenario]
		public static void AfterScenario() {
			if (ScenarioContext.Current.TestError != null && Convert.ToBoolean(ConfigurationManager.AppSettings["TakeScreenShotOnFailure"]) == true) {
				string fileName = string.Format("{0}TestFailure-{1}.png", ConfigurationManager.AppSettings["TestScreenCaptureDirectory"].ToString(), DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));
				FeatureContext.Current.GetWebDriver().TakeScreenshot().SaveAsFile(fileName, ImageFormat.Png);
			}
		}


	}
}