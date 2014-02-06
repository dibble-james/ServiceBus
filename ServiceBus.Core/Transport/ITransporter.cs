namespace ServiceBus.Transport
{
    using Messaging;

    public interface ITransporter
    {
        IMessageSerialiser Serialiser { get; }

        void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage;
    }
}