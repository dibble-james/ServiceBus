namespace ServiceBus.Queueing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Db4objects.Db4o;
    using Db4objects.Db4o.Linq;

    using ServiceBus.Core.Events;
    using ServiceBus.Messaging;

    internal class QueueManager : IQueueManager
    {
        private readonly string _databasePath;
        private Lazy<IObjectContainer> _queuePersistence;

        private bool _disposed;
                
        public QueueManager(string storeDirectory)
        {
            this._disposed = false;

            this._databasePath = Path.Combine(storeDirectory, "queue.db40");

            this._queuePersistence = new Lazy<IObjectContainer>(() => Db4oEmbedded.OpenFile(this._databasePath));
        }

        public event Action<QueuedMessage> MessageQueued;

        public async Task EnqueueAsync<TMessage>(Envelope<TMessage> envelope) where TMessage : class, IMessage
        {
            var queuedMessage = new QueuedMessage { QueuedAt = DateTime.Now, Envelope = envelope, HasSent = false };

            this._queuePersistence.Value.Store(queuedMessage);

            await Task.Factory.StartNew(() => this._queuePersistence.Value.Commit());

            this.RecreateQueuePersistence();

            if (this.MessageQueued != null)
            {
                await Task.Factory.StartNew(() => this.MessageQueued(queuedMessage));
            }
        }

        public void Dequeue(QueuedMessage message)
        {
            var queuedMessage = this._queuePersistence.Value.AsQueryable<QueuedMessage>()
                            .FirstOrDefault(
                                qm => qm.QueuedAt == message.QueuedAt 
                                   && qm.Envelope.Recipient == message.Envelope.Recipient 
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

            this._queuePersistence.Value.Store(queuedMessage);

            this._queuePersistence.Value.Commit();

            this.RecreateQueuePersistence();
        }

        public QueuedMessage PeersNextMessageOrDefault(IPeer peer)
        {
            var nextMessage =
                this._queuePersistence.Value.AsQueryable<QueuedMessage>()
                    .OrderByDescending(qm => qm.HasSent)
                    .ThenBy(qm => qm.QueuedAt)
                    .FirstOrDefault(
                    qm => 
                           !qm.HasSent 
                        && qm.Envelope.Recipient.PeerAddress == peer.PeerAddress
                        && !(qm.Envelope.Message is PeerConnectedEvent));

            this.RecreateQueuePersistence();

            return nextMessage;
        }

        public QueuedMessage PeersNextMessageOrDefault(IPeer peer, DateTime messageQueuedBefore)
        {
            var nextMessage =
                this._queuePersistence.Value.AsQueryable<QueuedMessage>()
                    .OrderByDescending(qm => qm.HasSent)
                    .ThenBy(qm => qm.QueuedAt)
                    .FirstOrDefault(
                        qm =>    !qm.HasSent 
                              && qm.Envelope.Recipient.PeerAddress == peer.PeerAddress
                              && qm.QueuedAt < messageQueuedBefore
                              && !(qm.Envelope.Message is PeerConnectedEvent));

            this.RecreateQueuePersistence();

            return nextMessage;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed && this._queuePersistence.IsValueCreated)
            {
                this._queuePersistence.Value.Dispose();
            }

            this._disposed = true;
        }

        private void RecreateQueuePersistence()
        {
            if (this._queuePersistence.IsValueCreated)
            {
                this._queuePersistence.Value.Close();

                this._queuePersistence.Value.Dispose();

                this._queuePersistence = new Lazy<IObjectContainer>(() => Db4oEmbedded.OpenFile(this._databasePath));
            }
        }
    }
}
