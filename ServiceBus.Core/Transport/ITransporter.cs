namespace ServiceBus.Transport
{
    using System;

    using ServiceBus.Messaging;

    public interface ITransporter : IDisposable
    {
        IMessageSerialiser Serialiser { get; }

        void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : class, IMessage;
    }
}