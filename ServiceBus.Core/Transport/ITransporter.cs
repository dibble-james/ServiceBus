namespace ServiceBus.Transport
{
    using Messages;
    using Messaging;

    public interface ITransporter
    {
        AvailableEndpointsMessage RequestEnpoints(IPeer peer);

        void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage;
    }
}