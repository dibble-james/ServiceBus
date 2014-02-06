namespace ServiceBus.Web.Mvc.Configuration
{
    using System.Web.Routing;
    using global::ServiceBus.Configuration;

    public static class ServiceBusBuilderExtensions
    {
        public static IHostApplicationConfiguration AsMvcServiceBus(this ITransportConfiguration transportConfiguration, RouteCollection routes)
        {
            return new HostApplicationConfiguration(transportConfiguration);
        }
    }
}
