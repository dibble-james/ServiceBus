namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Net.FtpClient;
    using System.Threading.Tasks;

    using ServiceBus.Messaging;
    using ServiceBus.Queueing;

    /// <summary>
    /// A <see cref="ITransporter"/> for sending messages to <see cref="IPeer"/>s using the File Transfer Protocol (FTP).
    /// </summary>
    public class FtpTransporter : ITransporter
    {
        private readonly IMessageSerialiser _serialiser;
        private readonly FtpClient _client;

        /// <summary>
        /// Initialises a new instance of the <see cref="FtpTransporter"/> class.
        /// </summary>
        /// <param name="client">The <see cref="FtpClient"/> to use to send messages.</param>
        /// <param name="messageSerialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        public FtpTransporter(FtpClient client, IMessageSerialiser messageSerialiser)
        {
            this._client = client;
            this._serialiser = messageSerialiser;
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
                await Task.Factory.StartNew(null);
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
            this._client.Dispose();
        }
    }
}
