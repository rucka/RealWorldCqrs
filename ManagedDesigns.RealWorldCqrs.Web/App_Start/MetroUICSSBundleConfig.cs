using System.Web.Optimization;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(ManagedDesigns.RealWorldCqrs.Web.App_Start.MetroUICSSBundleConfig), "RegisterBundles")]

namespace ManagedDesigns.RealWorldCqrs.Web.App_Start
{
	public class MetroUICSSBundleConfig
	{
		public static void RegisterBundles()
		{
			// Add @Styles.Render("~/Content/metroui") in the <head/> of your _Layout.cshtml view
			// Add @Scripts.Render("~/bundles/metroui") after jQuery in your _Layout.cshtml view
			// When <compilation debug="true" />, ASP.Net will render the full readable version. When set to <compilation debug="false" />, the minified version will be rendered automatically
			BundleTable.Bundles.Add(new ScriptBundle("~/bundles/metroui").Include("~/Scripts/metroui/accordion.js", "~/Scripts/metroui/buttonset.js", "~/Scripts/metroui/calendar.js", "~/Scripts/metroui/carousel.js", "~/Scripts/metroui/dialog.js", "~/Scripts/metroui/dropdown.js", "~/Scripts/metroui/input-control.js", "~/Scripts/metroui/pagecontrol.js", "~/Scripts/metroui/rating.js", "~/Scripts/metroui/slider.js", "~/Scripts/metroui/start-menu.js", "~/Scripts/metroui/tile-drag.js", "~/Scripts/metroui/tile-slider.js"));
			BundleTable.Bundles.Add(new StyleBundle("~/Content/metroui").Include("~/Content/metroui/css/modern.css", "~/Content/metroui/css/modern-responsive.css"));
		}
	}
}
