using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShopDTDD.Startup))]
namespace ShopDTDD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
