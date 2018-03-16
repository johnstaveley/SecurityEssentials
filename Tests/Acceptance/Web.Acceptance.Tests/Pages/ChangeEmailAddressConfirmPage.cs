using OpenQA.Selenium;
using System;
using PageFactory = SeleniumExtras.PageObjects.PageFactory;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class ChangeEmailAddressConfirmPage
	{

		public static HomePage NavigateToPage(IWebDriver webDriver, Uri baseUri, string newUserNameToken)
		{
			var userUri = new Uri(baseUri, $"Account/ChangeEmailAddressConfirmAsync?NewEmailAddressToken={newUserNameToken}");
			webDriver.Navigate().GoToUrl(userUri);
			var homePage = new HomePage(webDriver, baseUri);
			PageFactory.InitElements(webDriver, homePage);
			return homePage;
		}

	}

}
