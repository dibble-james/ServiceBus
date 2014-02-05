namespace ServiceBus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    public class HostApplicationConfiguration : IHostApplicationConfiguration
    {
        private readonly ITransportConfiguration _transportConfiguration;
        private readonly ICollection<IPeer> _peers;
        private readonly ICollection<IEndpoint> _endpoints; 

        public HostApplicationConfiguration(ITransportConfiguration transportConfiguration)
        {
            this._transportConfiguration = transportConfiguration;
            this._peers = new Collection<IPeer>();
            this._endpoints = new Collection<IEndpoint>();
        }

        public IServiceBus Build()
        {
            return new ServiceBus(this._transportConfiguration.HostAddressConfiguration.HostAddress, this._transportConfiguration.Transporter, this._endpoints, this._peers);
        }

        public IHostApplicationConfiguration WithPeer(Uri peer)
        {
            var newPeer = new Peer(peer);

            Task.Factory.StartNew(() => this._transportConfiguration.Transporter.RequestEnpoints(newPeer));

            this._peers.Add(newPeer);

            return this;
        }

        public IHostApplicationConfiguration WithLocalEndpoint(IEndpoint endpoint)
        {
            this._endpoints.Add(endpoint);

            return this;
        }
    }
}
