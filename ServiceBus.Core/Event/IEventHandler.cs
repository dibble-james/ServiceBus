namespace ServiceBus.Event
{
    using ServiceBus.Messaging;

    /// <summary>
    /// Implementing classes can be used to subscribe to <see cref="IEvent"/>s of <typeparamref name="TEvent"/> type.
    /// </summary>
    /// <typeparam name="TEvent">The type of <see cref="IEvent"/> this <see cref="IEventHandler{TEvent}"/> can subscribe to.</typeparam>
    public interface IEventHandler<TEvent> : IMessageHandler<TEvent> where TEvent : class, IEvent, new()
    {
    }
}