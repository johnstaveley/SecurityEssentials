using Microsoft.Owin;
using Owin;
using SecurityEssentials.App_Start;

[assembly: OwinStartup(typeof(Startup))]

namespace SecurityEssentials.App_Start
{
    public partial class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            ConfigureAuthentication(app);
        }
    }
}