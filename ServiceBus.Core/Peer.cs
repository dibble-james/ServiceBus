namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Peer : IPeer
    {
        private readonly ICollection<IEndpoint> _endpoints;

        public Peer(Uri address)
        {
            this.PeerAddress = address;
            this._endpoints = new Collection<IEndpoint>();
        }

        public Uri PeerAddress { get; private set; }
        
        public IEnumerable<IEndpoint> Endpoints
        {
            get { return this._endpoints; }
        }

        public void RegisterEnpoints(IEnumerable<IEndpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                this._endpoints.Add(endpoint);
            }
        }
    }
}
