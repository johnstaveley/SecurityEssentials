using System;
using OpenQA.Selenium;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RecoverPasswordSuccessPage : BasePage
	{

		public RecoverPasswordSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.RECOVER_PASSWORD_SUCCESS)
		{
		}

	}

}
