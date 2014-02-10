namespace ServiceBus.Queueing
{
    using ServiceBus.Messaging;

    internal class QueuedMessage<TMessage> where TMessage : class, IMessage, new()
    {
        internal IPeer Peer { get; set; }

        internal TMessage Message { get; set; }
    }
}
