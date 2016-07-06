using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Support.UI;

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
			get { return this.GetVisibleWebElement(By.Id("User_UserName")); }
		}

        private IWebElement FirstName
		{
			get { return this.GetVisibleWebElement(By.Id("User_FirstName")); }
		}

        private IWebElement LastName
		{
			get { return this.GetVisibleWebElement(By.Id("User_LastName")); }
		}

        private SelectElement SecurityQuestion
		{
			get { return new SelectElement(this.GetVisibleWebElement(By.Id("User_SecurityQuestionLookupItemId"))); }
		}

        private IWebElement SecurityAnswer
		{
			get { return this.GetVisibleWebElement(By.Id("User_SecurityAnswer")); }
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
			: base(webDriver, baseUri, PageTitles.REGISTER)
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
                switch (row[0].ToLower())
                {
                    case "username":
                        Username.SendKeys(row[1]);
                        break;
                    case "firstname":
                        FirstName.SendKeys(row[1]);
                        break;
                    case "lastname":
                        LastName.SendKeys(row[1]);
                        break;
                    case "securityquestion":
						SecurityQuestion.SelectByText(row[1]);
                        break;
                    case "securityanswer":
                        SecurityAnswer.SendKeys(row[1]);
                        break;
                    case "password":
                        Password.SendKeys(row[1]);
                        break;
                    case "confirmpassword":
                        ConfirmPassword.SendKeys(row[1]);
                        break;
                    default:
                        throw new Exception(string.Format("Field {0} not defined", row[0]));
                }         
            }             
                          
        }                 
                          
	}                     
                          
}
