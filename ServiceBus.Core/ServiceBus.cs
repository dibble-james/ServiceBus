namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Transport;

    public sealed class ServiceBus : IServiceBus
    {
        private readonly ICollection<IPeer> _peers;
        private readonly object _peersLock;
        private readonly IDictionary<Type, IEndpoint> _endpoints;
        private readonly object _endpointsLock;
        private ITransporter _transport;

        public ServiceBus()
        {
            this._peers = new Collection<IPeer>();
            this._peersLock = new object();
            this._endpoints = new Dictionary<Type, IEndpoint>();
            this._endpointsLock = new object();
        }

        public Uri HostAddress { get; internal set; }

        public IEnumerable<IEndpoint> LocalEndpoints { get; internal set; }

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
        
        internal void RegisterPeer(IPeer peer)
        {
            lock (this._peersLock)
            {
                this._peers.Add(peer);
            }
        }

        internal void RegisterLocalEndpoint<TEndpoint>(TEndpoint endpoint) where TEndpoint : IEndpoint
        {
            lock (this._endpointsLock)
            {
                this._endpoints.Add(typeof(TEndpoint), endpoint);
            }
        }

        internal void RegisterTransportation(ITransporter transporter)
        {
            this._transport = transporter;
        }
    }
}
