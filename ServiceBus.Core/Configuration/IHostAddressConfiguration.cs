namespace ServiceBus.Configuration
{
    using System;

    using log4net;

    /// <summary>
    /// Implementing classes define the configuration of the host address.
    /// </summary>
    public interface IHostAddressConfiguration
    {
        /// <summary>
        /// Gets the address this <see cref="IServiceBus"/> will be accessible from.
        /// </summary>
        Uri HostAddress { get; }

        ILoggingConfiguration LoggingConfiguration { get; }
    }
}
