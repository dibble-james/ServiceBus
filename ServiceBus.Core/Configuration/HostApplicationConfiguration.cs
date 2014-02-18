namespace ServiceBus.Configuration
{
    using ServiceBus.Queueing;

    /// <summary>
    /// The configuration for the host application type.
    /// </summary>
    public class HostApplicationConfiguration : IHostApplicationConfiguration
    {
        private readonly ITransportConfiguration _transportConfiguration;
        private readonly IQueueManager _queueManager;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostApplicationConfiguration"/> class.
        /// </summary>
        /// <param name="transportConfiguration">The <see cref="ITransportConfiguration"/>.</param>
        /// <param name="queueStoreDirectory">The path of the directory where the queued will be placed.</param>
        public HostApplicationConfiguration(ITransportConfiguration transportConfiguration, string queueStoreDirectory)
        {
            this._transportConfiguration = transportConfiguration;
            this._queueManager = new QueueManager(queueStoreDirectory);
        }

        /// <summary>
        /// Build an instance of <see cref="IServiceBus"/> with all the information previously set.
        /// </summary>
        /// <returns>An <see cref="IServiceBus"/> instance.</returns>
        public IServiceBus Build()
        {
            return new Bus(
                this._transportConfiguration.HostAddressConfiguration.HostAddress,
                this._transportConfiguration.Transporter,
                this._queueManager);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating where the class is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._queueManager.Dispose();
            }
        }
    }
}
