namespace ServiceBus.Desktop
{
    using Owin;
    using System.Web.Http;

    public class HttpServiceBusServer
    {
        private static readonly string[] _namespaces = new[] { "ServiceBus.Transport.Http.Controllers" };

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                "Endpoints",
                "service-bus/peer/endpoints",
                new { controller = "Peer", action = "Endpoints" },
                HttpServiceBusServer._namespaces);

            appBuilder.UseWebApi(config);
        } 
    }
}
