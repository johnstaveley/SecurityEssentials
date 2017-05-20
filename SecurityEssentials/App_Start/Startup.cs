using Microsoft.Owin;
using Owin;
using SecurityEssentials;

[assembly: OwinStartup(typeof(Startup))]

namespace SecurityEssentials
{
    public partial class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            ConfigureAuthentication(app);
        }
    }
}