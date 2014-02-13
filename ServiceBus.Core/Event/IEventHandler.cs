namespace ServiceBus.Event
{
    /// <summary>
    /// DO NOT IMPLEMENT.  Implement the generic IEventHandler so that it can be registered.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Invoke the business logic contained by this <see cref="IEventHandler"/>.
        /// </summary>
        /// <param name="event">The event data.</param>
        void Handle(IEvent @event);
    }

    /// <summary>
    /// Implementing classes can be used to subscribe to <see cref="IEvent"/>s of <typeparamref name="TEvent"/> type.
    /// </summary>
    /// <typeparam name="TEvent">The type of <see cref="IEvent"/> this <see cref="IEventHandler{TEvent}"/> can subscribe to.</typeparam>
    public interface IEventHandler<in TEvent> : IEndpoint, IEventHandler where TEvent : class, IEvent
    {
        /// <summary>
        /// Invoke services to deal with this <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="event">The <typeparamref name="TEvent"/> data.</param>
        void Handle(TEvent @event);
    }
}