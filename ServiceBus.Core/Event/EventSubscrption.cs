// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventSubscrption.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Event
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A class to represent a subscription to an <see cref="IEvent"/> type.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="IEvent"/> this <see cref="EventSubscription{TEvent}"/> holds.</typeparam>
    internal class EventSubscription<TEvent> : IEventSubscription<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// An event to raise the subscribed event.
        /// </summary>
        public event Action<TEvent> EventRaised;

        /// <summary>
        /// Raise the <see cref="E:EventRaised"/> event.
        /// </summary>
        /// <param name="event">The event instance.</param>
        /// <returns>An awaitable object representing the RaiseLocal operation.</returns>
        public async Task RaiseLocalAsync(TEvent @event)
        {
            if (this.EventRaised != null)
            {
                await Task.Factory.StartNew(() => this.EventRaised(@event));
            }
        }
    }
}