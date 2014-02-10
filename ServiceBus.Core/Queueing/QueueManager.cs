namespace ServiceBus.Queueing
{
    using Db4objects.Db4o;
    using ServiceBus.Messaging;
    using System;
    using System.IO;

    internal class QueueManager : IQueueManager
    {
        private bool _disposed;

        private readonly IObjectContainer _queuePersistence;

        public QueueManager(string storeDirectory)
        {
            this._disposed = false;

            var databasePath = Path.Combine(storeDirectory, "queue.db40");

            this._queuePersistence = Db4oEmbedded.OpenFile(databasePath);
        }

        public void Enqueue<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new()
        {
            var queuedMessage = new QueuedMessage<TMessage> { QueuedAt = DateTime.Now, Peer = peer, Message = message, HasSent = false };

            this._queuePersistence.Store(queuedMessage);

            this._queuePersistence.Commit();
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
