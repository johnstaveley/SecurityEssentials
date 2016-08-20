using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Support.UI;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class UserEditPage : BasePage
	{

		public MenuBar MenuBar { get; private set; }

		private IWebElement StatusMessage
		{
			get { return this.GetVisibleWebElement(By.Id("statusMessage")); }
		}

		private IWebElement UserName
		{
			get { return this.GetVisibleWebElement(By.Id("User_UserName")); }
		}

		private IWebElement Title
		{
			get { return this.GetVisibleWebElement(By.Id("User_Title")); }
		}

		private IWebElement FirstName
		{
			get { return this.GetVisibleWebElement(By.Id("User_FirstName")); }
		}

		private IWebElement LastName
		{
			get { return this.GetVisibleWebElement(By.Id("User_LastName")); }
		}

		private IWebElement TelNoWork
		{
			get { return this.GetVisibleWebElement(By.Id("User_TelNoWork")); }
		}

		private IWebElement TelNoHome
		{
			get { return this.GetVisibleWebElement(By.Id("User_TelNoHome")); }
		}

		private IWebElement TelNoMobile
		{
			get { return this.GetVisibleWebElement(By.Id("User_TelNoMobile")); }
		}

		private IWebElement Town
		{
			get { return this.GetVisibleWebElement(By.Id("User_Town")); }
		}

		private IWebElement PostCode
		{
			get { return this.GetVisibleWebElement(By.Id("User_Postcode")); }
		}

		private IWebElement SkypeName
		{
			get { return this.GetVisibleWebElement(By.Id("User_SkypeName")); }
		}

		private IWebElement EmailVerified
		{
			get { return this.GetVisibleWebElement(By.Id("User_EmailVerified")); }
		}

		private IWebElement Approved
		{
			get { return this.GetVisibleWebElement(By.Id("User_Approved")); }
		}

		private IWebElement Enabled
		{
			get { return this.GetVisibleWebElement(By.Id("User_Enabled")); }
		}


		private IWebElement SubmitButton
		{
			get { return this.GetVisibleWebElement(By.Id("submit")); }
		}		

		public UserEditPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.USER_EDIT)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void EnterDetails(Table table)
		{

			foreach (var row in table.Rows)
			{
				switch (row[0].ToLower())
				{
					case "approved":
						var approved = Convert.ToBoolean(row[1]);
						if ((Approved.Selected && !approved) || (!Approved.Selected && approved)) { Approved.Click(); }
						break;
					case "emailverified":
						var emailVerified = Convert.ToBoolean(row[1]);
						if ((EmailVerified.Selected && !emailVerified) || (!EmailVerified.Selected && emailVerified)) { EmailVerified.Click(); }
						break;
					case "enabled":
						var enabled = Convert.ToBoolean(row[1]);
						if ((Enabled.Selected && !enabled) || (!Enabled.Selected && enabled)) { Enabled.Click(); }
						break;
					case "firstname":
						FirstName.Clear();
						FirstName.SendKeys(row[1]);
						break;
					case "lastname":
						LastName.Clear();
						LastName.SendKeys(row[1]);
						break;
					case "postcode":
						PostCode.Clear();
						PostCode.SendKeys(row[1]);
						break;
					case "skypename":
						SkypeName.Clear();
						SkypeName.SendKeys(row[1]);
						break;
					case "hometelephonenumber":
						TelNoHome.Clear();
						TelNoHome.SendKeys(row[1]);
						break;
					case "mobiletelephonenumber":
						TelNoMobile.Clear();
						TelNoMobile.SendKeys(row[1]);
						break;
					case "worktelephonenumber":
						TelNoWork.Clear();
						TelNoWork.SendKeys(row[1]);
						break;
					case "title":
						Title.Clear();
						Title.SendKeys(row[1]);
						break;
					case "town":
						Town.Clear();
						Town.SendKeys(row[1]);
						break;
					default:
						throw new Exception(string.Format("Field {0} not defined", row[0]));
				}
			}
		}

		public bool GetApproved()
		{
			return this.Approved.Selected;
		}

		public bool GetEmailVerified()
		{
			return this.EmailVerified.Selected;
		}

		public bool GetEnabled()
		{
			return this.Enabled.Selected;
		}

		public string GetFirstName()
		{
			return this.FirstName.GetAttribute("value");
		}

		public string GetLastName()
		{
			return this.LastName.GetAttribute("value");
		}

		public string GetMobileTelephoneNumber()
		{
			return this.TelNoMobile.GetAttribute("value");
		}

		public string GetTitle()
		{
			return this.Title.GetAttribute("value");
		}

		public string GetUserName()
		{
			return this.UserName.Text;
		}

		public void ClickSubmit()
		{
			SubmitButton.Click();
		}

		public string GetStatusMessage()
		{
			return StatusMessage.Text;
		}

	}
}
