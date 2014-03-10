namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
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
        private readonly ITransporter _transport;
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

            this.PeerAddress = hostAddress;

            this._transport = transporter;
            this._queueManager = queueManager;
            this._messageRouter = new MessageRouter(this._queueManager, this);

            this._logger = log;
            this._loggingEventHandler = new LoggingEventHandler(this);

            this.RegisterSystemMessages();

            this.RegisterSystemEventHandlers();

            this.RegisterInternalEvents();
        }

        /// <summary>
        /// An event raised when a publicly accessible <see cref="IServiceBus"/> method
        /// receives a exception not previously dealt with.
        /// </summary>
        public event Action<Exception, string> UnhandledExceptionOccurs;

        /// <summary>
        /// An event raised when an <see cref="IEvent"/> is published on the <see cref="IServiceBus"/>.
        /// </summary>
        public event Action<IEvent> EventPublished;

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
                return this._messageRouter.Peers;
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

        /// <summary>
        /// Gets the <see cref="IEventHandler"/>s subscriptions.
        /// </summary>
        public MessageSubscriptionDictionary Subscriptions
        {
            get
            {
                return this._messageRouter.Subscriptions;
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
                await this._queueManager.EnqueueAsync(new Envelope<TMessage>
                                                      {
                                                          Message = message,
                                                          MessageCreated = DateTime.Now,
                                                          Recipient = peer,
                                                          Sender = this
                                                      });
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
        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent, new()
        {
            Argument.CannotBeNull(@event, "event", "The event to publish cannot be null");

            try
            {
                var publishTask = this._messageRouter.PublishEventAsync(@event);

                await Task.Factory.StartNew(() => this.EventPublished(@event));

                await publishTask;
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
        public IServiceBus Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent, new()
        {
            Argument.CannotBeNull(eventHandler, "eventHandler", "To subscribe to an event, the event handler cannot be null.");

            try
            {
                this.Subscriptions.Subscribe(eventHandler);

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
                    sendMessageTasks.Add(Task.Factory.StartNew(() => this._transport.SendMessageAsync(messagePointer)));

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
                    new Envelope<PeerConnectedEvent>
                    {
                        Message = new PeerConnectedEvent
                        {
                            ConnectedPeer = new Peer(this.PeerAddress)
                        },
                        Recipient = newPeer,
                        Sender = this
                    });

                this._messageRouter.Peers.Add(newPeer);

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
        /// Register an <see cref="IMessageHandler"/> to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="messageHandler">The <see cref="IMessageHandler"/> to register.</param>
        /// <returns>The <see cref="IServiceBus"/>.</returns>
        /// <typeparam name="TMessage">
        /// The type of <see cref="IMessage"/> the <see cref="IMessageHandler"/> is being registered too.
        /// </typeparam>
        public IServiceBus WithMessageHandler<TMessage>(IMessageHandler<TMessage> messageHandler)
            where TMessage : class, IMessage, new()
        {
            Argument.CannotBeNull(messageHandler, "messageHandler", "When registering an message handler cannot be null.");

            try
            {
                this.Subscriptions.Subscribe(messageHandler);

                return this;
            }
            catch (Exception ex)
            {
                this.RaiseUnahndledExceptionEvent(ex, ExpressionExtensions.MethodName(() => this.WithMessageHandler<TMessage>(null)));
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

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PeerAddress", this.PeerAddress);
        }

        private void RegisterSystemMessages()
        {
            this.Serialiser.MessageTypeDictionary.Add(MessageExtensions.MessageTypeSignature<PeerConnectedEvent>(), typeof(PeerConnectedEvent));
        }

        private void RegisterSystemEventHandlers()
        {
            this.Subscribe(new PeerConnectedEventHandler(this));
            this.Subscribe(this._loggingEventHandler);
        }

        private void RegisterInternalEvents()
        {
            this.EventPublished += this._loggingEventHandler.LogEventPublished;

            this._queueManager.MessageQueued += async m => await this._transport.SendMessageAsync(m);

            this._transport.MessageSent += this._queueManager.Dequeue;
            this._transport.MessageSent += this._loggingEventHandler.LogMessageSent;

            this._transport.MessageRecieved += async (m, mc) => await this._messageRouter.RouteMessageAsync(m, mc);
            this._transport.MessageRecieved += this._loggingEventHandler.LogMessageRecieved;

            this._transport.MessageFailedToSend += this._loggingEventHandler.LogMessageFailedToSend;

            this.Serialiser.UnrecognisedMessageReceived += this._loggingEventHandler.LogUnrecognisedMessage;

            this.UnhandledExceptionOccurs += this._loggingEventHandler.LogGeneralFailure;
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

            this._disposed = true;
        }
    }
}