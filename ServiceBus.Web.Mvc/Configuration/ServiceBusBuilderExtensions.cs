namespace ServiceBus.Web.Mvc.Configuration
{
    using System.Web.Mvc;
    using global::ServiceBus.Configuration;

    public static class ServiceBusBuilderExtensions
    {
        public static IServiceBusBuilder AsMvcServiceBus(this IServiceBusBuilder builder)
        {
            GlobalFilters.Filters.Add(new InterceptServiceBusRequestAttribute(builder.Build()));

            return builder;
        }
    }
}
