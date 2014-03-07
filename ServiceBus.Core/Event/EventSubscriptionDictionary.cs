// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSubscriptionDictionary.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Event
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper for a dictionary to hold mappings for <see cref="IEvent"/> types and
    /// <see cref="IEventHandler"/>s.
    /// </summary>
    public sealed class EventSubscriptionDictionary
    {
        private readonly object _subscriptionsLock;
        private readonly IDictionary<Type, IEventSubscription> _subscrptions;

        /// <summary>
        /// Initialises a new instance of the <see cref="EventSubscriptionDictionary"/>.
        /// </summary>
        public EventSubscriptionDictionary()
        {
            this._subscriptionsLock = new object();
            this._subscrptions = new Dictionary<Type, IEventSubscription>();
        } 

        /// <summary>
        /// Retrieve the event subscriptions for a given type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to get the subscriptions for.</typeparam>
        /// <returns>The <see cref="IEventSubscription{TEvent}"/> for the given event.</returns>
        public IEventSubscription<TEvent> GetEventSubscrption<TEvent>() where TEvent : class, IEvent
        {
            lock (this._subscriptionsLock)
            {
                var subsciption = this._subscrptions[typeof(TEvent)] as IEventSubscription<TEvent>;

                if (subsciption != null)
                {
                    return subsciption;
                }

                subsciption = new EventSubscription<TEvent>();

                this._subscrptions.Add(typeof(TEvent), subsciption);

                return subsciption;   
            }
        }

        /// <summary>
        /// Register an <see cref="IEventHandler{TEvent}"/> to an <see cref="IEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to register.</typeparam>
        /// <param name="eventHandler">The <see cref="IEventHandler{TEvent}"/> to register.</param>
        internal void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEvent, new()
        {
            lock (this._subscriptionsLock)
            {
                if (!this._subscrptions.ContainsKey(typeof(TEvent)))
                {
                    this._subscrptions.Add(typeof(TEvent), new EventSubscription<TEvent>());
                }

                var subscription = (IEventSubscription<TEvent>)this._subscrptions[typeof(TEvent)];

                subscription.EventRaised += e => eventHandler.HandleAsync(e);
            }
        }
    }
}