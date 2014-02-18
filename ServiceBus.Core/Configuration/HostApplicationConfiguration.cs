namespace ServiceBus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using ServiceBus.Core.Events;
    using ServiceBus.Event;
    using ServiceBus.Queueing;

    /// <summary>
    /// The configuration for the host application type.
    /// </summary>
    public class HostApplicationConfiguration : IHostApplicationConfiguration
    {
        private readonly ITransportConfiguration _transportConfiguration;
        private readonly ICollection<IPeer> _peers;
        private readonly ICollection<IEndpoint> _endpoints;
        private readonly ICollection<IEventHandler> _eventHandlers;
        private readonly IQueueManager _queueManager;

        /// <summary>
        /// Initialises a new instance of the <see cref="HostApplicationConfiguration"/> class.
        /// </summary>
        /// <param name="transportConfiguration">The <see cref="ITransportConfiguration"/>.</param>
        /// <param name="queueStoreDirectory">The path of the directory where the queued will be placed.</param>
        public HostApplicationConfiguration(ITransportConfiguration transportConfiguration, string queueStoreDirectory)
        {
            this._transportConfiguration = transportConfiguration;
            this._peers = new Collection<IPeer>();
            this._endpoints = new Collection<IEndpoint>();
            this._eventHandlers = new Collection<IEventHandler>();
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
                this._queueManager,
                this._endpoints,
                this._peers,
                this._eventHandlers);
        }

        /// <summary>
        /// Add a remote instance of <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="peer">The known <see cref="IServiceBus"/> location.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
        public IHostApplicationConfiguration WithPeer(Uri peer)
        {
            var newPeer = new Peer(peer);

            this._queueManager.EnqueueAsync(
                newPeer, 
                new PeerConnectedEvent
                    {
                        ConnectedPeer = 
                        new Peer(this._transportConfiguration.HostAddressConfiguration.HostAddress)
                    });

            this._peers.Add(newPeer);

            return this;
        }

        /// <summary>
        /// Register an <see cref="IEndpoint"/> to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="endpoint">The <see cref="IEndpoint"/> to register.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
        public IHostApplicationConfiguration WithLocalEndpoint(IEndpoint endpoint)
        {
            this._endpoints.Add(endpoint);

            return this;
        }

        /// <summary>
        /// Register an <see cref="IEventHandler"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IEvent"/> this <see cref="IEventHandler"/> handles.</typeparam>
        /// <param name="eventHandler">The <see cref="IEventHandler"/> to register.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
        public IHostApplicationConfiguration Subscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEvent<TEvent>
        {
            this._eventHandlers.Add(eventHandler);

            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._queueManager.Dispose();
            }
        }
    }
}
