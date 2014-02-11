namespace ServiceBus.Transport
{
    using System;

    using ServiceBus.Events;
    using ServiceBus.Messaging;
    using ServiceBus.Queueing;

    /// <summary>
    /// Implementing classes define methods for moving <see cref="IMessage"/>s around. 
    /// </summary>
    public interface ITransporter : IDisposable
    {
        /// <summary>
        /// An event raised when an <see cref="IMessage"/> is received by the <see cref="ITransporter"/>.
        /// </summary>
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// An event raised when an <see cref="IMessage"/> is successfully exported.
        /// </summary>
        event EventHandler<MessageSentEventArgs> MessageSent;

        /// <summary>
        /// Gets the <see cref="IMessageSerialiser"/> that is registered to this <see cref="ITransporter"/>.
        /// </summary>
        IMessageSerialiser Serialiser { get; }

        /// <summary>
        /// Take the raw content of the message, de-serialize it, and pass it back to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="messageContent">The raw content of the message.</param>
        void Recieve(string messageContent);

        /// <summary>
        /// Transport a <see cref="QueuedMessage"/>.
        /// </summary>
        /// <param name="sender">The object that raised the <see cref="E:IQueueManager.MessageQueued"/> event.</param>
        /// <param name="args">The <see cref="QueuedMessage"/>.</param>
        void SendMessage(object sender, MessageQueuedEventArgs args);

        /// <summary>
        /// Transport a <see cref="QueuedMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="IMessage"/> to transport.</typeparam>
        /// <param name="peerToRecieve">The <see cref="IPeer"/> that should receive the <paramref name="message"/>.</param>
        /// <param name="message">The <see cref="IMessage"/> to transport.</param>
        void SendMessage<TMessage>(IPeer peerToRecieve, TMessage message) where TMessage : QueuedMessage;
    }
}