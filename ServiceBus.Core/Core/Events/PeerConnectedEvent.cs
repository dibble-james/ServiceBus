namespace ServiceBus.Core.Events
{
    using System;
    using System.Runtime.Serialization;

    using ServiceBus.Event;

    /// <summary>
    /// A service bus system event to register a peer to it's own known peer network.  This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class PeerConnectedEvent : EventBase
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PeerConnectedEvent"/> class.
        /// </summary>
        public PeerConnectedEvent()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PeerConnectedEvent"/> class from a serialiser.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to extract data. </param>
        /// <param name="context">The source (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        private PeerConnectedEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.ConnectedPeer = info.GetValue("ConnectedPeer", typeof(Peer)) as IPeer;
        }

        /// <summary>
        /// Gets or sets the source <see cref="IPeer"/> that this event relates too.
        /// </summary>
        public IPeer ConnectedPeer { get; set; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ConnectedPeer", this.ConnectedPeer);
        }
    }
}
