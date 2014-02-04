namespace ServiceBus.Transport.Http
{
    using System.Linq;

    using Messages;
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

        public HttpTransporter(HttpClient client)
        {
            this._client = client;
        }

        public void RequestEnpoints(IPeer peer)
        {
            const string action = "peer/endpoints";

            var fullActionPath = Path.Combine(peer.PeerAddress.AbsoluteUri, HttpTransporter.actionBase, action);

            var message = this.ExecuteGetRequestAsync<AvailableEndpointsMessage>(new Uri(fullActionPath));

            peer.RegisterEnpoints(message.Endpoints.Select(endpoint => new Endpoint(endpoint.EndpointAddress)));
        }

        public void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage
        {
            const string action = "message/recieve";

            var fullActionPath = Path.Combine(peerToRecieve.PeerAddress.AbsolutePath, HttpTransporter.actionBase, action);

            this.ExecutePostRequestAsync<TMessage, ResponseMessage>(peerToRecieve.PeerAddress, message);
        }

        private TMessageBack ExecutePostRequestAsync<TMessageOut, TMessageBack>(Uri address, TMessageOut messageToPost) 
            where TMessageOut : class, IMessage
            where TMessageBack : class, IMessage
        {
            var messageAsJson = JsonConvert.SerializeObject(messageToPost);

            var content = new StringContent(messageAsJson, Encoding.UTF8, "application/json");

            var response = this._client.PostAsync(address, content);

            if (!response.Result.IsSuccessStatusCode)
            {
                return null;
            }

            return DeserialiseMessage<TMessageBack>(response.Result.Content.ReadAsStringAsync().Result);
        }

        private TMessage ExecuteGetRequestAsync<TMessage>(Uri address) where TMessage : class, IMessage
        {
            var response = this._client.GetAsync(address);

            if (!response.Result.IsSuccessStatusCode)
            {
                return null;
            }

            return DeserialiseMessage<TMessage>(response.Result.Content.ReadAsStringAsync().Result);
        }

        private static TMessage DeserialiseMessage<TMessage>(string messageContent) where TMessage : class, IMessage
        {
            var message = JsonConvert.DeserializeObject<TMessage>(messageContent);

            return message;
        }
    }
}