namespace ServiceBus.Configuration
{
    using System;

    /// <summary>
    /// Implementing classes define the configuration for the host application type of the <see cref="IServiceBus"/>.
    /// </summary>
    public interface IHostApplicationConfiguration : IDisposable
    {
        /// <summary>
        /// Build an instance of <see cref="IServiceBus"/> with all the information previously set.
        /// </summary>
        /// <returns>An <see cref="IServiceBus"/> instance.</returns>
        IServiceBus Build();
    }
}
