namespace ServiceBus.Messaging
{
    using System;

    /// <summary>
    /// Implementing classes define methods to transform message data to or from raw strings.
    /// </summary>
    public interface IMessageSerialiser
    {
        /// <summary>
        /// An event raised when the <see cref="IMessageSerialiser"/> encounters a message
        /// not registered to the <see cref="MessageTypeDictionary"/>.
        /// </summary>
        event Action<string> UnrecognisedMessageReceived;

        /// <summary>
        /// Gets the message type mappings.
        /// </summary>
        MessageTypeDictionary MessageTypeDictionary { get; }

        /// <summary>
        /// Take raw message content and transform it into its concrete implementation.
        /// </summary>
        /// <param name="messageContent">The raw message content.</param>
        /// <returns>The concrete message data.</returns>
        IMessage Deserialise(string messageContent);

        /// <summary>
        /// Transform a <typeparamref name="TMessage"/> into raw message data.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="IMessage"/> to transform.</typeparam>
        /// <param name="message">The message to transform.</param>
        /// <returns>The raw message data.</returns>
        string Serialise<TMessage>(TMessage message) where TMessage : class, IMessage;
    }
}