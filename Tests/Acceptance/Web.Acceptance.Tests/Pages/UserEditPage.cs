using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using System;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class UserEditPage : BasePage
	{

		public MenuBar MenuBar { get; }
		private IWebElement StatusMessage => GetVisibleWebElement(By.Id("statusMessage"));
		private IWebElement UserName => GetVisibleWebElement(By.Id("User_UserName"));
		private IWebElement Title => GetVisibleWebElement(By.Id("User_Title"));
		private IWebElement FirstName => GetVisibleWebElement(By.Id("User_FirstName"));
		private IWebElement LastName => GetVisibleWebElement(By.Id("User_LastName"));
		private IWebElement MakeAdminButton => GetVisibleWebElement(By.Id("makeAdmin"));
		private IWebElement RemoveAdminButton => GetVisibleWebElement(By.Id("removeAdmin"));
		private IWebElement TelNoWork => GetVisibleWebElement(By.Id("User_TelNoWork"));
		private IWebElement TelNoHome => GetVisibleWebElement(By.Id("User_TelNoHome"));
		private IWebElement TelNoMobile => GetVisibleWebElement(By.Id("User_TelNoMobile"));
		private IWebElement Town => GetVisibleWebElement(By.Id("User_Town"));
		private IWebElement PostCode => GetVisibleWebElement(By.Id("User_Postcode"));
		private IWebElement SkypeName => GetVisibleWebElement(By.Id("User_SkypeName"));
		private IWebElement EmailVerified => GetVisibleWebElement(By.Id("User_EmailVerified"));
		private IWebElement Approved => GetVisibleWebElement(By.Id("User_Approved"));
		private IWebElement DeleteUser => GetVisibleWebElement(By.Id("deleteUser"));
		private IWebElement Enabled => GetVisibleWebElement(By.Id("User_Enabled"));
		private IWebElement ResetPassword => GetVisibleWebElement(By.Id("resetPassword"));
		private IWebElement SubmitButton => GetVisibleWebElement(By.Id("submit"));
		public UserEditPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.USER_EDIT)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}
		public void ClickDeleteUser()
		{
			DeleteUser.Click();
		}
		public void ClickMakeAdmin()
		{
			MakeAdminButton.Click();
		}
		public void ClickRemoveAdmin()
		{
			RemoveAdminButton.Click();
		}
		public void ClickResetPassword()
		{
			ResetPassword.Click();
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
						throw new Exception($"Field {row[0]} not defined");
				}
			}
		}
		public bool GetApproved()
		{
			return Approved.Selected;
		}
		public bool GetEmailVerified()
		{
			return EmailVerified.Selected;
		}

		public bool GetEnabled()
		{
			return Enabled.Selected;
		}
		public string GetFirstName()
		{
			return FirstName.GetAttribute("value");
		}
		public bool GetIfTextIndicatingUserIsAnAdminExists => Driver.FindElements(By.Id("textIndicatingUserIsAnAdmin")).Count > 0;
		public string GetStatusMessage()
		{
			return StatusMessage.Text;
		}
		public string GetTelNoHome()
		{
			return TelNoHome.GetAttribute("value");
		}
		public string GetLastName()
		{
			return LastName.GetAttribute("value");
		}
		public string GetPostcode()
		{
			return PostCode.GetAttribute("value");
		}
		public string GetSkypeName()
		{
			return SkypeName.GetAttribute("value");
		}
		public string GetTelNoMobile()
		{
			return TelNoMobile.GetAttribute("value");
		}
		public string GetTelNoWork()
		{
			return TelNoWork.GetAttribute("value");
		}
		public string GetTown()
		{
			return Town.GetAttribute("value");
		}

		public bool IsUserNameEditable()
		{
			return UserName.TagName == "input";
		}	
		public string GetTitle()
		{
			return Title.GetAttribute("value");
		}

		public string GetUserName()
		{
			return (UserName.TagName == "input" ? UserName.GetAttribute("value") : UserName.Text);
		}
		public void ClickSubmit()
		{
			SubmitButton.Click();
		}
	}
}
