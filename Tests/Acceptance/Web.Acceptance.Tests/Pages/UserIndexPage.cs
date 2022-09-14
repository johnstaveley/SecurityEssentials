using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using SecurityEssentials.Acceptance.Tests.Model;
using System;
using System.Collections.Generic;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class UserIndexPage : BasePage
	{
		public MenuBar MenuBar { get; }

		private IWebElement EditButton(string text)
		{
			var row = UserTable.FindElement(By.XPath($"//tr[td//text()[contains(., '{text}')]]"));
			return row.FindElement(By.CssSelector("a.editUser"));
		}
		private IWebElement UserTable => GetVisibleWebElement(By.Id("userGrid"));
		public UserIndexPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.USERS_INDEX)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void ClickEditUserWithName(string text)
		{
			// Click the row with the right text
			EditButton(text).Click();
		}
		public List<UserModel> GetUsersDisplayed()
		{
			var usersDisplayed = new List<UserModel>();
			var rowCollection = UserTable.FindElements(By.CssSelector("tr"));
			foreach (var dataRow in rowCollection)
			{
				var columnCollection = dataRow.FindElements(By.TagName("td"));
				if (columnCollection.Count > 0)
				{
					usersDisplayed.Add(MapRow(columnCollection));
				}
			}
			return usersDisplayed;
		}

		private UserModel MapRow(IList<IWebElement> columnCollection)
		{
			var model = new UserModel
			{
				UserName = columnCollection[1].Text.Trim(' '),
				FullName = columnCollection[0].Text.Trim(' '),
				TelNoMobile = columnCollection[2].Text.Trim(' '),
				Enabled = columnCollection[3].Text == "Yes",
				Approved = columnCollection[4].Text == "Yes"
			};
			return model;
		}
	}

}
