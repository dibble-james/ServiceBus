namespace ServiceBus.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using ServiceBus.Event;
    using ServiceBus.Messaging;
    using ServiceBus.Queueing;

    internal class MessageRouter
    {
        private readonly IPeer _self;
        private readonly ICollection<IPeer> _peers;
        private readonly object _peersLock;

        private readonly ICollection<IMessageHandler> _messageHandlers;
        private readonly object _messageHandlersLock;

        private readonly EventSubscriptionDictionary _subscriptionDictionary;
        private readonly IQueueManager _queueManager;

        internal MessageRouter(IQueueManager queueManager, IPeer self)
        {
            this._queueManager = queueManager;
            this._subscriptionDictionary = new EventSubscriptionDictionary();

            this._messageHandlers = new Collection<IMessageHandler>();
            this._messageHandlersLock = new object();

            this._peersLock = new object();
            this._peers = new Collection<IPeer>();

            this._self = self;
        }
        
        internal ICollection<IPeer> Peers
        {
            get
            {
                lock (this._peersLock)
                {
                    return this._peers;   
                }
            }
        }

        internal ICollection<IMessageHandler> MessageHandlers
        {
            get
            {
                lock (this._messageHandlersLock)
                {
                    return this._messageHandlers;
                }
            }
        }

        internal EventSubscriptionDictionary Subscriptions
        {
            get
            {
                return this._subscriptionDictionary;
            }
        }

        internal async Task PublishEventAsync<TEvent>(TEvent @event) 
            where TEvent : class, IEvent
        {
            var routeMessageTask =
                this.RouteMessageAsync(new Envelope<TEvent> { Message = @event, Recipient = this._self, Sender = this._self });

            await Task.WhenAll(this.Peers.Select(peer => this._queueManager.EnqueueAsync(new Envelope<TEvent>
                                                                                      {
                                                                                          Message = @event, 
                                                                                          Recipient = peer, 
                                                                                          Sender = this._self
                                                                                      })));

            await routeMessageTask;
        }

        internal async Task RouteMessageAsync(EnvelopeBase envelope)
        {
            if (envelope.Message is IEvent)
            {
                await this.HandleEvent(envelope.Message as IEvent);
                return;
            }

            await this.HandleMessage(envelope.Message);
        }

        private async Task HandleMessage(IMessage message)
        {
            var handleMessageGeneric = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == ExpressionExtensions.MethodName(() => this.HandleMessage(message)) && m.IsGenericMethod)
                .MakeGenericMethod(message.GetType());

            await Task.Factory.StartNew(() => handleMessageGeneric.Invoke(this, new object[] { message }));
        }

        private async Task HandleMessage<TMessage>(Envelope<TMessage> message) where TMessage : class, IMessage
        {
            var handlingTasks = this.MessageHandlers
                    .OfType<IMessageHandler<TMessage>>()
                    .Select(mh => mh.ProcessMessageAsync(message.Message));

            await Task.WhenAll(handlingTasks);
        }

        private async Task HandleEvent(IEvent @event)
        {
            var handleEventGeneric = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == ExpressionExtensions.MethodName(() => this.HandleEvent(@event)) && m.IsGenericMethod)
                .MakeGenericMethod(@event.GetType());

            await Task.Factory.StartNew(() => handleEventGeneric.Invoke(this, new object[] { @event }));
        }

        private async Task HandleEvent<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            var subscription = this._subscriptionDictionary.GetEventSubscrption<TEvent>();

            await subscription.RaiseLocalAsync(@event);
        }
    }
}