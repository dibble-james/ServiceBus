namespace ServiceBus.Queueing
{
    using System;
    using System.IO;
    using System.Linq;
    using Db4objects.Db4o;
    using Db4objects.Db4o.Linq;
    using ServiceBus.Events;
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

        public event EventHandler<MessageQueuedEventArgs> MessageQueued;

        public void Enqueue<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new()
        {
            var queuedMessage = new QueuedMessage { QueuedAt = DateTime.Now, Peer = peer, Message = message, HasSent = false };

            this._queuePersistence.Store(queuedMessage);

            this._queuePersistence.Commit();

            if (this.MessageQueued != null)
            {
                this.MessageQueued(this, new MessageQueuedEventArgs { MessageQueued = queuedMessage });
            }
        }

        public void Dequeue(object sender, MessageSentEventArgs args)
        {
            this.Dequeue(args.MessageSent);
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
                    .OrderByDescending(qm => qm.QueuedAt)
                    .FirstOrDefault(qm => !qm.HasSent);

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
