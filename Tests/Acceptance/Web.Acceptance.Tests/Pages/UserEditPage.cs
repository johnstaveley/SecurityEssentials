using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class UserEditPage : BasePage
    {
        public UserEditPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.USER_EDIT)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement StatusMessage => GetVisibleWebElement(By.Id("statusMessage"));

        private IWebElement UserName => GetVisibleWebElement(By.Id("User_UserName"));

        private IWebElement Title => GetVisibleWebElement(By.Id("User_Title"));

        private IWebElement FirstName => GetVisibleWebElement(By.Id("User_FirstName"));

        private IWebElement LastName => GetVisibleWebElement(By.Id("User_LastName"));

        private IWebElement TelNoWork => GetVisibleWebElement(By.Id("User_TelNoWork"));

        private IWebElement TelNoHome => GetVisibleWebElement(By.Id("User_TelNoHome"));

        private IWebElement TelNoMobile => GetVisibleWebElement(By.Id("User_TelNoMobile"));

        private IWebElement Town => GetVisibleWebElement(By.Id("User_Town"));

        private IWebElement PostCode => GetVisibleWebElement(By.Id("User_Postcode"));

        private IWebElement SkypeName => GetVisibleWebElement(By.Id("User_SkypeName"));

        private IWebElement EmailVerified => GetVisibleWebElement(By.Id("User_EmailVerified"));

        private IWebElement Approved => GetVisibleWebElement(By.Id("User_Approved"));

        private IWebElement Enabled => GetVisibleWebElement(By.Id("User_Enabled"));


        private IWebElement SubmitButton => GetVisibleWebElement(By.Id("submit"));

        public void EnterDetails(Table table)
        {
            foreach (var row in table.Rows)
                switch (row[0].ToLower())
                {
                    case "approved":
                        var approved = Convert.ToBoolean(row[1]);
                        if (Approved.Selected && !approved || !Approved.Selected && approved) Approved.Click();
                        break;
                    case "emailverified":
                        var emailVerified = Convert.ToBoolean(row[1]);
                        if (EmailVerified.Selected && !emailVerified || !EmailVerified.Selected && emailVerified)
                            EmailVerified.Click();
                        break;
                    case "enabled":
                        var enabled = Convert.ToBoolean(row[1]);
                        if (Enabled.Selected && !enabled || !Enabled.Selected && enabled) Enabled.Click();
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

        public string GetLastName()
        {
            return LastName.GetAttribute("value");
        }

        public string GetMobileTelephoneNumber()
        {
            return TelNoMobile.GetAttribute("value");
        }

        public string GetTitle()
        {
            return Title.GetAttribute("value");
        }

        public string GetUserName()
        {
            return UserName.Text;
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