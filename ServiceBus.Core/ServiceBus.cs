namespace ServiceBus
{
    using System;
    using System.Collections.Generic;

    using Messaging;

    using Transport;

    public sealed class ServiceBus : IServiceBus
    {
        private readonly IEnumerable<IPeer> _peers;
        private readonly object _peersLock;
        private readonly IEnumerable<IEndpoint> _endpoints;
        private readonly object _endpointsLock;
        private readonly ITransporter _transport;

        public ServiceBus(Uri hostAddress, ITransporter transporter, IEnumerable<IEndpoint> endpoints, IEnumerable<IPeer> peers)
        {
            this._peersLock = new object();
            this._endpointsLock = new object();

            this.HostAddress = hostAddress;
            this._endpoints = endpoints;
            this._peers = peers;
            this._transport = transporter;
        }

        public Uri HostAddress { get; private set; }

        public IEnumerable<IEndpoint> LocalEndpoints
        {
            get
            {
                lock (this._endpointsLock)
                {
                    return this._endpoints;
                }
            }
        }

        public IMessageSerialiser Serialiser
        {
            get
            {
                return this._transport.Serialiser;
            }
        }

        public IEnumerable<IPeer> Peers
        {
            get
            {
                lock (this._peersLock)
                {
                    return this._peers;
                }
            }
        }
    }
}
