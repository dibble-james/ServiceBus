namespace ServiceBus.Transport.Http.Configuration
{
    using System.Net.Http;

    using ServiceBus.Configuration;
    using ServiceBus.Messaging;

    /// <summary>
    /// Extension methods for <see cref="IHostAddressConfiguration"/> to define HTTP transportation.
    /// </summary>
    public static class ServiceBusBuilderExtensions
    {
        /// <summary>
        /// Use HTTP transportation for this <see cref="IServiceBus"/> using the default HTTP client.
        /// </summary>
        /// <param name="hostAddressConfiguration">The <see cref="IHostAddressConfiguration"/>.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        /// <returns>The <see cref="ITransportConfiguration"/>.</returns>
        public static ITransportConfiguration WithHttpTransport(this IHostAddressConfiguration hostAddressConfiguration, IMessageSerialiser messageSerialiser)
        {
            return hostAddressConfiguration.WithHttpTransport(new HttpClient(), messageSerialiser);
        }

        /// <summary>
        /// Use HTTP transportation for this <see cref="IServiceBus"/> using the a predefined HTTP client.
        /// </summary>
        /// <param name="hostAddressConfiguration">The <see cref="IHostAddressConfiguration"/>.</param>
        /// <param name="client">The HTTP client to use.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        /// <returns>The <see cref="ITransportConfiguration"/>.</returns>
        public static ITransportConfiguration WithHttpTransport(this IHostAddressConfiguration hostAddressConfiguration, HttpClient client, IMessageSerialiser messageSerialiser)
        {
            var transporter = new HttpTransporter(client, messageSerialiser);

            return new TransportConfiguration(hostAddressConfiguration, transporter);
        }
    }
}
