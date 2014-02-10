namespace ServiceBus.Queueing
{
    using ServiceBus.Messaging;
    using System;

    public interface IQueueManager : IDisposable
    {
        void Enqueue<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new();
    }
}
