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
        
        private readonly MessageSubscriptionDictionary _subscriptionDictionary;
        private readonly IQueueManager _queueManager;

        internal MessageRouter(IQueueManager queueManager, IPeer self)
        {
            this._queueManager = queueManager;
            this._subscriptionDictionary = new MessageSubscriptionDictionary();

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

        internal MessageSubscriptionDictionary Subscriptions
        {
            get
            {
                return this._subscriptionDictionary;
            }
        }

        internal async Task PublishEventAsync<TEvent>(TEvent @event) 
            where TEvent : class, IEvent, new()
        {
            var routeMessageTask =
                this.HandleMessageAsync(new Envelope<TEvent>
                                       {
                                           Message = @event, 
                                           MessageCreated = DateTime.Now,
                                           Recipient = this._self, 
                                           Sender = this._self
                                       });

            await Task.WhenAll(this.Peers.Select(peer => this._queueManager.EnqueueAsync(new Envelope<TEvent>
                                                                                      {
                                                                                          Message = @event,
                                                                                          MessageCreated = DateTime.Now,
                                                                                          Recipient = peer, 
                                                                                          Sender = this._self
                                                                                      })));

            await routeMessageTask;
        }

        internal async Task RouteMessageAsync(EnvelopeBase envelope, string messageContent)
        {
            var handleMessageGeneric = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == "HandleMessageAsync" && m.IsGenericMethod)
                .MakeGenericMethod(envelope.Message.GetType());

            await Task.Factory.StartNew(() => handleMessageGeneric.Invoke(this, new object[] { envelope }));
        }

        private async Task HandleMessageAsync<TMessage>(Envelope<TMessage> envelope) 
            where TMessage : class, IMessage, new()
        {
            var subscription = this._subscriptionDictionary.GetMessageSubscrption<TMessage>();

            if (subscription == null)
            {
                return;
            }

            await subscription.RaiseMessageReceivedAsync(envelope);
        }
    }
}