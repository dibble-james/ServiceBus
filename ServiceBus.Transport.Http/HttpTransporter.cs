namespace ServiceBus.Transport.Http
{
    using System.Collections.Generic;

    using Messaging;
    using System;
    using System.IO;
    using System.Net.Http;

    public class HttpTransporter : ITransporter
    {
        private const string actionBase = "service-bus";

        private readonly HttpClient _client;
        private readonly IMessageSerialiser _serialiser;

        private bool _disposed;

        public HttpTransporter(HttpClient client, IMessageSerialiser serialiser)
        {
            this._client = client;
            this._serialiser = serialiser;

            this._disposed = false;
        }

        public IMessageSerialiser Serialiser
        {
            get
            {
                return this._serialiser;
            }
        }

        public void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage
        {
            const string action = "message";

            var fullActionPath = new Uri(peerToRecieve.PeerAddress, Path.Combine(HttpTransporter.actionBase, action));

            this.ExecutePostRequest(fullActionPath, message);
        }

        private void ExecutePostRequest<TMessageOut>(Uri address, TMessageOut messageToPost)
            where TMessageOut : class, IMessage
        {
            var serialisedMessage = this.Serialiser.Serialise(messageToPost);

            var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", serialisedMessage } });

            this._client.PostAsync(address, content);
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

        public void Dispose()
        {
            if (!this._disposed)
            {
                this.Dispose(true);
            }
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