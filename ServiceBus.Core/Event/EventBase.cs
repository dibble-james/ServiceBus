namespace ServiceBus.Event
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// A base class for an event message so the pattern can be transparently implemented.
    /// </summary>
    /// <typeparam name="TEvent">A self referencing type.</typeparam>
    public abstract class EventBase<TEvent> : IEvent<TEvent> where TEvent : class, IEvent<TEvent>
    {
        /// <summary>
        /// A subscription for raising this <see cref="IEvent"/> instance when it is received.
        /// </summary>
        public event Action<TEvent> EventRaised;

        /// <summary>
        /// Gets the identifier of this <see cref="IMessage"/>.
        /// </summary>
        public abstract string MessageType { get; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

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
