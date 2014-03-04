namespace ServiceBus.Configuration
{
    using System;

    /// <summary>
    /// Implementing classes define the configuration of the host address.
    /// </summary>
    public interface IHostAddressConfiguration : ILoggingConfiguration
    {
        /// <summary>
        /// Gets the address this <see cref="IServiceBus"/> will be accessible from.
        /// </summary>
        Uri HostAddress { get; }

        /// <summary>
        /// Gets the previously set configuration for the host address.
        /// </summary>
        IHostAddressConfiguration HostAddressConfigurationInstance { get; }
    }
}
