namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ServiceBus.Core.EventHandlers;
    using ServiceBus.Event;
    using ServiceBus.Messaging;
    using ServiceBus.Queueing;
    using ServiceBus.Routing;
    using ServiceBus.Transport;

    /// <summary>
    /// An object for orchestrating reliable message and event managing.  This class cannot be inherited.
    /// </summary>
    internal sealed class Bus : IServiceBus
    {
        private readonly IEnumerable<IPeer> _peers;
        private readonly object _peersLock;
        private readonly IEnumerable<IEndpoint> _endpoints;
        private readonly object _endpointsLock;
        private readonly ITransporter _transport;
        private readonly ICollection<IEventHandler> _eventHandlers;
        private readonly object _eventHandlersLock;
        private readonly IQueueManager _queueManager;
        private readonly MessageRouter _messageRouter;

        private bool _disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="Bus"/> class.
        /// </summary>
        /// <param name="hostAddress">The address this service bus can be reached.</param>
        /// <param name="transporter">The protocol to use to communicate with other <see cref="IServiceBus"/>es.</param>
        /// <param name="queueManager">The message persistence service to use.</param>
        /// <param name="endpoints">Any <see cref="IMessageHandler"/>s known before runtime.</param>
        /// <param name="peers">Any known remote instances of <see cref="IServiceBus"/> known before runtime.</param>
        /// <param name="eventHandlers">Any subscribed <see cref="IEventHandler"/>s known before runtime.</param>
        internal Bus(
            Uri hostAddress,
            ITransporter transporter,
            IQueueManager queueManager,
            IEnumerable<IEndpoint> endpoints,
            IEnumerable<IPeer> peers,
            ICollection<IEventHandler> eventHandlers)
        {
            this._disposed = false;

            this._peersLock = new object();
            this._endpointsLock = new object();
            this._eventHandlersLock = new object();

            this.PeerAddress = hostAddress;

            this._endpoints = endpoints;
            this._eventHandlers = eventHandlers;
            this._peers = peers;

            this._transport = transporter;
            this._queueManager = queueManager;
            this._messageRouter = new MessageRouter(this.LocalEndpoints, this.EventHandlers);

            this.RegisterSystemEventHandlers();

            this._queueManager.MessageQueued += m => this._transport.SendMessage(m.Peer, m);
            this._transport.MessageSent += this._queueManager.Dequeue;
            this._transport.MessageRecieved += this._messageRouter.RouteMessageAsync;
        }

        /// <summary>
        /// Gets the <see cref="System.Uri"/> of the location of the <see cref="IPeer"/>.
        /// </summary>
        public Uri PeerAddress { get; private set; }

        /// <summary>
        /// Gets the <see cref="IEndpoint"/>s that are known to the <see cref="IServiceBus"/>.
        /// </summary>
        public IEnumerable<IEndpoint> LocalEndpoints
        {
            get
            {
                lock (this._endpointsLock)
                {
                    return this._endpoints;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="IMessageSerialiser"/> that is registered to the <see cref="IServiceBus"/>es <see cref="ITransporter"/>.
        /// </summary>
        public IMessageSerialiser Serialiser
        {
            get
            {
                return this._transport.Serialiser;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITransporter"/> that is registered to the <see cref="IServiceBus"/>.
        /// </summary>
        public ITransporter Transporter
        {
            get
            {
                return this._transport;
            }
        }

        /// <summary>
        /// Gets the <see cref="IPeer"/>s that are known to the <see cref="IServiceBus"/>.
        /// </summary>
        public IEnumerable<IPeer> Peers
        {
            get
            {
                lock (this._peersLock)
                {
                    return this._peers;
                }
            }
        }
        
        private ICollection<IEventHandler> EventHandlers
        {
            get
            {
                lock (this._eventHandlersLock)
                {
                    return this._eventHandlers;
                }
            }
        }

        /// <summary>
        /// Directly send an <paramref name="message"/> to a given <paramref name="peer"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the <see cref="IMessage"/> to send.</typeparam>
        /// <param name="peer">The peer who should receive the <paramref name="message"/>.</param>
        /// <param name="message">The <see cref="IMessage"/> to send.</param>
        /// <returns>An awaitable object representing the send operation.</returns>
        public async Task SendAsync<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new()
        {
            await this._queueManager.EnqueueAsync(peer, message);
        }

        /// <summary>
        /// Raise an instance of <typeparamref name="TEvent"/> to the <see cref="P:Peers"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IEvent"/> to raise.</typeparam>
        /// <param name="event">The event data to publish.</param>
        /// <returns>An awaitable object representing the publish operation.</returns>
        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent<TEvent>, new()
        {
            var localEventHandlerTasks = 
                this.EventHandlers.OfType<IEventHandler<TEvent>>().Select(eh => Task.Factory.StartNew(() => eh.Handle(@event)));

            var raiseEventToPeerTasks =
                this.Peers.Select(p => Task.Factory.StartNew(() => this._queueManager.EnqueueAsync(p, @event)));

            await Task.WhenAll(raiseEventToPeerTasks.Union(localEventHandlerTasks));
        }

        /// <summary>
        /// Register an instance of an <see cref="IEventHandler{TEvent}"/> to the <see cref="IServiceBus"/> so it can handle a <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of event the <paramref name="eventHandler"/> handles.</typeparam>
        /// <param name="eventHandler">The <see cref="IEventHandler{TEvent}"/> to register.</param>
        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent<TEvent>, new()
        {
            this.EventHandlers.Add(eventHandler);
        }

        /// <summary>
        /// Transmit all queued messages to the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to synchronise.</param>
        /// <returns>An awaitable object representing the synchronise operation.</returns>
        public async Task SynchroniseAsync(IPeer peer)
        {
            var message = this._queueManager.PeersNextMessageOrDefault(peer);

            var sendMessageTasks = new List<Task>();

            while (message != null)
            {
                var messagePointer = message;
                sendMessageTasks.Add(new Task(() => this._transport.SendMessage(peer, messagePointer)));

                message = this._queueManager.PeersNextMessageOrDefault(peer);
            }

            await Task.WhenAll(sendMessageTasks);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
                this.Dispose(true);
            }
        }

        private void RegisterSystemEventHandlers()
        {
            this.Subscribe(new PeerConnectedEventHandler(this));
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this._queueManager.Dispose();

            this._transport.Dispose();

            foreach (var eventHandler in this.EventHandlers.OfType<IDisposable>())
            {
                eventHandler.Dispose();
            }

            foreach (var localEndpoint in this.LocalEndpoints.OfType<IDisposable>())
            {
                localEndpoint.Dispose();
            }

            this._disposed = true;
        }
    }
}