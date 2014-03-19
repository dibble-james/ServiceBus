namespace ServiceBus.Configuration
{
    using ServiceBus.Queueing;

    /// <summary>
    /// The configuration for the host application type.
    /// </summary>
    public class HostApplicationConfiguration : TransportConfiguration, IHostApplicationConfiguration
    {
        private readonly IQueueManager _queueManager;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostApplicationConfiguration"/> class.
        /// </summary>
        /// <param name="transportConfiguration">The <see cref="ITransportConfiguration"/>.</param>
        /// <param name="queueManager">The embedded queue database.</param>
        public HostApplicationConfiguration(ITransportConfiguration transportConfiguration, IQueueManager queueManager)
            : base(transportConfiguration)
        {
            this._queueManager = queueManager;
        }

        /// <summary>
        /// Build an instance of <see cref="IServiceBus"/> with all the information previously set.
        /// </summary>
        /// <returns>An <see cref="IServiceBus"/> instance.</returns>
        public IServiceBus Build()
        {
            return new Bus(
                this.HostAddress,
                this.Transporter,
                this._queueManager,
                this.Logger);
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
