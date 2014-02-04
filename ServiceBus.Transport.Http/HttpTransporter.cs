namespace ServiceBus.Transport.Http
{
    using Messages;
    using Messaging;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public class HttpTransporter : ITransporter
    {
        private const string actionBase = "service-bus";

        private readonly HttpClient _client;

        public HttpTransporter(HttpClient client)
        {
            this._client = client;
        }

        public void RequestEnpoints(IPeer peer)
        {
            const string action = "peer/endpoints";

            var fullActionPath = Path.Combine(peer.PeerAddress.AbsoluteUri, HttpTransporter.actionBase, action);

            var message = this.ExuecuteGetRequestAsync<AvailableEndpointsMessage>(new Uri(fullActionPath));

        }

        public void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage
        {
            const string action = "message/recieve";

            var fullActionPath = Path.Combine(peerToRecieve.PeerAddress.AbsolutePath, HttpTransporter.actionBase, action);

            this.ExuecutePostRequestAsync<TMessage, ResponseMessage>(peerToRecieve.PeerAddress, message);
        }

        private TMessageBack ExuecutePostRequestAsync<TMessageOut, TMessageBack>(Uri address, TMessageOut messageToPost) 
            where TMessageOut : class, IMessage
            where TMessageBack : class, IMessage
        {
            var response = this._client.GetAsync(address);

            if (!response.Result.IsSuccessStatusCode)
            {
                return null;
            }

            return this.DeserialiseMessage<TMessageBack>(response.Result.Content.ReadAsStringAsync().Result);
        }

        private TMessage ExuecuteGetRequestAsync<TMessage>(Uri address) where TMessage : class, IMessage
        {
            var response = this._client.GetAsync(address);

            if (!response.Result.IsSuccessStatusCode)
            {
                return null;
            }

            return this.DeserialiseMessage<TMessage>(response.Result.Content.ReadAsStringAsync().Result);
        }

        private TMessage DeserialiseMessage<TMessage>(string messageContent) where TMessage : class, IMessage
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(messageContent)))
            {
                var serialiser = new DataContractJsonSerializer(typeof(TMessage));

                var message = serialiser.ReadObject(ms);

                return message as TMessage;
            }
        }
    }
}