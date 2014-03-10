// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventSubscrption.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using System;
    using System.Threading.Tasks;
    
    using ServiceBus.Event;

    /// <summary>
    /// A class to represent a subscription to an <see cref="IEvent"/> type.
    /// </summary>
    /// <typeparam name="TMessage">The <see cref="IEvent"/> this <see cref="MessageSubscrption{TMessage}"/> holds.</typeparam>
    internal class MessageSubscrption<TMessage> : IMessageSubscription<TMessage>
        where TMessage : class, IMessage, new()
    {
        /// <summary>
        /// An event to raise the subscribed event.
        /// </summary>
        public event Action<Envelope<TMessage>> MessageReceived;

        /// <summary>
        /// Raise the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="envelope">The message instance that has been received.</param>
        /// <returns>An awaitable object representing the RaiseMessageRaised operation.</returns>
        public async Task RaiseMessageReceivedAsync(Envelope<TMessage> envelope)
        {
            if (this.MessageReceived != null)
            {
                await Task.Factory.StartNew(() => this.MessageReceived(envelope));
            }
        }
    }
}