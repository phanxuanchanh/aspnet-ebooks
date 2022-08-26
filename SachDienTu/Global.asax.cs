using SachDienTu.App_Start;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SachDienTu
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application.Lock();
            Application["Online"] = 0;
            Application.UnLock();
        }

        protected void Session_Start()
        {
            Application.Lock();
            int count = (int)Application["Online"];
            Application["Online"] = ++count;
            Application.UnLock();
        }


        protected void Session_End()
        {
            Application.Lock();
            int count = (int)Application["Online"];
            Application["Online"] = --count;
            Application.UnLock();
        }

        protected void Application_End()
        {

        }
    }
}
