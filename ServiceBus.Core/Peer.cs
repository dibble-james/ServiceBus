namespace ServiceBus
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A remote <see cref="IServiceBus"/> instance.
    /// </summary>
    [Serializable]
    public sealed class Peer : IPeer
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Peer"/> class.
        /// </summary>
        /// <param name="address">The remote location of the <see cref="IPeer"/>.</param>
        public Peer(Uri address)
        {
            this.PeerAddress = address;
        }

        private Peer(SerializationInfo info, StreamingContext context)
        {
            this.PeerAddress = info.GetValue("PeerAddress", typeof(Uri)) as Uri;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Uri"/> of the location of the <see cref="IPeer"/>.
        /// </summary>
        public Uri PeerAddress { get; set; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PeerAddress", this.PeerAddress);
        }
    }
}
