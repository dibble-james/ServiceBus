namespace ServiceBus.Configuration
{
    using System;
    using System.Threading.Tasks;

    public class ServiceBusBuilder : IServiceBusBuilder
    {
        private ServiceBus _instance;
        private readonly PeerBuilder _peerBuilder;
        
        public ServiceBusBuilder()
        {
            this._instance = new ServiceBus();
            this._peerBuilder = new PeerBuilder();
        }

        public IServiceBusBuilder WithHostAddress(Uri address)
        {
            this._instance.HostAddress = address;

            return this;
        }

        public IServiceBusBuilder WithPeer(Uri peerAddress)
        {
            var peer = new Peer(peerAddress);

            Task.Run(() => { this._peerBuilder.BuildAndRegister(peer, this._instance); });

            return this;
        }

        public IServiceBus Build()
        {
            return this._instance;
        }
    }
}
