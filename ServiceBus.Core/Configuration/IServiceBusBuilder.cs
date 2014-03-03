namespace ServiceBus.Configuration
{
    using log4net;

    /// <summary>
    /// Implementing classes define methods to start building an <see cref="IServiceBus"/> instance.
    /// </summary>
    public interface IServiceBusBuilder
    {
        ILoggingConfiguration WithLogger(ILog logger);
    }
}
