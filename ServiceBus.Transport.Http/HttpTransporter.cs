namespace ServiceBus.Transport.Http
{
    using Messages;
    using Messaging;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpTransporter : ITransporter
    {
        private const string actionBase = "service-bus";

        private readonly HttpClient _client;

        public HttpTransporter(HttpClient client)
        {
            this._client = client;
        }

        public async Task<AvailableEndpointsMessage> RequestEnpoints(IPeer peer)
        {
            const string action = "peer/endpoints";

            var fullActionPath = Path.Combine(peer.PeerAddress.AbsolutePath, HttpTransporter.actionBase, action);

            var message = await this.ExuecuteGetRequestAsync<AvailableEndpointsMessage>(new Uri(fullActionPath));

            return message;
        }

        public async Task SendMessageAsync<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage
        {
            const string action = "message/recieve";

            var fullActionPath = Path.Combine(peerToRecieve.PeerAddress.AbsolutePath, HttpTransporter.actionBase, action);

            var response = await this.ExuecutePostRequestAsync<TMessage, ResponseMessage>(peerToRecieve.PeerAddress, message);
        }

        private async Task<TMessageBack> ExuecutePostRequestAsync<TMessageOut, TMessageBack>(Uri address, TMessageOut messageToPost) 
            where TMessageOut : class, IMessage
            where TMessageBack : class, IMessage
        {
            var response = await this._client.GetAsync(address);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return this.DeserialiseMessage<TMessageBack>(await response.Content.ReadAsStringAsync());
        }

        private async Task<TMessage> ExuecuteGetRequestAsync<TMessage>(Uri address) where TMessage : class, IMessage
        {
            var response = await this._client.GetAsync(address);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return this.DeserialiseMessage<TMessage>(await response.Content.ReadAsStringAsync());
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