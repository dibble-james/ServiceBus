namespace ServiceBus.Transport.Http
{
    using Messaging;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;

    using Newtonsoft.Json;

    public class HttpTransporter : ITransporter
    {
        private const string actionBase = "service-bus";

        private readonly HttpClient _client;
        private readonly IMessageSerialiser _serialiser;

        public HttpTransporter(HttpClient client, IMessageSerialiser serialiser)
        {
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

        public void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage
        {
            const string action = "message";

            var fullActionPath = Path.Combine(peerToRecieve.PeerAddress.AbsolutePath, HttpTransporter.actionBase, action);

            this.ExecutePostRequest(new Uri(fullActionPath), message);
        }

        private void ExecutePostRequest<TMessageOut>(Uri address, TMessageOut messageToPost)
            where TMessageOut : class, IMessage
        {
            var messageAsJson = JsonConvert.SerializeObject(messageToPost);

            var content = new StringContent(messageAsJson, Encoding.UTF8, "application/json");

            this._client.PostAsync(address, content);
        }

        private TMessage ExecuteGetRequest<TMessage>(Uri address) where TMessage : class, IMessage
        {
            var response = this._client.GetAsync(address);

            if (!response.Result.IsSuccessStatusCode)
            {
                return null;
            }

            return this._serialiser.Deserialise<TMessage>(response.Result.Content.ReadAsStringAsync().Result);
        }
    }
}