namespace ServiceBus.Queueing
{
    using System;
    using ServiceBus.Events;
    using ServiceBus.Messaging;

    public interface IQueueManager : IDisposable
    {
        event EventHandler<MessageQueuedEventArgs> MessageQueued;

        void Enqueue<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new();

        void Dequeue(object sender, MessageSentEventArgs args);

        void Dequeue(QueuedMessage message);

        QueuedMessage PeersNextMessageOrDefault(IPeer peer);
    }
}
