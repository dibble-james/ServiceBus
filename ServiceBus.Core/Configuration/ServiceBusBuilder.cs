namespace ServiceBus.Configuration
{
    using Transport;
    using System;
    using System.Threading.Tasks;

    public class ServiceBusBuilder : IServiceBusBuilder
    {
        private ServiceBus _instance;
        private readonly PeerBuilder _peerBuilder;

        public ServiceBusBuilder()
        {
            this._peerBuilder = new PeerBuilder();
        }

        public void Configure()
        {
            this._instance = new ServiceBus();
        }

        public IServiceBusBuilder WithTransport<TTransport>(TTransport transporter) where TTransport : ITransporter
        {
            this._instance.RegisterTransportation(transporter);

            return this;
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

        public IServiceBusBuilder WithLocalEndpoint<TEndpoint>(TEndpoint endpoint) where TEndpoint : IEndpoint
        {
            this._instance.RegisterLocalEndpoint(endpoint);

            return this;
        }

        public IServiceBus Build()
        {
            return this._instance;
        }
    }
}
