using System;
using OpenQA.Selenium;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RecoverSuccessPage : BasePage
	{

		public RecoverSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.RECOVER_SUCCESS)
		{
		}

	}

}
