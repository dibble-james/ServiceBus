// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventSubscrption.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Queueing.Ftp
{
    using System.Globalization;

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

            var messageLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/queue/{1}.msg",
                EscapePeerAddress(envelope.Recipient),
                queuedMessage.QueuedAt.ToFileTimeUtc());

            var messageContent = this._messageSerialiser.Serialise(envelope);

            await this._ftpClient.PutMessage(new Uri(messageLocation, UriKind.Relative), messageContent);

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        private static string EscapePeerAddress(IPeer peer)
        {
            return peer.PeerAddress.AbsoluteUri.Replace('/', '~');
        }
    }
}
