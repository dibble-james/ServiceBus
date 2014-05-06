// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventSubscrption.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace ServiceBus.Queueing.Ftp
{
    using ServiceBus.Messaging;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of the <see cref="IQueueManager"/> interface to queue messages using an FTP
    /// server to persist the queue.
    /// </summary>
    public class FtpQueueManager : IQueueManager
    {
        private readonly IFtpQueueClient _ftpClient;
        private readonly IMessageSerialiser _messageSerialiser;

        /// <summary>
        /// Initialises a new instance of the <see cref="FtpQueueManager"/> class.
        /// </summary>
        /// <param name="client">The <see cref="IFtpQueueClient"/> to communicate with the queue.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to persist message data.</param>
        public FtpQueueManager(IFtpQueueClient client, IMessageSerialiser messageSerialiser)
        {
            this._ftpClient = client;
            this._messageSerialiser = messageSerialiser;
        }

        /// <summary>
        /// An event that is raised when an <see cref="T:ServiceBus.Messaging.IMessage"/> is placed onto the <see cref="T:ServiceBus.IServiceBus"/> and persisted.
        /// </summary>
        public event Action<QueuedMessage> MessageQueued;

        /// <summary>
        /// Persist an <see cref="T:ServiceBus.Messaging.IMessage"/> and raise <see cref="E:IQueueManager.MessageQueue"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="T:ServiceBus.Messaging.IMessage"/> to queue.</typeparam>
        /// <param name="envelope">The message to place into the queue.</param>
        /// <returns>
        /// An awaitable object representing the enqueue operation.
        /// </returns>
        public async Task EnqueueAsync<TMessage>(Envelope<TMessage> envelope) where TMessage : class, IMessage
        {
            var queuedMessage = new QueuedMessage { QueuedAt = DateTime.Now, Envelope = envelope, HasSent = false };
            
            var messageContent = this._messageSerialiser.Serialise(envelope);

            await this._ftpClient.CreatePeerDirectoryIfNotExist(envelope.Recipient);

            await this._ftpClient.PutMessage(new Uri(queuedMessage.MessageLocation(), UriKind.Relative), messageContent);

            if (this.MessageQueued != null)
            {
                await Task.Factory.StartNew(() => this.MessageQueued(queuedMessage));
            }
        }

        /// <summary>
        /// Mark a message as sent.
        /// </summary>
        /// <param name="message">The <see cref="T:ServiceBus.Queueing.QueuedMessage"/> that was sent.</param>
        /// <param name="messageContent">The raw message content.</param>
        public void Dequeue(QueuedMessage message, string messageContent)
        {
            message.HasSent = true;

            this._ftpClient.CreatePeerDirectoryIfNotExist(message.Envelope.Recipient);

            this._ftpClient.DeleteMessage(new Uri(message.MessageLocation(), UriKind.Relative));
            
            this._ftpClient.PutMessage(new Uri(message.SentLocation(), UriKind.Relative), messageContent);
        }

        /// <summary>
        /// Retrieve the next <see cref="T:ServiceBus.Queueing.QueuedMessage"/> for the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to check for <see cref="T:ServiceBus.Queueing.QueuedMessage"/>s.</param>
        /// <returns>
        /// The next <see cref="T:ServiceBus.Queueing.QueuedMessage"/> or null if the queue for the <paramref name="peer"/> is empty.
        /// </returns>
        public QueuedMessage PeersNextMessageOrDefault(IPeer peer)
        {
            return this.PeersNextMessageOrDefault(peer, DateTime.Now);
        }

        /// <summary>
        /// Retrieve the next <see cref="T:ServiceBus.Queueing.QueuedMessage"/> for the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to check for <see cref="T:ServiceBus.Queueing.QueuedMessage"/>s.</param>
        /// <param name="messageQueuedBefore">The <see cref="T:System.DateTime"/> to find messages queued before.</param>
        /// <returns>
        /// The next <see cref="T:ServiceBus.Queueing.QueuedMessage"/> or null if the queue for the <paramref name="peer"/> is empty.
        /// </returns>
        public QueuedMessage PeersNextMessageOrDefault(IPeer peer, DateTime messageQueuedBefore)
        {
            var messages = this._ftpClient.GetFileListings(new Uri(peer.QueueLocation(), UriKind.Relative)).Result.ToList();

            if (!messages.Any(queuedDate => queuedDate > messageQueuedBefore))
            {
                return null;
            }

            var messageQueuedAt = messages.OrderBy(m => m).First(queuedDate => queuedDate > messageQueuedBefore);

            var rawEnvelope =
                this._ftpClient.GetMessage(
                    new Uri(MessageExtensions.MessageLocation(peer, messageQueuedAt))).Result;

            var envelope = this._messageSerialiser.Deserialise(rawEnvelope);

            var queuedMessage = new QueuedMessage { QueuedAt = messageQueuedAt, Envelope = envelope, HasSent = false };

            return queuedMessage;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
