using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Awpbs.Web.App.Startup))]
namespace Awpbs.Web.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
