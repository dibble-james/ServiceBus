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

        public ILoggingConfiguration WithLogger(ILog logger)
        {
            return new LoggingConfiguration(logger);
        }
    }
}
