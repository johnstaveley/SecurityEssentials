using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SecurityEssentials.Acceptance.Tests.Utility;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public abstract class BasePage
	{
		protected readonly WebDriverWait _wait;

		public string PageTitle { get; private set; }
		protected Uri BaseUri { get; private set; }
		protected IWebDriver Driver { get; private set; }
		protected TimeSpan WaitInterval { get; private set; }

		protected BasePage(IWebDriver webDriver, Uri baseUri, string pageTitle)
		{
			BaseUri = baseUri;
			Driver = webDriver;
			PageTitle = pageTitle;
			WaitInterval = new TimeSpan(0, 0, Convert.ToInt32(ConfigurationManager.AppSettings["WaitIntervalSeconds"]));
			_wait = new WebDriverWait(Driver, WaitInterval);
		}

		protected IWebElement GetVisibleWebElement(By by)
		{
			return _wait.Until(ExpectedConditions.ElementIsVisible(by));
		}

		public virtual bool IsCurrentPage
		{
			get
			{
				Repeater.DoOrTimeout(() =>
				{
					if (Driver.Title == PageTitle)
					{
						return true;
					}
					Console.WriteLine("Page title is {0} but expected {1}", Driver.Title, PageTitle);
					return false;
				}, new TimeSpan(0, 0, 120), new TimeSpan(0, 0, 1));
				return Driver.Title == PageTitle;
			}
		}

		public void ReloadPage()
		{
			Driver.Navigate().Refresh();
		}

		//Commented out code is result of breaking change on Driver.FindElements in Selenium 2.46
		//Commented out code is preferred is change is fixed.
		//Explicit reason for breaking change not known but commented code works with Selenium 2.42.
		public string GetErrorMessage(string errorFieldId)
		{

			var element = Driver.FindElement(By.Id(errorFieldId));
			return element != null ? element.Text : null;
		}

		public IAlert AlertPresent()
		{
			return ExpectedConditions.AlertIsPresent().Invoke(Driver);
		}


	}

}
