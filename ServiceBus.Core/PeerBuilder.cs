namespace ServiceBus
{
    internal class PeerBuilder
    {
        public void BuildAndRegister(IPeer peer, ServiceBus bus)
        {
            bus.RegisterPeer(peer);
        }
    }
}
