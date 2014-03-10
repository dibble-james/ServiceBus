// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventSubscrption.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// DO NOT IMPLEMENT.  This interface exists only to be able to hold <see cref="IMessageSubscription{TMessage}"/>s
    /// in a collection.
    /// </summary>
    public interface IMessageSubscription
    { 
    }

    /// <summary>
    /// A interface to represent a subscription to an <see cref="IMessage"/> type.
    /// </summary>
    /// <typeparam name="TMessage">The <see cref="IMessage"/> this <see cref="IMessageSubscription{TMessage}"/> holds.</typeparam>
    public interface IMessageSubscription<TMessage> : IMessageSubscription where TMessage : class, IMessage, new()
    {
        /// <summary>
        /// A subscription for raising this <see cref="IMessage"/> instance when it is received.
        /// </summary>
        event Action<Envelope<TMessage>> MessageReceived;

        /// <summary>
        /// Raise the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="message">The message instance that has been received.</param>
        /// <returns>An awaitable object representing the RaiseMessageRaised operation.</returns>
        Task RaiseMessageReceivedAsync(Envelope<TMessage> message);
    }
}