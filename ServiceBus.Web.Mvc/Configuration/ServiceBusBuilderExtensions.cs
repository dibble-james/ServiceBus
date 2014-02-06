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
                "MessageRecieve", 
                "service-bus/message", 
                new { controller = "Message", action = "Recieve" },
                new string[] { typeof(MessageController).Namespace });

            return new HostApplicationConfiguration(transportConfiguration);
        }
    }
}
