namespace ServiceBus.Core.Events
{
    using System;
    using System.Runtime.Serialization;

    using ServiceBus.Event;
    using ServiceBus.Messaging;

    public sealed class PeerConnectedEvent : EventBase<PeerConnectedEvent>
    {
        public const string PeerConnectedEventMessageType = "PeerConnectedEvent";
        
        public PeerConnectedEvent()
        {
        }

        public PeerConnectedEvent(SerializationInfo info, StreamingContext context)
        {
            this.ConnectedPeer = info.GetValue("ConnectedPeer", typeof(Peer)) as IPeer;
        }

        internal IPeer ConnectedPeer { get; set; }

        /// <summary>
        /// Gets the identifier of this <see cref="IMessage"/>.
        /// </summary>
        public override string MessageType
        {
            get
            {
                return PeerConnectedEventMessageType;
            }
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ConnectedPeer", this.ConnectedPeer);
        }
    }
}
