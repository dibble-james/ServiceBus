// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventSubscrption.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Queueing.Ftp
{
    using ServiceBus.Messaging;
    using System;
    using System.Threading.Tasks;

    public class FtpQueueManager : IQueueManager
    {
        public event Action<QueuedMessage> MessageQueued;

        public Task EnqueueAsync<TMessage>(Envelope<TMessage> envelope) where TMessage : class, IMessage
        {
            throw new System.NotImplementedException();
        }

        public void Dequeue(QueuedMessage message, string messageContent)
        {
            throw new NotImplementedException();
        }

        public QueuedMessage PeersNextMessageOrDefault(IPeer peer)
        {
            throw new NotImplementedException();
        }

        public QueuedMessage PeersNextMessageOrDefault(IPeer peer, DateTime messageQueuedBefore)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
