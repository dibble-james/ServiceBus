namespace ServiceBus
{
    using System;

    /// <summary>
    /// A remote <see cref="IServiceBus"/> instance.
    /// </summary>
    public class Peer : IPeer
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Peer"/> class.
        /// </summary>
        /// <param name="address">The remote location of the <see cref="IPeer"/>.</param>
        public Peer(Uri address)
        {
            this.PeerAddress = address;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Uri"/> of the location of the <see cref="IPeer"/>.
        /// </summary>
        public Uri PeerAddress { get; set; }
    }
}
