namespace ServiceBus.Configuration
{
    using ServiceBus.Transport;

    /// <summary>
    /// The transportation configuration.
    /// </summary>
    public interface ITransportConfiguration : IHostAddressConfiguration
    {
        /// <summary>
        /// Gets the <see cref="ITransporter"/> instance to be used by the <see cref="IServiceBus"/>.
        /// </summary>
        ITransporter Transporter { get; }

        /// <summary>
        /// Gets the previously set configuration for the transporter.
        /// </summary>
        ITransportConfiguration TransportConfigurationInstance { get; }
    }
}
