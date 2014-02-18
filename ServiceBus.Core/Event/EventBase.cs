namespace ServiceBus.Event
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using ServiceBus.Messaging;

    /// <summary>
    /// A base class for an event message so the pattern can be transparently implemented.
    /// </summary>
    /// <typeparam name="TEvent">A self referencing type.</typeparam>
    [Serializable]
    public abstract class EventBase<TEvent> : MessageBase, IEvent<TEvent> where TEvent : class, IEvent<TEvent>
    {
        /// <summary>
        /// Initialises the <see cref="EventBase{TEvent}"/> class.
        /// </summary>
        protected EventBase()
        {
        }

        /// <summary>
        /// Initialises the <see cref="EventBase{TEvent}"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to extract data. </param>
        /// <param name="context">The source (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        protected EventBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// A subscription for raising this <see cref="IEvent"/> instance when it is received.
        /// </summary>
        public event Action<TEvent> EventRaised;

        /// <summary>
        /// Raise the <see cref="E:EventRaised"/> event.
        /// </summary>
        /// <returns>An awaitable object representing the RaiseLocal operation.</returns>
        public async Task RaiseLocalAsync()
        {
            if (this.EventRaised != null)
            {
                await Task.Factory.StartNew(() => this.EventRaised(this as TEvent));
            }
        }
    }
}
