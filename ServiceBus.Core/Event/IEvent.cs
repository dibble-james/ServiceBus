namespace ServiceBus.Event
{
    using ServiceBus.Messaging;

    /// <summary>
    /// Implementing classes define an event that can be raised and subscribed to.
    /// </summary>
    public interface IEvent : IMessage
    {
    }
}