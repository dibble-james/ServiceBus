namespace ServiceBus.Configuration
{
    using System;

    /// <summary>
    /// The host address configuration.
    /// </summary>
    internal class HostAddressConfiguration : IHostAddressConfiguration
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HostAddressConfiguration"/> class.
        /// </summary>
        /// <param name="hostAddress">The address the <see cref="IServiceBus"/> will be accessible from.</param>
        public HostAddressConfiguration(ILoggingConfiguration loggingConfiguration, Uri hostAddress)
        {
            this.HostAddress = hostAddress;
            this.LoggingConfiguration = loggingConfiguration;
        }

        /// <summary>
        /// Gets the address this <see cref="IServiceBus"/> will be accessible from.
        /// </summary>
        public Uri HostAddress
        {
            get; private set;
        }

        public ILoggingConfiguration LoggingConfiguration { get; private set; }
    }
}
