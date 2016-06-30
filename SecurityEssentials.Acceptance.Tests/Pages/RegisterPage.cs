using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RegisterPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		private IWebElement Submit
		{
			get { return this.GetVisibleWebElement(By.Id("submit")); }
		}

        private IWebElement Username
		{
			get { return this.GetVisibleWebElement(By.Id("Username")); }
		}

        private IWebElement Email
		{
			get { return this.GetVisibleWebElement(By.Id("Email")); }
		}

        private IWebElement FirstName
		{
			get { return this.GetVisibleWebElement(By.Id("FirstName")); }
		}

        private IWebElement LastName
		{
			get { return this.GetVisibleWebElement(By.Id("LastName")); }
		}

        private IWebElement SecurityQuestion
		{
            get { return this.GetVisibleWebElement(By.Id("SecurityQuestionLookupItemId")); }
		}

        private IWebElement SecurityAnswer
		{
			get { return this.GetVisibleWebElement(By.Id("SecurityAnswer")); }
		}

        private IWebElement Password
		{
			get { return this.GetVisibleWebElement(By.Id("Password")); }
		}
        private IWebElement ConfirmPassword
		{
			get { return this.GetVisibleWebElement(By.Id("ConfirmPassword")); }
		}
        
		public RegisterPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.HOME)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void ClickSubmit()
		{
			Submit.Click();
		}

        public void EnterDetails(Table table)
        {

            foreach (var row in table.Rows)
            {
                switch (row[0])
                {
                    case "Username":
                        Username.SendKeys(row[1]);
                        break;
                    case "Email":
                        Email.SendKeys(row[1]);
                        break;
                    case "FirstName":
                        FirstName.SendKeys(row[1]);
                        break;
                    case "LastName":
                        LastName.SendKeys(row[1]);
                        break;
                    case "SecurityQuestion":
                        // todo
                        break;
                    case "SecurityAnswer":
                        SecurityAnswer.SendKeys(row[1]);
                        break;
                    case "Password":
                        Password.SendKeys(row[1]);
                        break;
                    case "ConfirmPassword":
                        ConfirmPassword.SendKeys(row[1]);
                        break;
                    default:
                        throw new Exception(string.Format("Field {0} not defined", row[0]));
                }         
            }             
                          
        }                 
                          
	}                     
                          
}
