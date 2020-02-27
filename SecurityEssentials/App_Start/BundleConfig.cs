using System;
using System.Web.Optimization;

namespace SecurityEssentials
{
    public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			if (bundles == null) throw new ArgumentNullException(nameof(bundles));

			// Clear all items from the default ignore list to allow minified CSS and JavaScript files to be included in debug mode
			bundles.IgnoreList.Clear();

			// Add back the default ignore list rules sans the ones which affect minified files and debug mode
			bundles.IgnoreList.Ignore("*.intellisense.js");
			bundles.IgnoreList.Ignore("*-vsdoc.js");
			bundles.IgnoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);

			bundles.Add(new ScriptBundle("~/bundles/antiforgerytoken").Include("~/Scripts/app/antiforgerytoken.js"));
			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));
			bundles.Add(new ScriptBundle("~/bundles/kendoui").Include(
				"~/Scripts/kendo/2014.1.318/kendo.web.min.js",
				"~/Scripts/kendo/2014.1.318/cultures/kendo.culture.en-GB.min.js"
				));
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
			bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-{version}.js"));
			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.unobtrusive*",
						"~/Scripts/jquery.validate*"));
			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
			bundles.Add(new ScriptBundle("~/bundles/quickfind").Include("~/Scripts/app/quickfind.js"));
			bundles.Add(new ScriptBundle("~/bundles/user").Include("~/Scripts/app/user.js"));
			bundles.Add(new ScriptBundle("~/bundles/users").Include("~/Scripts/app/users.js"));


			// CSS Styles
			bundles.Add(new StyleBundle("~/Content/css").Include(
				    "~/Content/bootstrap.css",
                    "~/Content/site.css"
				));
			bundles.Add(new StyleBundle("~/Content/kendo/2014.1.318/kendoui").Include(
				"~/Content/kendo/2014.1.318/kendo.common.min.css",
				"~/Content/kendo/2014.1.318/kendo.default.min.css")); 

			bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
						"~/Content/themes/base/core.css",
						"~/Content/themes/base/resizable.css",
						"~/Content/themes/base/selectable.css",
						"~/Content/themes/base/accordion.css",
						"~/Content/themes/base/autocomplete.css",
						"~/Content/themes/base/button.css",
						"~/Content/themes/base/dialog.css",
						"~/Content/themes/base/slider.css",
						"~/Content/themes/base/tabs.css",
						"~/Content/themes/base/datepicker.css",
						"~/Content/themes/base/progressbar.css",
						"~/Content/themes/base/theme.css"));
		}
	}
}