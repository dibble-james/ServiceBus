namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using log4net;

    using ServiceBus.Core.EventHandlers;
    using ServiceBus.Core.Events;
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
        private readonly ICollection<IPeer> _peers;
        private readonly object _peersLock;
        private readonly ICollection<IEndpoint> _endpoints;
        private readonly object _endpointsLock;
        private readonly ITransporter _transport;
        private readonly ICollection<IEventHandler> _eventHandlers;
        private readonly object _eventHandlersLock;
        private readonly IQueueManager _queueManager;
        private readonly MessageRouter _messageRouter;
        private readonly ILog _logger;
        private readonly LoggingEventHandler _loggingEventHandler;

        private bool _disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="Bus"/> class.
        /// </summary>
        /// <param name="hostAddress">The address this service bus can be reached.</param>
        /// <param name="transporter">The protocol to use to communicate with other <see cref="IServiceBus"/>es.</param>
        /// <param name="queueManager">The message persistence service to use.</param>
        /// <param name="log">The <see cref="log4net.ILog"/> to use with this service bus instance.</param>
        internal Bus(
            Uri hostAddress,
            ITransporter transporter,
            IQueueManager queueManager,
            ILog log)
        {
            this._disposed = false;

            this._peersLock = new object();
            this._endpointsLock = new object();
            this._eventHandlersLock = new object();

            this.PeerAddress = hostAddress;

            this._endpoints = new Collection<IEndpoint>();
            this._eventHandlers = new Collection<IEventHandler>();
            this._peers = new Collection<IPeer>();

            this._transport = transporter;
            this._queueManager = queueManager;
            this._messageRouter = new MessageRouter(this.LocalEndpoints, this.EventHandlers);

            this._logger = log;
            this._loggingEventHandler = new LoggingEventHandler(this);

            this.RegisterSystemEventHandlers();

            this._queueManager.MessageQueued += m => this._transport.SendMessageAsync(m.Peer, m);

            this._transport.MessageSent += this._queueManager.Dequeue;
            this._transport.MessageSent += this._loggingEventHandler.LogMessageSent;

            this._transport.MessageRecieved += this._messageRouter.RouteMessageAsync;
            this._transport.MessageRecieved += this._loggingEventHandler.LogMessageRecieved;

            this._transport.MessageFailedToSend += this._loggingEventHandler.LogMessageFailedToSend;

            this.UnhandledExceptionOccurs += this._loggingEventHandler.LogGeneralFailure;
        }

        /// <summary>
        /// An event raised when a publicly accessible <see cref="IServiceBus"/> method
        /// receives a exception not previously dealt with.
        /// </summary>
        public event Action<Exception, string> UnhandledExceptionOccurs;

        /// <summary>
        /// Gets the <see cref="System.Uri"/> of the location of the <see cref="IPeer"/>.
        /// </summary>
        public Uri PeerAddress { get; private set; }
        
        /// <summary>
        /// Gets the <see cref="IPeer"/>s that are known to the <see cref="IServiceBus"/>.
        /// </summary>
        public IEnumerable<IPeer> Peers
        {
            get
            {
                return this.RegisteredPeers;
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
        /// Gets the <see cref="log4net.ILog"/> instance registered to the <see cref="IServiceBus"/>.
        /// </summary>
        public ILog Log
        {
            get
            {
                return this._logger;
            }
        }

        private ICollection<IPeer> RegisteredPeers
        {
            get
            {
                lock (this._peersLock)
                {
                    return this._peers;
                }
            }
        }

        private ICollection<IEndpoint> LocalEndpoints
        {
            get
            {
                lock (this._endpointsLock)
                {
                    return this._endpoints;
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
            Argument.CannotBeNull(peer, "peer", "The peer to send to cannot be null.");
            Argument.CannotBeNull(message, "message", "The message to send cannot be null.");

            try
            {
                await this._queueManager.EnqueueAsync(peer, message);
            }
            catch (Exception ex)
            {   
                this.RaiseUnahndledExceptionEvent(ex, ExpressionExtensions.MethodName(() => this.SendAsync<TMessage>(null, null)));
                throw;
            }
        }

        /// <summary>
        /// Raise an instance of <typeparamref name="TEvent"/> to the <see cref="P:Peers"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IEvent"/> to raise.</typeparam>
        /// <param name="event">The event data to publish.</param>
        /// <returns>An awaitable object representing the publish operation.</returns>
        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent<TEvent>, new()
        {
            Argument.CannotBeNull(@event, "event", "The event to publish cannot be null");

            try
            {
                foreach (var eventHandler in this.EventHandlers.OfType<IEventHandler<TEvent>>())
                {
                    var eventHandlerPointer = eventHandler;

                    @event.EventRaised += e => eventHandlerPointer.HandleAsync(e);
                }

                var handleEventLocallyTask = @event.RaiseLocalAsync();

                var raiseEventToPeerTasks =
                    this.RegisteredPeers.Select(p => this._queueManager.EnqueueAsync(p, @event));

                await Task.WhenAll(raiseEventToPeerTasks);

                await handleEventLocallyTask;
            }
            catch (Exception ex)
            {
                this.RaiseUnahndledExceptionEvent(ex, ExpressionExtensions.MethodName(() => this.PublishAsync<TEvent>(null)));
                throw;
            }
        }

        /// <summary>
        /// Register an instance of an <see cref="IEventHandler{TEvent}"/> to the <see cref="IServiceBus"/> so it can handle a <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of event the <paramref name="eventHandler"/> handles.</typeparam>
        /// <param name="eventHandler">The <see cref="IEventHandler{TEvent}"/> to register.</param>
        /// <returns>The <see cref="IServiceBus"/>.</returns>
        public IServiceBus Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent<TEvent>, new()
        {
            Argument.CannotBeNull(eventHandler, "eventHandler", "To subscribe to an event, the event handler cannot be null.");

            try
            {
                this.EventHandlers.Add(eventHandler);

                return this;
            }
            catch (Exception ex)
            {
                this.RaiseUnahndledExceptionEvent(ex, ExpressionExtensions.MethodName(() => this.Subscribe<TEvent>(null)));
                throw;
            }
        }

        /// <summary>
        /// Transmit all queued messages to the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to synchronise.</param>
        /// <returns>An awaitable object representing the synchronise operation.</returns>
        public async Task SynchroniseAsync(IPeer peer)
        {
            Argument.CannotBeNull(peer, "peer", "The peer to synchronise cannot be null.");

            try
            {
                var message = this._queueManager.PeersNextMessageOrDefault(peer);

                var sendMessageTasks = new List<Task>();

                while (message != null)
                {
                    var messagePointer = message;
                    sendMessageTasks.Add(Task.Factory.StartNew(() => this._transport.SendMessageAsync(peer, messagePointer)));

                    message = this._queueManager.PeersNextMessageOrDefault(peer, message.QueuedAt);
                }

                await Task.WhenAll(sendMessageTasks);
            }
            catch (Exception ex)
            {
                this.RaiseUnahndledExceptionEvent(ex, ExpressionExtensions.MethodName(() => this.SynchroniseAsync(null)));
                throw;
            }
        }

        /// <summary>
        /// Add a remote instance of <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="peer">The known <see cref="IServiceBus"/> location.</param>
        /// <returns>The <see cref="IServiceBus"/>.</returns>
        public async Task<IServiceBus> WithPeerAsync(Uri peer)
        {
            Argument.CannotBeNull(peer, "peer", "When registering a peer, its address cannot be null.");

            try
            {
                var newPeer = new Peer(peer);

                var registerWithPeerTask = this._queueManager.EnqueueAsync(
                    newPeer,
                    new PeerConnectedEvent
                    {
                        ConnectedPeer =
                        new Peer(this.PeerAddress)
                    });

                this.RegisteredPeers.Add(newPeer);

                await registerWithPeerTask;

                return this;
            }
            catch (Exception ex)
            {
                this.RaiseUnahndledExceptionEvent(ex, ExpressionExtensions.MethodName(() => this.WithPeerAsync(null)));
                throw;
            }
        }

        /// <summary>
        /// Register an <see cref="IEndpoint"/> to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="endpoint">The <see cref="IEndpoint"/> to register.</param>
        /// <returns>The <see cref="IServiceBus"/>.</returns>
        public IServiceBus WithLocalEndpoint(IEndpoint endpoint)
        {
            Argument.CannotBeNull(endpoint, "endpoint", "When registering an endpoint it cannot be null.");

            try
            {
                this.LocalEndpoints.Add(endpoint);

                return this;
            }
            catch (Exception ex)
            {
                this.RaiseUnahndledExceptionEvent(ex, ExpressionExtensions.MethodName(() => this.WithLocalEndpoint(null)));
                throw;
            }
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
            this.Subscribe(this._loggingEventHandler);
        }

        private void RaiseUnahndledExceptionEvent(Exception ex, string methodName)
        {
            if (this.UnhandledExceptionOccurs != null)
            {
                this.UnhandledExceptionOccurs(ex, methodName);
            }
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