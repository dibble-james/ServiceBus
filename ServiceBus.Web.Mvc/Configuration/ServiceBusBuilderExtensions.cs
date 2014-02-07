namespace ServiceBus.Web.Mvc.Configuration
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Transport.Http.Controllers;

    using global::ServiceBus.Configuration;

    public static class ServiceBusBuilderExtensions
    {
        public static IHostApplicationConfiguration AsMvcServiceBus(this ITransportConfiguration transportConfiguration, RouteCollection routes)
        {
            routes.MapRoute(
                "MessageReceive", 
                "service-bus/message", 
                new { controller = "Message", action = "Receive" },
                new string[] { typeof(MessageController).Namespace });

            return new HostApplicationConfiguration(transportConfiguration);
        }
    }
}
