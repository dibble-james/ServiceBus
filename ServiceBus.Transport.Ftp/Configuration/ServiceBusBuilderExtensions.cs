namespace ServiceBus.Transport.Ftp.Configuration
{
    using System;

    using ServiceBus.Configuration;
    using ServiceBus.Messaging;
    using ServiceBus.Transport.Ftp;

    /// <summary>
    /// Extension methods for <see cref="IHostAddressConfiguration"/> to define FTP transportation.
    /// </summary>
    public static class ServiceBusBuilderExtensions
    {
        /// <summary>
        /// Use FTP transportation for this <see cref="IServiceBus"/> using the default FTP client factory.
        /// </summary>
        /// <param name="hostAddressConfiguration">The <see cref="IHostAddressConfiguration"/>.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        /// <param name="pathToReciever">The full file path of the location this peers FTP server is mapped to receive messages.</param>
        /// <returns>The <see cref="ITransportConfiguration"/>.</returns>
        public static ITransportConfiguration WithFtpTransport(this IHostAddressConfiguration hostAddressConfiguration, IMessageSerialiser messageSerialiser, string pathToReciever)
        {
            return hostAddressConfiguration.WithFtpTransport(new FtpClientFactory(), messageSerialiser, pathToReciever);
        }

        /// <summary>
        /// Use FTP transportation for this <see cref="IServiceBus"/> using the a predefined FTP client factory.
        /// </summary>
        /// <param name="hostAddressConfiguration">The <see cref="IHostAddressConfiguration"/>.</param>
        /// <param name="clientFactory">The FTP client factory to use.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        /// <param name="pathToReciever">The full file path of the location this peers FTP server is mapped to receive messages.</param>
        /// <returns>The <see cref="ITransportConfiguration"/>.</returns>
        public static ITransportConfiguration WithFtpTransport(this IHostAddressConfiguration hostAddressConfiguration, IFtpClientFactory clientFactory, IMessageSerialiser messageSerialiser, string pathToReciever)
        {
            Argument.CannotBeNull(clientFactory, "clientFactory", "The FTP transporter cannot accept a null FTP Client Factory.");
            Argument.CannotBeNull(messageSerialiser, "messageSerialiser", "A message serialiser to be used by the transporter cannot be null.");
            Argument.CannotBe(pathToReciever, "pathToReciever", path => !string.IsNullOrEmpty(path));

            var transporter = new FtpTransporter(clientFactory, messageSerialiser, pathToReciever);

            return new TransportConfiguration(hostAddressConfiguration, transporter);
        }
    }
}
