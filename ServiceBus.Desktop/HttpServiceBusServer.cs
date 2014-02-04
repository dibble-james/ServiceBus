namespace ServiceBus.Desktop
{
    using System.Web.Http;

    using Owin;

    public class HttpServiceBusServer
    {
        private static readonly string[] _namespaces = new[] { "ServiceBus.Transport.Http.Controllers" };

        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                "Endpoints",
                "service-bus/peer/endpoints",
                new { controller = "Peer", action = "Endpoints" },
                _namespaces);
        } 
    }
}
