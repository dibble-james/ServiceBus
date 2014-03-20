namespace ServiceBus.Queueing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Db4objects.Db4o;
    using Db4objects.Db4o.Linq;

    using ServiceBus.Core.Events;
    using ServiceBus.Messaging;

    public sealed class QueueManager : IQueueManager
    {
        private readonly IObjectContainer _queuePersistence;

        private bool _disposed;

        public QueueManager(IObjectContainer queuePersistence)
        {
            this._disposed = false;

            this._queuePersistence = queuePersistence;
        }

        public event Action<QueuedMessage> MessageQueued;

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
