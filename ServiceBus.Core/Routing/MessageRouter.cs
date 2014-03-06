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
        private readonly ICollection<IPeer> _peers;
        private readonly object _peersLock;

        private readonly ICollection<IMessageHandler> _messageHandlers;
        private readonly object _messageHandlersLock;

        private readonly EventSubscriptionDictionary _subscriptionDictionary;
        private readonly IQueueManager _queueManager;

        internal MessageRouter(IQueueManager queueManager)
        {
            this._queueManager = queueManager;
            this._subscriptionDictionary = new EventSubscriptionDictionary();

            this._messageHandlers = new Collection<IMessageHandler>();
            this._messageHandlersLock = new object();

            this._peersLock = new object();
            this._peers = new Collection<IPeer>();
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

        internal async Task PublishEvent<TEvent>(TEvent @event) 
            where TEvent : class, IEvent
        {
            await Task.WhenAll(this.Peers.Select(p => this._queueManager.EnqueueAsync(p, @event)));
        }

        internal async Task RouteMessageAsync(IMessage message)
        {
            if (message is IEvent)
            {
                await this.HandleEvent(message as IEvent);
                return;
            }

            await this.HandleMessage(message);
        }

        private async Task HandleMessage(IMessage message)
        {
            var handleMessageGeneric = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == ExpressionExtensions.MethodName(() => this.HandleMessage(message)) && m.IsGenericMethod)
                .MakeGenericMethod(message.GetType());

            await Task.Factory.StartNew(() => handleMessageGeneric.Invoke(this, new object[] { message }));
        }

        private async Task HandleMessage<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            var handlingTasks = this.MessageHandlers
                    .OfType<IMessageHandler<TMessage>>()
                    .Select(mh => mh.ProcessMessageAsync(message));

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