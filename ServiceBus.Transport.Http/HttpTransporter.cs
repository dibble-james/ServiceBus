namespace ServiceBus.Transport.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;

    using ServiceBus.Messaging;
    using ServiceBus.Queueing;

    /// <summary>
    /// An <see cref="ITransporter"/> that uses the HTTP protocol.
    /// </summary>
    public class HttpTransporter : ITransporter
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
        public HttpTransporter(HttpClient client, IMessageSerialiser serialiser)
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
        public void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : QueuedMessage
        {
            const string Action = "message";

            var fullActionPath = new Uri(peerToRecieve.PeerAddress, Path.Combine(HttpTransporter.ActionBase, Action));

            try
            {
                var result = this.ExecutePostRequest(fullActionPath, message.Message);

                if (result.IsSuccessStatusCode && this.MessageSent != null)
                {
                    this.MessageSent(message);
                }
            }
            catch
            {
                // For now just swallow exceptions.
            }
        }

        /// <summary>
        /// Take the raw content of the message, de-serialize it, and pass it back to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="messageContent">The raw content of the message.</param>
        public void Recieve(string messageContent)
        {
            var message = this.Serialiser.Deserialise(messageContent);

            if (this.MessageRecieved != null)
            {
                this.MessageRecieved(message);
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

        private HttpResponseMessage ExecutePostRequest<TMessageOut>(Uri address, TMessageOut messageToPost)
            where TMessageOut : class, IMessage
        {
            var serialisedMessage = this.Serialiser.Serialise(messageToPost);

            var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", serialisedMessage } });

            var postResult = this._client.PostAsync(address, content);

            return postResult.Result;
        }

        private IMessage ExecuteGetRequest(Uri address)
        {
            var response = this._client.GetAsync(address);

            if (!response.Result.IsSuccessStatusCode)
            {
                return null;
            }

            return this._serialiser.Deserialise(response.Result.Content.ReadAsStringAsync().Result);
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