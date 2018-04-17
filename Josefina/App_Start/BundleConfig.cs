using System.Web;
using System.Web.Optimization;

namespace Josefina
{
  public class BundleConfig
  {
    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    public static void RegisterBundles(BundleCollection bundles)
    {


      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js"));

      bundles.Add(new ScriptBundle("~/bundles/Gridmvc").Include(
        "~/Scripts/gridmvc.js"));

      bundles.Add(new ScriptBundle("~/bundles/JosefinaApp")
        .IncludeDirectory("~/AngularControllers", "*.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
        "~/Scripts/jquery-ui-{version}.js",
          "~/Scripts/Cultures/DatepickerCs.js"));

      bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
        "~/Scripts/Extensions/datepicker.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.validate*"));

      bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                  "~/Scripts/angular.js",
                  "~/Scripts/angular-ui-router.js",
                  "~/Scripts/angular-resource.js"));

      bundles.Add(new ScriptBundle("~/bundles/uiLayout").Include(
               "~/Scripts/angular-ui/ui-layout.js"));

      bundles.Add(new ScriptBundle("~/bundles/jstree").Include(
         "~/Scripts/jstree/jstree.js"));

      // Use the development version of Modernizr to develop with and learn from. Then, when you're
      // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                  "~/Scripts/modernizr-*"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

      bundles.Add(new StyleBundle("~/jstree").Include(
                "~/Content/jstree/style.css", new CssRewriteUrlTransform()));

      bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

      bundles.Add(new StyleBundle("~/uiLayout").Include(
               "~/Content/ui-layout.css"));


      bundles.Add(new StyleBundle("~/Gridmvc").Include(
                "~/Content/Gridmvc.css"));

      bundles.Add(new StyleBundle("~/jqueryUI").Include(
                "~/Content/themes/base/all.css"));

      // Set EnableOptimizations to false for debugging. For more information,
      // visit http://go.microsoft.com/fwlink/?LinkId=301862
      BundleTable.EnableOptimizations = true;
    }
  }
}
