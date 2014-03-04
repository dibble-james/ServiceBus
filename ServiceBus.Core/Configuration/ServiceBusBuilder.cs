namespace ServiceBus.Configuration
{
    using log4net;

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
        /// Set the <see cref="log4net.ILog"/> for use by the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="logger">The <see cref="log4net.ILog"/> instance to use.</param>
        /// <returns>Configuration settings.</returns>
        public ILoggingConfiguration WithLogger(ILog logger)
        {
            return new LoggingConfiguration(logger);
        }
    }
}
