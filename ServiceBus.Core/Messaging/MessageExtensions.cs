// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using System;

    /// <summary>
    /// Extension methods and helpers for the <see cref="IMessage"/> class.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Retrieve the signature of an <see cref="IMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="IMessage"/> to get a signature for.</typeparam>
        /// <returns>The <see cref="IMessage"/> signature.</returns>
        public static string MessageTypeSignature<TMessage>() where TMessage : IMessage
        {
            return MessageTypeSignature(typeof(TMessage));
        }

        /// <summary>
        /// Retrieve the signature of an <see cref="IMessage"/>.
        /// </summary>
        /// <param name="messageType">The type of <see cref="IMessage"/> to get a signature for.</param>
        /// <returns>The <see cref="IMessage"/> signature.</returns>
        public static string MessageTypeSignature(Type messageType)
        {
            return messageType.Name;
        }
    }
}