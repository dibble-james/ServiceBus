// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using ServiceBus.Events;

    /// <summary>
    /// DO NOT IMPLEMENT.  Always implement <see cref="IMessageHandler{TMessage}"/> so that it can be registered.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Invoke the business logic contained by this <see cref="IMessageHandler"/>.
        /// </summary>
        /// <param name="message">The message data.</param>
        void ProcessMessage(IMessage message);
    }

    /// <summary>
    /// Implementing classes can be used to recieve messages of <typeparamref name="TMessage"/> type.
    /// </summary>
    /// <typeparam name="TMessage">The type of <see cref="IMessage"/> this <see cref="IMessageHandler{TMessage}"/> can recieve.</typeparam>
    public interface IMessageHandler<in TMessage> : IEndpoint, IMessageHandler where TMessage : class, IMessage
    {
        /// <summary>
        /// Invoke services to deal with this <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="message">The <typeparamref name="TMessage"/> data.</param>
        void ProcessMessage(TMessage message);
    }
}