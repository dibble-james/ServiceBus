namespace ServiceBus.Queueing
{
    using ServiceBus.Messaging;
    using System;
    using System.IO;

    internal class QueueManager : IQueueManager
    {
        private bool _disposed;

        
        public QueueManager(string storeDirectory)
        {
            this._disposed = false;
        }

        public void Enqueue<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
            }

            this._disposed = true;
        }
    }
}
