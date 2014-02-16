namespace ServiceBus.Event
{
    using System;
    using System.Threading.Tasks;
    using ServiceBus.Messaging;

    /// <summary>
    /// DO NOT IMPLEMENT.  Generic implementation must be used.
    /// </summary>
    public interface IEvent : IMessage
    {
    }

    /// <summary>
    /// Implementing classes define an event that can be raised and subscribed to.
    /// </summary>
    /// <typeparam name="TEvent">A self referencing type.</typeparam>
    public interface IEvent<TEvent> : IEvent where TEvent : class, IEvent<TEvent>
    {
        /// <summary>
        /// A subscription for raising this <see cref="IEvent"/> instance when it is received.
        /// </summary>
        event Action<TEvent> EventRaised;

        /// <summary>
        /// Raise the <see cref="E:EventRaised"/> event.
        /// </summary>
        /// <returns>An awaitable object representing the RaiseLocal operation.</returns>
        Task RaiseLocalAsync();
    }
}