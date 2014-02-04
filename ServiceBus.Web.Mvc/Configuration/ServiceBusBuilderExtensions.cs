namespace ServiceBus.Web.Mvc.Configuration
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using global::ServiceBus.Configuration;
    using global::ServiceBus.Transport.Http.Controllers;

    public static class ServiceBusBuilderExtensions
    {
        public static IHostApplicationConfiguration AsMvcServiceBus(this ITransportConfiguration transportConfiguration, RouteCollection routes)
        {
            routes.MapRoute(
                "Endpoints",
                "service-bus/peer/endpoints",
                new { controller = "Peer", action = "Endpoints" },
                new [] { typeof(PeerController).Namespace });

            return new HostApplicationConfiguration(transportConfiguration);
        }
    }
}
