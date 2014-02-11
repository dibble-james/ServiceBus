namespace ServiceBus.Configuration
{
    using System;

    /// <summary>
    /// A fluid <see cref="IServiceBus"/> factory.
    /// </summary>
    public class ServiceBusBuilder : IServiceBusBuilder
    {
        /// <summary>
        /// Start building an <see cref="IServiceBus"/>.
        /// </summary>
        /// <returns>A <see cref="IServiceBusBuilder"/> instance.</returns>
        public static IServiceBusBuilder Configure()
        {
            return new ServiceBusBuilder();
        }

        /// <summary>
        /// Sets the address the <see cref="IServiceBus"/> will be accessible from.
        /// </summary>
        /// <param name="address">The address the <see cref="IServiceBus"/> will be accessible from.</param>
        /// <returns>The host address configuration.</returns>
        public IHostAddressConfiguration WithHostAddress(Uri address)
        {
            return new HostAddressConfiguration(address);
        }
    }
}
