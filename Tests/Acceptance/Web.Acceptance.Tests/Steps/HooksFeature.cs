using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Utility;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
	public class HooksFeature
	{

		[BeforeFeature]
		public static void BeforeFeature(FeatureContext featureContext)
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
							chromeOptions.AddArguments("--headless", "--disable-infobars", "--disable-extensions", "--test-type", "--allow-insecure-localhost", "--disable-gpu", "--no-sandbox");
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
			featureContext.SetWebDriver(webDriver);

			var baseUri = new Uri(ConfigurationManager.AppSettings["WebServerUrl"]);
			featureContext.SetBaseUri(baseUri);

		}

		[AfterFeature]
		public static void AfterFeature(FeatureContext featureContext)
		{
			if (featureContext.HasWebDriver()) featureContext.GetWebDriver().Quit();
		}

		[AfterTestRun]
		public static async Task AfterTestRun()
		{
			if (bool.Parse(ConfigurationManager.AppSettings["RestoreDatabaseAfterTests"]))
			{
				DatabaseCommand.Execute("SecurityEssentials.Acceptance.Tests.Resources.DatabaseTeardown.sql");
				DatabaseCommand.Execute("SecurityEssentials.Acceptance.Tests.Resources.LookupRestore.sql");
				DatabaseCommand.Execute("SecurityEssentials.Acceptance.Tests.Resources.DatabaseRestore.sql");
			}
			// Cleanup screenshot files after X days
			var numberOfDays = 3;
			var storage = ConfigurationManager.AppSettings["TestScreenCaptureStorage"];
			if (Convert.ToBoolean(ConfigurationManager.AppSettings["TakeScreenShotOnFailure"]))
			{
				Console.WriteLine($"Cleaning up screen captures older than {numberOfDays} days");
				if (storage.Contains("Endpoint"))
				{

					BlobServiceClient cloudBlobClient = new BlobServiceClient(storage);
					var cloudBlobContainer = cloudBlobClient.GetBlobContainerClient("selenium");
					await cloudBlobContainer.CreateIfNotExistsAsync();
					if (await cloudBlobContainer.ExistsAsync())
					{
						var results = cloudBlobContainer.GetBlobs(BlobTraits.None, BlobStates.All, null);
						foreach (var blobItem in results)
						{
							if (blobItem.Properties.LastModified.HasValue && blobItem.Properties.LastModified.Value.UtcDateTime < DateTime.UtcNow.AddDays(-numberOfDays))
							{
								await cloudBlobContainer.DeleteBlobAsync(blobItem.Name);
							}
						}
					}

				}
				else
				{
					if (Directory.Exists(storage))
					{
						var screenshots = Directory.GetFiles(storage, "*.png")
							.Select(a => new FileInfo(a))
							.Where(b => b.CreationTimeUtc < DateTime.UtcNow.AddDays(-numberOfDays))
							.ToList();
						foreach (var screenshot in screenshots)
						{
							screenshot.Delete();
						}
					}
				}
			}
		}


	}
}