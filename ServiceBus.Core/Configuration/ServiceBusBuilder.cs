namespace ServiceBus.Configuration
{
    using Transport;
    using System;

    public class ServiceBusBuilder : IServiceBusBuilder
    {
        private ServiceBus _instance;
        
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

            this._instance.RegisterPeer(peer);

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
