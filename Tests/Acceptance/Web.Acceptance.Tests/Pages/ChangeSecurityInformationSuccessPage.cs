using OpenQA.Selenium;
using System;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class ChangeSecurityInformationSuccessPage : BasePage
	{
		public MenuBar MenuBar { get; }

		public ChangeSecurityInformationSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_SECURITY_INFORMATION_SUCCESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

	}


}
