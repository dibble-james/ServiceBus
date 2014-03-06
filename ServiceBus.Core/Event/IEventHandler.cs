namespace ServiceBus.Event
{
    using System.Threading.Tasks;

    /// <summary>
    /// DO NOT IMPLEMENT.  Implement the generic IEventHandler so that it can be registered.  This interface exists only to 
    /// be able to store <see cref="IEventHandler{TEvent}"/>s in a collection.
    /// </summary>
    public interface IEventHandler
    {
    }

    /// <summary>
    /// Implementing classes can be used to subscribe to <see cref="IEvent"/>s of <typeparamref name="TEvent"/> type.
    /// </summary>
    /// <typeparam name="TEvent">The type of <see cref="IEvent"/> this <see cref="IEventHandler{TEvent}"/> can subscribe to.</typeparam>
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class, IEvent, new()
    {
        /// <summary>
        /// Invoke services to deal with this <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="event">The <typeparamref name="TEvent"/> data.</param>
        /// <returns>An awaitable object representing the handling operation.</returns>
        Task HandleAsync(TEvent @event);
    }
}