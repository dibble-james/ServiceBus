namespace ServiceBus.Transport.Http
{
    using System.Net.Http;
    using System.Collections.Generic;

    using Messaging;
    using System.IO;
    using System;
    using System.Threading.Tasks;
    using System.Net;
    using System.Text;
    using System.Runtime.Serialization.Json;

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
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(messageContent)))
            {
                var serialiser = new DataContractJsonSerializer(typeof(TMessage));

                var message = serialiser.ReadObject(ms);

                return message as TMessage;
            }
        }
    }
}
