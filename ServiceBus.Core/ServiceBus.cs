namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ServiceBus : IServiceBus
    {
        private readonly ICollection<IPeer> _peers;
        private readonly object _peersLock;

        public ServiceBus()
        {
            this._peers = new Collection<IPeer>();
            this._peersLock = new object();
        }

        public Uri HostAddress { get; internal set; }

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

        public void RegisterPeer(IPeer peer)
        {
            lock (this._peersLock)
            {
                this._peers.Add(peer);
            }
        }
    }
}
