namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using ServiceBus.Messaging;
    using ServiceBus.Queueing;


    /// <summary>
    /// A <see cref="ITransporter"/> for sending messages to <see cref="IPeer"/>s using the File Transfer Protocol (FTP).
    /// </summary>
    public class FtpTransporter : ITransporter
    {
        private readonly IMessageSerialiser _serialiser;
        private readonly IFtpClientFactory _clientFactory;
        private readonly FileSystemWatcher _messageRecievedWatcher;

        /// <summary>
        /// Initialises a new instance of the <see cref="FtpTransporter"/> class.
        /// </summary>
        /// <param name="clientFactory">The <see cref="FtpMessageSender"/> to use to send messages.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        /// <param name="pathToReciever">The full file path of the location this peers FTP server is mapped to receive messages.</param>
        public FtpTransporter(IFtpClientFactory clientFactory, IMessageSerialiser messageSerialiser, string pathToReciever)
        {
            this._clientFactory = clientFactory;
            this._serialiser = messageSerialiser;
            this._messageRecievedWatcher = new FileSystemWatcher(pathToReciever);

            this._messageRecievedWatcher.Created += this.MessageReceived;
        }

        /// <summary>
        /// An event raised when an <see cref="IMessage"/> is received by the <see cref="ITransporter"/>.
        /// </summary>
        public event Action<EnvelopeBase, string> MessageRecieved;

        /// <summary>
        /// An event raised when an <see cref="IMessage"/> is successfully exported.
        /// </summary>
        public event Action<QueuedMessage, string> MessageSent;

        /// <summary>
        /// An event raised when an <see cref="IMessage"/> could not be sent.
        /// </summary>
        public event Action<Exception, QueuedMessage> MessageFailedToSend;

        /// <summary>
        /// Gets the <see cref="IMessageSerialiser"/> that is registered to this <see cref="ITransporter"/>.
        /// </summary>
        public IMessageSerialiser Serialiser
        {
            get 
            {
                return this._serialiser;
            }
        }

        /// <summary>
        /// Take the raw content of the message, de-serialize it, and pass it back to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="messageContent">The raw content of the message.</param>
        /// <returns>An awaitable object representing the receive operation.</returns>
        public async Task ReceiveAsync(string messageContent)
        {
            var message = this.Serialiser.Deserialise(messageContent);

            if (message != null && this.MessageRecieved != null)
            {
                await Task.Factory.StartNew(() => this.MessageRecieved(message, messageContent));
            }
        }

        /// <summary>
        /// Transport a <see cref="QueuedMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to transport.</param>
        /// <returns>An awaitable object representing the send operation.</returns>
        public async Task SendMessageAsync(QueuedMessage message)
        {
            try
            {
                var recipient = message.Envelope.Recipient as FtpPeer;

                if (recipient == null)
                {
                    throw new InvalidOperationException("The FTP Transporter cannot send a message to a non FtpPeer.");
                }

                var clientConnectTask = this._clientFactory.ConnectAsync(recipient);

                var messageContent = this.Serialiser.Serialise(message.Envelope);

                var client = await clientConnectTask;

                await client.SendMessageAsync(messageContent);
            }
            catch (Exception exception)
            {
                if (this.MessageFailedToSend != null)
                {
                    this.MessageFailedToSend(exception, message);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this._messageRecievedWatcher.Dispose();
        }

        private void MessageReceived(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created)
            {
                return;
            }

            var messageContents = File.ReadAllLines(e.FullPath);

            Task.Factory.StartNew(async () => await this.ReceiveAsync(string.Join(string.Empty, messageContents)));
        }
    }
}
