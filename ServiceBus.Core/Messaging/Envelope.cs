// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Evelope.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base class for an envelope to expose non-generic envelope properties.
    /// </summary>
    public abstract class Envelope
    {
        /// <summary>
        /// Gets or sets the <see cref="IPeer" /> that created this <see cref="Envelope" />.
        /// </summary>
        public IPeer Sender { get; set; }

        /// <summary>
        /// Gets or sets the intended recipient <see cref="IPeer" /> of this <see cref="Envelope" />.
        /// </summary>
        public IPeer Recipient { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IMessage" /> this <see cref="Envelope" /> contains.
        /// </summary>
        public IMessage Message { get; set; }
    }

    /// <summary>
    /// A container for <see cref="IMessage" /> meta-data.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this <see cref="Envelope{TMessage}" /> contains.</typeparam>
    [Serializable]
    public sealed class Envelope<TMessage> : Envelope, ISerializable where TMessage : class, IMessage
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Envelope"/> class.
        /// </summary>
        public Envelope()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Envelope"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public Envelope(SerializationInfo info, StreamingContext context)
        {
            this.Sender = info.GetValue("Sender", typeof(Peer)) as IPeer;
            this.Recipient = info.GetValue("Recipient", typeof(Peer)) as IPeer;
            this.Message = info.GetValue("Message", typeof(TMessage)) as TMessage;
        }

        /// <summary>
        /// Gets or sets <typeparamref cref="TMessage" /> this <see cref="Envelope" /> contains.
        /// </summary>
        public new TMessage Message 
        {
            get
            {
                return base.Message as TMessage;
            }
            set
            {
                base.Message = value;
            } 
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Sender", this.Sender);
            info.AddValue("Recipient", this.Recipient);
            info.AddValue("Message", this.Message, typeof(TMessage));
        }
    }
}