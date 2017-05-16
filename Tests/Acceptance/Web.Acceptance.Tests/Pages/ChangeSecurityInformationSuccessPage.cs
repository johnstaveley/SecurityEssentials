﻿using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class ChangeSecurityInformationSuccessPage : BasePage
    {
        public ChangeSecurityInformationSuccessPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.CHANGE_SECURITY_INFORMATION_SUCCESS)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }
    }
}