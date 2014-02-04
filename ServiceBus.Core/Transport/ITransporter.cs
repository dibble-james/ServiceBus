namespace ServiceBus.Transport
{
    using Messaging;

    public interface ITransporter
    {
        void RequestEnpoints(IPeer peer);

        void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage;
    }
}