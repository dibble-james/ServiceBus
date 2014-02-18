namespace ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base implementation for <see cref="IMessage"/>.
    /// </summary>
    [Serializable]
    public abstract class MessageBase : IMessage
    {
        /// <summary>
        /// Initialises the <see cref="MessageBase"/> class.
        /// </summary>
        protected MessageBase()
        {
        }

        /// <summary>
        /// Initialises the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to extract data. </param>
        /// <param name="context">The source (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        protected MessageBase(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Gets the identifier of this <see cref="IMessage"/>.
        /// </summary>
        public abstract string MessageType { get; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MessageType", this.MessageType);
        }
    }
}