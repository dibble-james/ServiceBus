namespace ServiceBus.Configuration
{
    using ServiceBus.Transport;

    /// <summary>
    /// The transport configuration.
    /// </summary>
    public class TransportConfiguration : HostAddressConfiguration, ITransportConfiguration
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TransportConfiguration"/> class.
        /// </summary>
        /// <param name="transportConfiguration">The previously set configuration for the transporter.</param>
        public TransportConfiguration(ITransportConfiguration transportConfiguration)
            : this(transportConfiguration.HostAddressConfigurationInstance, transportConfiguration.Transporter)
        {
            this.TransportConfigurationInstance = transportConfiguration;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="TransportConfiguration"/> class.
        /// </summary>
        /// <param name="hostAddress">The <see cref="IHostAddressConfiguration"/>.</param>
        /// <param name="transporter">The <see cref="ITransporter"/> for the <see cref="IServiceBus"/> to use.</param>
        public TransportConfiguration(IHostAddressConfiguration hostAddress, ITransporter transporter) 
            : base(hostAddress)
        {
            this.Transporter = transporter;
        }

        /// <summary>
        /// Gets the <see cref="ITransporter"/> instance to be used by the <see cref="IServiceBus"/>.
        /// </summary>
        public ITransporter Transporter { get; private set; }

        /// <summary>
        /// Gets the previously set configuration for the transporter.
        /// </summary>
        public ITransportConfiguration TransportConfigurationInstance { get; private set; }
    }
}
