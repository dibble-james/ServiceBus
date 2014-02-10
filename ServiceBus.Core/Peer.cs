namespace ServiceBus
{
    using System;

    public class Peer : IPeer
    {
        public Peer(Uri address)
        {
            this.PeerAddress = address;
        }

        public Uri PeerAddress { get; set; }
    }
}
