namespace ServiceBus.Transport.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;

    using ServiceBus.Messaging;
    using ServiceBus.Events;
    using ServiceBus.Queueing;
    using System.Threading.Tasks;

    public class HttpTransporter : ITransporter
    {
        private const string actionBase = "service-bus";

        private readonly HttpClient _client;
        private readonly IMessageSerialiser _serialiser;

        private bool _disposed;

        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        public event EventHandler<MessageSentEventArgs> MessageSent;

        public HttpTransporter(HttpClient client, IMessageSerialiser serialiser)
        {
            this._disposed = false;

            this._client = client;
            this._serialiser = serialiser;
        }

        public IMessageSerialiser Serialiser
        {
            get
            {
                return this._serialiser;
            }
        }

        public void SendMessage(object sender, MessageQueuedEventArgs args)
        {
            this.SendMessage(args.MessageQueued.Peer, args.MessageQueued);
        }

        public void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : QueuedMessage
        {
            const string action = "message";

            var fullActionPath = new Uri(peerToRecieve.PeerAddress, Path.Combine(HttpTransporter.actionBase, action));

            var result = this.ExecutePostRequest(fullActionPath, message.Message);

            if (result.IsSuccessStatusCode && this.MessageSent != null)
            {
                this.MessageSent(this, new MessageSentEventArgs { MessageSent = message, Recipient = peerToRecieve }); 
            }
        }

        public void Receive(string messageContent)
        {
            var message = this.Serialiser.Deserialise(messageContent);

            if (this.MessageRecieved != null)
            {
                this.MessageRecieved(this, new MessageRecievedEventArgs { MessageRecieved = message });
            }
        }

        public void Dispose()
        {
            if (!this._disposed)
            {
                this.Dispose(true);
            }
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