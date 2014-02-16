namespace ServiceBus.Queueing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Db4objects.Db4o;
    using Db4objects.Db4o.Linq;
    using ServiceBus.Messaging;

    internal class QueueManager : IQueueManager
    {
        private readonly IObjectContainer _queuePersistence;

        private bool _disposed;
                
        public QueueManager(string storeDirectory)
        {
            this._disposed = false;

            var databasePath = Path.Combine(storeDirectory, "queue.db40");

            this._queuePersistence = Db4oEmbedded.OpenFile(databasePath);
        }

        public event Action<QueuedMessage> MessageQueued;

        public async Task EnqueueAsync<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new()
        {
            var queuedMessage = new QueuedMessage { QueuedAt = DateTime.Now, Peer = peer, Message = message, HasSent = false };

            this._queuePersistence.Store(queuedMessage);

            await Task.Factory.StartNew(() => this._queuePersistence.Commit());

            if (this.MessageQueued != null)
            {
                await Task.Factory.StartNew(() => this.MessageQueued(queuedMessage));
            }
        }

        public void Dequeue(QueuedMessage message)
        {
            var queuedMessage = this._queuePersistence.AsQueryable<QueuedMessage>()
                            .FirstOrDefault(qm => qm.QueuedAt == message.QueuedAt && qm.Peer == message.Peer && !message.HasSent);

            if (queuedMessage == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Message for peer [{0}] queued at [{1}] with message type [{2}] has already been dequeued",
                        message.Peer.PeerAddress.AbsoluteUri,
                        message.QueuedAt,
                        message.Message.MessageType));
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
                    .ThenByDescending(qm => qm.QueuedAt)
                    .FirstOrDefault(qm => !qm.HasSent && qm.Peer.PeerAddress == peer.PeerAddress);

            return nextMessage;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
                this._queuePersistence.Dispose();
            }

            this._disposed = true;
        }
    }
}
