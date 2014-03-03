namespace ServiceBus.Transport.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using ServiceBus.Messaging;
    using ServiceBus.Queueing;

    /// <summary>
    /// An <see cref="ITransporter"/> that uses the HTTP protocol.
    /// </summary>
    internal class HttpTransporter : ITransporter
    {
        private const string ActionBase = "service-bus";

        private readonly HttpClient _client;
        private readonly IMessageSerialiser _serialiser;

        private bool _disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="HttpTransporter"/> class.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> to use.</param>
        /// <param name="serialiser">The <see cref="IMessageSerialiser"/> to use.</param>
        internal HttpTransporter(HttpClient client, IMessageSerialiser serialiser)
        {
            this._disposed = false;

            this._client = client;
            this._serialiser = serialiser;
        }

        /// <summary>
        /// An event raised when an <see cref="IMessage"/> is received by the <see cref="ITransporter"/>.
        /// </summary>
        public event Action<IMessage> MessageRecieved;

        /// <summary>
        /// An event raised when an <see cref="IMessage"/> is successfully exported.
        /// </summary>
        public event Action<QueuedMessage> MessageSent;

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
        /// Transport a <see cref="QueuedMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="IMessage"/> to transport.</typeparam>
        /// <param name="peerToRecieve">The <see cref="IPeer"/> that should receive the <paramref name="message"/>.</param>
        /// <param name="message">The <see cref="IMessage"/> to transport.</param>
        public async Task SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : QueuedMessage
        {
            const string action = "message";

            var fullActionPath = new Uri(peerToRecieve.PeerAddress, Path.Combine(ActionBase, action));

            try
            {
                var result = await this.ExecutePostRequest(fullActionPath, message.Message);

                if (result.IsSuccessStatusCode && this.MessageSent != null)
                {
                    this.MessageSent(message);
                }
            }
            catch
            {
                // For now just swallow exceptions.  TODO: Log a failed send attempt
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

            if (this.MessageRecieved != null)
            {
                await Task.Factory.StartNew(() => this.MessageRecieved(message));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!this._disposed)
            {
                this.Dispose(true);
            }

            this._disposed = true;
        }

        private Task<HttpResponseMessage> ExecutePostRequest<TMessageOut>(Uri address, TMessageOut messageToPost)
            where TMessageOut : class, IMessage
        {
            var serialisedMessage = this.Serialiser.Serialise(messageToPost);

            var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", serialisedMessage } });

            return this._client.PostAsync(address, content);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this._client.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}