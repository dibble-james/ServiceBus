namespace ServiceBus.Transport
{
    using System;

    using ServiceBus.Messaging;
    using ServiceBus.Events;
    using ServiceBus.Queueing;

    public interface ITransporter : IDisposable
    {
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        event EventHandler<MessageSentEventArgs> MessageSent;

        IMessageSerialiser Serialiser { get; }

        void Receive(string messageContent);

        void SendMessage(object sender, MessageQueuedEventArgs args);

        void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : QueuedMessage;
    }
}