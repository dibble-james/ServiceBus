namespace ServiceBus.Configuration
{
    using log4net;

    /// <summary>
    /// Implementing classes define methods to start building an <see cref="IServiceBus"/> instance.
    /// </summary>
    public interface IServiceBusBuilder
    {
        /// <summary>
        /// Set the <see cref="log4net.ILog"/> for use by the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="logger">The <see cref="log4net.ILog"/> instance to use.</param>
        /// <returns>Configuration settings.</returns>
        ILoggingConfiguration WithLogger(ILog logger);
    }
}
