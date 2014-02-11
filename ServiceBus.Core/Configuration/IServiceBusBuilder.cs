namespace ServiceBus.Configuration
{
    using System;

    /// <summary>
    /// Implementing classes define methods to start building an <see cref="IServiceBus"/> instance.
    /// </summary>
    public interface IServiceBusBuilder
    {
        /// <summary>
        /// Sets the address the <see cref="IServiceBus"/> will be accessible from.
        /// </summary>
        /// <param name="address">The address the <see cref="IServiceBus"/> will be accessible from.</param>
        /// <returns>The host address configuration.</returns>
        IHostAddressConfiguration WithHostAddress(Uri address);
    }
}
