namespace ServiceBus.Web.Mvc.Configuration
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using global::ServiceBus.Configuration;

    public static class ServiceBusBuilderExtensions
    {
        private static readonly string[] _namespaces = new[] { "ServiceBus.Transport.Http.Controllers" };

        public static IHostApplicationConfiguration AsMvcServiceBus(this ITransportConfiguration transportConfiguration, RouteCollection routes)
        {
            routes.MapRoute(
                "Endpoints",
                "service-bus/peer/endpoints",
                new { controller = "Peer", action = "Endpoints" },
                ServiceBusBuilderExtensions._namespaces);

            return new HostApplicationConfiguration(transportConfiguration);
        }
    }
}
