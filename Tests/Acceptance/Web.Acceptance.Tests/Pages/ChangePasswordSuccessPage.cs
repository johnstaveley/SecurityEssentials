﻿using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class ChangePasswordSuccessPage : BasePage
    {
        public ChangePasswordSuccessPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.CHANGE_PASSWORD_SUCCESS)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }
    }
}