using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using SecurityEssentials.Acceptance.Tests.Web.Extensions;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Steps
{
    [Binding]
    public class Hooks
    {
        [BeforeFeature]
        public static void BeforeFeature()
        {
            var webBrowserProxy = ConfigurationManager.AppSettings["WebBrowserProxy"];
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
            else
            {
                webDriver = new FirefoxDriver();
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

        [AfterScenario]
        public static void AfterScenario()
        {
            if (ScenarioContext.Current.TestError != null &&
                Convert.ToBoolean(ConfigurationManager.AppSettings["TakeScreenShotOnFailure"]))
            {
                var fileName =
                    $"{ConfigurationManager.AppSettings["TestScreenCaptureDirectory"]}TestFailure-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.png";
                FeatureContext.Current.GetWebDriver().TakeScreenshot().SaveAsFile(fileName, ScreenshotImageFormat.Png);
            }
        }
    }
}