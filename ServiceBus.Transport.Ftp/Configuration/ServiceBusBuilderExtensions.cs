namespace ServiceBus.Transport.Http.Configuration
{
    using System;
    using System.Net.FtpClient;

    using ServiceBus.Configuration;
    using ServiceBus.Messaging;
    using ServiceBus.Transport.Ftp;

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
        public static ITransportConfiguration WithFtpTransport(this IHostAddressConfiguration hostAddressConfiguration, IMessageSerialiser messageSerialiser)
        {
            return hostAddressConfiguration.WithFtpTransport(new FtpClient(), messageSerialiser);
        }

        /// <summary>
        /// Use HTTP transportation for this <see cref="IServiceBus"/> using the a predefined HTTP client.
        /// </summary>
        /// <param name="hostAddressConfiguration">The <see cref="IHostAddressConfiguration"/>.</param>
        /// <param name="client">The HTTP client to use.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        /// <returns>The <see cref="ITransportConfiguration"/>.</returns>
        public static ITransportConfiguration WithFtpTransport(this IHostAddressConfiguration hostAddressConfiguration, FtpClient client, IMessageSerialiser messageSerialiser)
        {
            Argument.CannotBeNull(client, "client", "The FTP transporter cannot accept a null FTP Client.");
            Argument.CannotBeNull(messageSerialiser, "messageSerialiser", "A message serialiser to be used by the transporter cannot be null.");

            var transporter = new FtpTransporter(client, messageSerialiser);

            return new TransportConfiguration(hostAddressConfiguration, transporter);
        }
    }
}
