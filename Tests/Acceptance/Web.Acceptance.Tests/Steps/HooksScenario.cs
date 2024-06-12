using Azure.Storage.Blobs;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
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
	public class HooksScenario
	{
		private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public HooksScenario(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
		[AfterScenario]
		public async Task TakeScreenShotIfInError()
		{
			if (_featureContext.HasWebDriver())
			{
				var webDriver = _featureContext.GetWebDriver();
				if (_scenarioContext.TestError != null && Convert.ToBoolean(ConfigurationManager.AppSettings["TakeScreenShotOnFailure"]))
				{
					var storage = ConfigurationManager.AppSettings["TestScreenCaptureStorage"];
					var filename = $"Failure-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
					Console.WriteLine($"Storing Screenshot of error with filename {filename}");
					if (!storage.Contains("Endpoint"))
					{
						string filePath = $"{storage}{filename}";
						webDriver.TakeScreenshot().SaveAsFile(filePath);
					}
					else
					{
						if (webDriver is ITakesScreenshot screenshotDriver)
						{
							Screenshot screenshot = screenshotDriver.GetScreenshot();
							BlobServiceClient  cloudBlobClient = new BlobServiceClient(storage);
							var cloudBlobContainer = cloudBlobClient.GetBlobContainerClient("selenium");
							await cloudBlobContainer.CreateIfNotExistsAsync();
							var cloudBlockBlob = cloudBlobContainer.GetBlobClient(filename);
							var memoryStream = new MemoryStream(screenshot.AsByteArray);
							memoryStream.Seek(0, SeekOrigin.Begin);
							await cloudBlockBlob.UploadAsync(memoryStream);
						}
					}
					TestContext.WriteLine("Failed on url " + webDriver.Url);
				}
			}
		}		

		[AfterScenario("CheckForErrors")]
		public void CheckForErrors()
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