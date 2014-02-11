namespace ServiceBus.Configuration
{
    using ServiceBus.Transport;

    /// <summary>
    /// The transportation configuration.
    /// </summary>
    public interface ITransportConfiguration
    {
        /// <summary>
        /// Gets the configuration for the host address.
        /// </summary>
        IHostAddressConfiguration HostAddressConfiguration { get; }

        /// <summary>
        /// Gets the <see cref="ITransporter"/> instance to be used by the <see cref="IServiceBus"/>.
        /// </summary>
        ITransporter Transporter { get; }
    }
}
