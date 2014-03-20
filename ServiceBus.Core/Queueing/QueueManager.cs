namespace ServiceBus.Queueing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Db4objects.Db4o;
    using Db4objects.Db4o.Linq;

    using ServiceBus.Core.Events;
    using ServiceBus.Messaging;

    /// <summary>
    /// An queue that uses an embedded database for persisting <see cref="IMessage"/>s.
    /// </summary>
    public sealed class QueueManager : IQueueManager
    {
        private readonly IObjectContainer _queuePersistence;

        private bool _disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="QueueManager"/> class.
        /// </summary>
        /// <param name="queuePersistence">The embedded queue database.</param>
        public QueueManager(IObjectContainer queuePersistence)
        {
            this._disposed = false;

            this._queuePersistence = queuePersistence;
        }

        /// <summary>
        /// An event that is raised when an <see cref="IMessage"/> is placed onto the <see cref="IServiceBus"/> and persisted.
        /// </summary>
        public event Action<QueuedMessage> MessageQueued;

        /// <summary>
        /// Persist an <see cref="IMessage"/> and raise <see cref="E:IQueueManager.MessageQueue"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="IMessage"/> to queue.</typeparam>
        /// <param name="envelope">The message to place into the queue.</param>
        /// <returns>An awaitable object representing the enqueue operation.</returns>
        public async Task EnqueueAsync<TMessage>(Envelope<TMessage> envelope) where TMessage : class, IMessage
        {
            var queuedMessage = new QueuedMessage { QueuedAt = DateTime.Now, Envelope = envelope, HasSent = false };

            this._queuePersistence.Store(queuedMessage);

            await Task.Factory.StartNew(() => this._queuePersistence.Commit());

            if (this.MessageQueued != null)
            {
                await Task.Factory.StartNew(() => this.MessageQueued(queuedMessage));
            }
        }

        /// <summary>
        /// Mark a message as sent.
        /// </summary>
        /// <param name="message">The <see cref="QueuedMessage"/> that was sent.</param>
        /// <param name="messageContent">The raw message content.</param>
        public void Dequeue(QueuedMessage message, string messageContent)
        {
            var queuedMessage = this._queuePersistence.AsQueryable<QueuedMessage>()
                            .FirstOrDefault(
                                qm => qm.QueuedAt == message.QueuedAt
                                   && qm.Envelope.Recipient.PeerAddress == message.Envelope.Recipient.PeerAddress
                                   && !message.HasSent);

            if (queuedMessage == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Message for peer [{0}] queued at [{1}] with message type [{2}] has already been dequeued",
                        message.Envelope.Recipient.PeerAddress.AbsoluteUri,
                        message.QueuedAt,
                        message.Envelope.Message.MessageType));
            }

            queuedMessage.HasSent = true;

            this._queuePersistence.Store(queuedMessage);

            this._queuePersistence.Commit();
        }

        /// <summary>
        /// Retrieve the next <see cref="QueuedMessage"/> for the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to check for <see cref="QueuedMessage"/>s.</param>
        /// <returns>The next <see cref="QueuedMessage"/> or null if the queue for the <paramref name="peer"/> is empty.</returns>
        public QueuedMessage PeersNextMessageOrDefault(IPeer peer)
        {
            var nextMessage =
                    this._queuePersistence.AsQueryable<QueuedMessage>()
                        .OrderByDescending(qm => qm.HasSent)
                        .ThenBy(qm => qm.QueuedAt)
                        .FirstOrDefault(
                        qm =>
                               !qm.HasSent
                            && qm.Envelope.Recipient.PeerAddress == peer.PeerAddress
                            && !(qm.Envelope.Message is PeerConnectedEvent));

            return nextMessage;
        }

        /// <summary>
        /// Retrieve the next <see cref="QueuedMessage"/> for the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to check for <see cref="QueuedMessage"/>s.</param>
        /// <param name="messageQueuedBefore">The <see cref="System.DateTime"/> to find messages queued before.</param>
        /// <returns>The next <see cref="QueuedMessage"/> or null if the queue for the <paramref name="peer"/> is empty.</returns>
        public QueuedMessage PeersNextMessageOrDefault(IPeer peer, DateTime messageQueuedBefore)
        {
            var nextMessage =
                this._queuePersistence.AsQueryable<QueuedMessage>()
                    .OrderByDescending(qm => qm.HasSent)
                    .ThenBy(qm => qm.QueuedAt)
                    .FirstOrDefault(
                        qm => !qm.HasSent
                              && qm.Envelope.Recipient.PeerAddress == peer.PeerAddress
                              && qm.QueuedAt > messageQueuedBefore
                              && !(qm.Envelope.Message is PeerConnectedEvent));

            return nextMessage;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this._disposed = true;
        }
    }
}
