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
    /// DO NOT IMPLEMENT.  This interface exists only to be able to hold <see cref="IEventSubscription{TEvent}"/>s
    /// in a collection.
    /// </summary>
    public interface IEventSubscription
    { 
    }

    /// <summary>
    /// A interface to represent a subscription to an <see cref="IEvent"/> type.
    /// </summary>
    /// <typeparam name="TEvent">The <see cref="IEvent"/> this <see cref="EventSubscription{TEvent}"/> holds.</typeparam>
    public interface IEventSubscription<TEvent> : IEventSubscription where TEvent : class, IEvent
    {
        /// <summary>
        /// A subscription for raising this <see cref="IEvent"/> instance when it is received.
        /// </summary>
        event Action<TEvent> EventRaised;

        /// <summary>
        /// Raise the <see cref="E:EventRaised"/> event.
        /// </summary>
        /// <param name="event">The event instance being raised.</param>
        /// <returns>An awaitable object representing the RaiseLocal operation.</returns>
        Task RaiseLocalAsync(TEvent @event);
    }
}