namespace ServiceBus.Transport.Http.Configuration
{
    using global::ServiceBus.Configuration;
    using System.Net.Http;

    public static class ServiceBusBuilderExtensions
    {
        public static ITransportConfiguration WithHttpTransport(this IHostAddressConfiguration hostAddressConfiguration)
        {
            return hostAddressConfiguration.WithHttpTransport(new HttpClient());
        }

        public static ITransportConfiguration WithHttpTransport(this IHostAddressConfiguration hostAddressConfiguration, HttpClient client)
        {
            var transporter = new HttpTransporter(client);

            return new TransportConfiguration(hostAddressConfiguration, transporter);
        }
    }
}
