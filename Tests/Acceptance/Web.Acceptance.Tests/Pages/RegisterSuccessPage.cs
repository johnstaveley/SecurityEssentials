using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RegisterSuccessPage : BasePage
	{
        
		public RegisterSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.REGISTER_SUCCESS)
		{
		}

                          
	}                     
                          
}
