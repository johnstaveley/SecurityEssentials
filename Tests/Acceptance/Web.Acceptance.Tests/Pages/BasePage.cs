using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SecurityEssentials.Acceptance.Tests.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public abstract class BasePage
	{
		protected readonly WebDriverWait Wait;

		public string PageTitle { get; }
		protected Uri BaseUri { get; private set; }
		protected IWebDriver Driver { get; }
		protected TimeSpan WaitInterval { get; }

		protected BasePage(IWebDriver webDriver, Uri baseUri, string pageTitle)
		{
			BaseUri = baseUri;
			Driver = webDriver;
			PageTitle = pageTitle;
			WaitInterval = new TimeSpan(0, 0, Convert.ToInt32(ConfigurationManager.AppSettings["WaitIntervalSeconds"]));
			Wait = new WebDriverWait(Driver, WaitInterval);
		}

		protected IWebElement GetVisibleWebElement(By by)
		{
			return Wait.Until(ExpectedConditions.ElementIsVisible(by));
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

		public List<string> ErrorSummary
		{
			get { return GetVisibleWebElement(By.ClassName("validation-summary-errors")).FindElements(By.TagName("li")).Select(a => a.Text).ToList(); }
		}

		public List<string> FieldErrors
		{
			get { return Driver.FindElements(By.ClassName("field-validation-error")).Select(a => a.Text).ToList(); }
		}

		public void ReloadPage()
		{
			Driver.Navigate().Refresh();
		}

	}

}
