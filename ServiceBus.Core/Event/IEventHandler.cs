namespace ServiceBus.Event
{
    using System.Threading.Tasks;
    using ServiceBus.Messaging;

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
    public interface IEventHandler<TEvent> : IEventHandler, IMessageHandler<TEvent> where TEvent : class, IEvent, new()
    {
    }
}