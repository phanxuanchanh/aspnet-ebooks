using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SachDienTu.Startup))]
namespace SachDienTu
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
