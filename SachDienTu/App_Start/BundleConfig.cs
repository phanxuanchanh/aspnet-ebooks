using System.Web.Optimization;

namespace SachDienTu
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/adminstyles").
                Include("~/front-end/admin/css/sb-admin-2.css"));

            bundles.Add(new StyleBundle("~/front-end/admin/vendor/fontawesome-free/css/all").
                Include("~/front-end/admin/vendor/fontawesome-free/css/all.css"));

            bundles.Add(new ScriptBundle("~/bundles/adminscripts").
                Include("~/front-end/admin/vendor/jquery/jquery.js",
                "~/front-end/admin/vendor/bootstrap/js/bootstrap.bundle.js",
                "~/front-end/admin/vendor/jquery-easing/jquery.easing.js",
                "~/front-end/admin/js/sb-admin-2.js"));

            bundles.Add(new StyleBundle("~/bundles/bookstyles")
                .Include("~/front-end/css/bootstrap.min.css",
                "~/front-end/css/owl.carousel.min.css",
                "~/front-end/css/styles.css"));

            bundles.Add(new ScriptBundle("~/bundles/bookscripts")
                .Include("~/front-end/js/jquery.min.js",
                "~/front-end/js/bootstrap.min.js",
                "~/front-end/js/owl.carousel.min.js",
                "~/front-end/js/custom.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
