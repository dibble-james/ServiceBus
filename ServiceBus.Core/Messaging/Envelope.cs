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
    /// A container for <see cref="IMessage" /> meta-data.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this <see cref="Envelope{TMessage}" /> contains.</typeparam>
    [Serializable]
    public sealed class Envelope<TMessage> : EnvelopeBase, ISerializable where TMessage : class, IMessage
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EnvelopeBase"/> class.
        /// </summary>
        public Envelope()
        {
        }

        private Envelope(SerializationInfo info, StreamingContext context)
        {
            this.Sender = info.GetValue("Sender", typeof(Peer)) as IPeer;
            this.Recipient = info.GetValue("Recipient", typeof(Peer)) as IPeer;
            this.Message = info.GetValue("Message", typeof(TMessage)) as TMessage;
            this.MessageCreated = info.GetDateTime("MessageCreated");
        }

        /// <summary>
        /// Gets or sets <typeparamref name="TMessage" /> this <see cref="EnvelopeBase" /> contains.
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
            info.AddValue("MessageCreated", this.MessageCreated);
        }
    }
}