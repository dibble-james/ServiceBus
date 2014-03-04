namespace ServiceBus.Configuration
{
    using System;

    /// <summary>
    /// The host address configuration.
    /// </summary>
    public class HostAddressConfiguration : LoggingConfiguration, IHostAddressConfiguration
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HostAddressConfiguration"/> class.
        /// </summary>
        /// <param name="hostAddressConfiguration">The previously set configuration of the host address.</param>
        public HostAddressConfiguration(IHostAddressConfiguration hostAddressConfiguration) 
            : this(hostAddressConfiguration.LoggingConfigurationInstance, hostAddressConfiguration.HostAddress)
        {
            this.HostAddressConfigurationInstance = hostAddressConfiguration;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="HostAddressConfiguration"/> class.
        /// </summary>
        /// <param name="loggingConfiguration">Previously set logging information.</param>
        /// <param name="hostAddress">The address the <see cref="IServiceBus"/> will be accessible from.</param>
        public HostAddressConfiguration(ILoggingConfiguration loggingConfiguration, Uri hostAddress) 
            : base(loggingConfiguration)
        {
            this.HostAddress = hostAddress;
        }

        /// <summary>
        /// Gets the address this <see cref="IServiceBus"/> will be accessible from.
        /// </summary>
        public Uri HostAddress
        {
            get; private set;
        }

        /// <summary>
        /// Gets the previously set configuration for the host address.
        /// </summary>
        public IHostAddressConfiguration HostAddressConfigurationInstance { get; private set; }
    }
}
