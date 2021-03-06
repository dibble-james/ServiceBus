﻿namespace ServiceBus.Queueing
{
    using System;
    using System.Threading.Tasks;

    using ServiceBus.Messaging;

    /// <summary>
    /// Implementing classes define methods of persisting messages FIFO.
    /// </summary>
    public interface IQueueManager : IDisposable
    {
        /// <summary>
        /// An event that is raised when an <see cref="IMessage"/> is placed onto the <see cref="IServiceBus"/> and persisted.
        /// </summary>
        event Action<QueuedMessage> MessageQueued;

        /// <summary>
        /// Persist an <see cref="IMessage"/> and raise <see cref="E:IQueueManager.MessageQueue"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="IMessage"/> to queue.</typeparam>
        /// <param name="envelope">The message to place into the queue.</param>
        /// <returns>An awaitable object representing the enqueue operation.</returns>
        Task EnqueueAsync<TMessage>(Envelope<TMessage> envelope) where TMessage : class, IMessage;

        /// <summary>
        /// Mark a message as sent.
        /// </summary>
        /// <param name="message">The <see cref="QueuedMessage"/> that was sent.</param>
        /// <param name="messageContent">The raw message content.</param>
        void Dequeue(QueuedMessage message, string messageContent);

        /// <summary>
        /// Retrieve the next <see cref="QueuedMessage"/> for the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to check for <see cref="QueuedMessage"/>s.</param>
        /// <returns>The next <see cref="QueuedMessage"/> or null if the queue for the <paramref name="peer"/> is empty.</returns>
        QueuedMessage PeersNextMessageOrDefault(IPeer peer);

        /// <summary>
        /// Retrieve the next <see cref="QueuedMessage"/> for the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to check for <see cref="QueuedMessage"/>s.</param>
        /// <param name="messageQueuedBefore">The <see cref="System.DateTime"/> to find messages queued before.</param>
        /// <returns>The next <see cref="QueuedMessage"/> or null if the queue for the <paramref name="peer"/> is empty.</returns>
        QueuedMessage PeersNextMessageOrDefault(IPeer peer, DateTime messageQueuedBefore);
    }
}
