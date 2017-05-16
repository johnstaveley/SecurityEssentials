using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class ChangeEmailAddressConfirmPage
    {
        public static HomePage NavigateToPage(IWebDriver webDriver, Uri baseUri, string newUserNameToken)
        {
            var userUri = new Uri(baseUri,
                $"Account/ChangeEmailAddressConfirm?NewEmailAddressToken={newUserNameToken}");
            webDriver.Navigate().GoToUrl(userUri);
            var homePage = new HomePage(webDriver, baseUri);
            PageFactory.InitElements(webDriver, homePage);
            return homePage;
        }
    }
}