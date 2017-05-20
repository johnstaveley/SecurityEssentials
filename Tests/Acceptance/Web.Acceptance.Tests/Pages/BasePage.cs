using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SecurityEssentials.Acceptance.Tests.Utility;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public abstract class BasePage
    {
        protected readonly WebDriverWait _wait;

        protected BasePage(IWebDriver webDriver, Uri baseUri, string pageTitle)
        {
            BaseUri = baseUri;
            Driver = webDriver;
            PageTitle = pageTitle;
            WaitInterval = new TimeSpan(0, 0, Convert.ToInt32(ConfigurationManager.AppSettings["WaitIntervalSeconds"]));
            _wait = new WebDriverWait(Driver, WaitInterval);
        }

        public string PageTitle { get; }
        protected Uri BaseUri { get; }
        protected IWebDriver Driver { get; }
        protected TimeSpan WaitInterval { get; }

        public virtual bool IsCurrentPage
        {
            get
            {
                Repeater.DoOrTimeout(() =>
                {
                    if (Driver.Title == PageTitle)
                        return true;
                    Console.WriteLine("Page title is {0} but expected {1}", Driver.Title, PageTitle);
                    return false;
                }, new TimeSpan(0, 0, 120), new TimeSpan(0, 0, 1));
                return Driver.Title == PageTitle;
            }
        }

        private IWebElement ErrorSummary => GetVisibleWebElement(By.ClassName("validation-summary-errors"));

        public List<string> Errors
        {
            get { return ErrorSummary.FindElements(By.TagName("li")).Select(a => a.Text).ToList(); }
        }

        private IWebElement LastAccountActivity => GetVisibleWebElement(By.Id("LastAccountActivity"));

        protected IWebElement GetVisibleWebElement(By by)
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        public void ReloadPage()
        {
            Driver.Navigate().Refresh();
        }
    }
}