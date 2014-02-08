namespace ServiceBus.Transport.Http.Configuration
{
    using System.Net.Http;

    using ServiceBus.Configuration;
    using ServiceBus.Messaging;

    public static class ServiceBusBuilderExtensions
    {
        public static ITransportConfiguration WithHttpTransport(this IHostAddressConfiguration hostAddressConfiguration, IMessageSerialiser messageSerialiser)
        {
            return hostAddressConfiguration.WithHttpTransport(new HttpClient(), messageSerialiser);
        }

        public static ITransportConfiguration WithHttpTransport(this IHostAddressConfiguration hostAddressConfiguration, HttpClient client, IMessageSerialiser messageSerialiser)
        {
            var transporter = new HttpTransporter(client, messageSerialiser);

            return new TransportConfiguration(hostAddressConfiguration, transporter);
        }
    }
}
