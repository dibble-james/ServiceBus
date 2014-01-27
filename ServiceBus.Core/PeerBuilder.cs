namespace ServiceBus
{
    public class PeerBuilder
    {
        public void BuildAndRegister(IPeer peer, IServiceBus bus)
        {
            peer.DiscoverRemoteEndpoints();

            bus.RegisterPeer(peer);
        }
    }
}
