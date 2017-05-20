using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using SecurityEssentials.Acceptance.Tests.Web.Extensions;
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
                var profile = new FirefoxProfile();
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

        [AfterScenario]
        public static void AfterScenario()
        {
            if (ScenarioContext.Current.TestError == null ||
                !Convert.ToBoolean(ConfigurationManager.AppSettings["TakeScreenShotOnFailure"])) return;
            var fileName =
                $"{ConfigurationManager.AppSettings["TestScreenCaptureDirectory"]}TestFailure-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.png";
            FeatureContext.Current.GetWebDriver().TakeScreenshot().SaveAsFile(fileName, ScreenshotImageFormat.Png);
        }
    }
}