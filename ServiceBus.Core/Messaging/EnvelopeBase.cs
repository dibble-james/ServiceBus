// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvelopeBase.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using System;

    /// <summary>
    /// A base class for an envelope to expose non-generic envelope properties.
    /// </summary>
    public abstract class EnvelopeBase
    {
        /// <summary>
        /// Gets or sets the <see cref="IPeer" /> that created this <see cref="EnvelopeBase" />.
        /// </summary>
        public IPeer Sender { get; set; }

        /// <summary>
        /// Gets or sets the intended recipient <see cref="IPeer" /> of this <see cref="EnvelopeBase" />.
        /// </summary>
        public IPeer Recipient { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IMessage" /> this <see cref="EnvelopeBase" /> contains.
        /// </summary>
        public IMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.DateTime"/> that the message was passed to 
        /// the <see cref="IServiceBus"/>.
        /// </summary>
        public DateTime MessageCreated { get; set; }
    }
}