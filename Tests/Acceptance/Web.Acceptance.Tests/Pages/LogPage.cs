using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using SecurityEssentials.Model;
using System;
using System.Collections.Generic;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class LogPage : BasePage
	{
		public MenuBar MenuBar { get; }
		private IWebElement Grid => GetVisibleWebElement(By.Id("grid"));
		private bool GridDisplayed => Driver.FindElements(By.Id("grid")).Count > 0 && Driver.FindElements(By.Id("grid"))[0].Displayed;
		public LogPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.LOGS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}


		public List<Log> GetLogsDisplayed()
		{
			var data = new List<Log>();
			if (!GridDisplayed) return data;
			var rowCollection = Grid.FindElements(By.CssSelector("tr:not(.jqgfirstrow)"));
			foreach (var dataRow in rowCollection)
			{
				var columnCollection = dataRow.FindElements(By.TagName("td"));
				if (columnCollection.Count > 0)
				{
					data.Add(MapRow(columnCollection));
				}
			}
			return data;
		}

		private Log MapRow(IList<IWebElement> columnCollection)
		{
			var rowModel = new Log()
			{
				Level = columnCollection[1].Text.Trim(' '),
				Message = columnCollection[2].Text.Trim(' ')
			};
			return rowModel;
		}		
	}
}
