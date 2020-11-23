using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FlipWeb.Startup))]
namespace FlipWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
