using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using SecurityEssentials.Acceptance.Tests.Model;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class AccountLogPage : BasePage
	{
		public MenuBar MenuBar { get; }
		private IWebElement UserLogTable => GetVisibleWebElement(By.Id("userLogTable"));

		public AccountLogPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.ACCOUNT_LOG)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}	

		public List<DescriptionViewModel> GetUserLogsDisplayed()
		{
			var userLogs = new List<DescriptionViewModel>();
			var rowCollection = UserLogTable.FindElements(By.TagName("tr"));
			foreach (var dataRow in rowCollection)
			{
				var columnCollection = dataRow.FindElements(By.TagName("td"));
				if (columnCollection.Count > 0)
				{
					userLogs.Add(new DescriptionViewModel { Description = columnCollection[1].Text });
				}
			}
			return userLogs;
		}

	}

}
