// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSubscriptionDictionary.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{    
    using System;
    using System.Collections.Generic;

    using ServiceBus.Event;

    /// <summary>
    /// A wrapper for a dictionary to hold mappings for <see cref="IEvent"/> types and
    /// <see cref="IEventHandler"/>s.
    /// </summary>
    public sealed class MessageSubscriptionDictionary
    {
        private readonly object _subscriptionsLock;
        private readonly IDictionary<Type, IMessageSubscription> _subscrptions;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageSubscriptionDictionary"/>.
        /// </summary>
        public MessageSubscriptionDictionary()
        {
            this._subscriptionsLock = new object();
            this._subscrptions = new Dictionary<Type, IMessageSubscription>();
        } 

        /// <summary>
        /// Retrieve the event subscriptions for a given type.
        /// </summary>
        /// <typeparam name="TMessage">The type of event to get the subscriptions for.</typeparam>
        /// <returns>
        /// The <see cref="IMessageSubscription{TMessage}"/> for the given event or null if no subscription exists.
        /// </returns>
        public IMessageSubscription<TMessage> GetMessageSubscrption<TMessage>() where TMessage : class, IMessage, new()
        {
            lock (this._subscriptionsLock)
            {
                if (!this._subscrptions.ContainsKey(typeof(TMessage)))
                {
                    return null;
                }
                
                var subsciption = (IMessageSubscription<TMessage>)this._subscrptions[typeof(TMessage)];
                
                this._subscrptions.Add(typeof(TMessage), subsciption);

                return subsciption;   
            }
        }

        /// <summary>
        /// Register an <see cref="IMessageHandler{TMessage}"/> to an <see cref="IMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the <see cref="IMessage"/> to register.</typeparam>
        /// <param name="messageHandler">The <see cref="IMessageHandler{TMessage}"/> to register.</param>
        internal void Subscribe<TMessage>(IMessageHandler<TMessage> messageHandler)
            where TMessage : class, IMessage, new()
        {
            lock (this._subscriptionsLock)
            {
                if (!this._subscrptions.ContainsKey(typeof(TMessage)))
                {
                    this._subscrptions.Add(typeof(TMessage), new MessageSubscrption<TMessage>());
                }

                var subscription = (IMessageSubscription<TMessage>)this._subscrptions[typeof(TMessage)];

                subscription.MessageReceived += e => messageHandler.ProcessMessageAsync(e);
            }
        }
    }
}