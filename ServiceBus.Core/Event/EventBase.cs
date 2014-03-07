namespace ServiceBus.Event
{
    using System;
    using System.Runtime.Serialization;

    using ServiceBus.Messaging;

    /// <summary>
    /// A base class for an event message so the pattern can be transparently implemented.
    /// </summary>
    [Serializable]
    public abstract class EventBase : MessageBase, IEvent
    {
        /// <summary>
        /// Initialises the <see cref="EventBase"/> class.
        /// </summary>
        protected EventBase()
        {
        }

        /// <summary>
        /// Initialises the <see cref="EventBase"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to extract data. </param>
        /// <param name="context">The source (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        protected EventBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
