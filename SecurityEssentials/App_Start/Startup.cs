using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SecurityEssentials.App_Start.Startup))]
namespace SecurityEssentials.App_Start
{
    public partial class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            ConfigureAuthorisation(app);
        }
    }
}
