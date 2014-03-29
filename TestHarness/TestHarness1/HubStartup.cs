using Microsoft.Owin;

using TestHarness1;

[assembly: OwinStartup(typeof(HubStartup))]
namespace TestHarness1
{
    using Owin;

    public class HubStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}