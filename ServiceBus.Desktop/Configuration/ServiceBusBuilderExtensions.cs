namespace ServiceBus.Transport.Http.Configuration
{
    using global::ServiceBus.Configuration;
    using System.Net.Http;

    public static class ServiceBusBuilderExtensions
    {
        public static IServiceBusBuilder WithHttpTransport(this IServiceBusBuilder builder)
        {
            return builder.WithHttpTransport(new HttpClient());
        }

        public static IServiceBusBuilder WithHttpTransport(this IServiceBusBuilder builder, HttpClient client)
        {
            builder.WithTransport(new HttpTransporter(client));

            return builder;
        }
    }
}
