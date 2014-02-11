namespace ServiceBus.Configuration
{
    using ServiceBus.Transport;

    /// <summary>
    /// The transport configuration.
    /// </summary>
    public class TransportConfiguration : ITransportConfiguration
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TransportConfiguration"/> class.
        /// </summary>
        /// <param name="hostAddress">The <see cref="IHostAddressConfiguration"/>.</param>
        /// <param name="transporter">The <see cref="ITransporter"/> for the <see cref="IServiceBus"/> to use.</param>
        public TransportConfiguration(IHostAddressConfiguration hostAddress, ITransporter transporter)
        {
            this.HostAddressConfiguration = hostAddress;
            this.Transporter = transporter;
        }

        /// <summary>
        /// Gets the configuration for the host address.
        /// </summary>
        public IHostAddressConfiguration HostAddressConfiguration { get; private set; }

        /// <summary>
        /// Gets the <see cref="ITransporter"/> instance to be used by the <see cref="IServiceBus"/>.
        /// </summary>
        public ITransporter Transporter { get; private set; }
    }
}
