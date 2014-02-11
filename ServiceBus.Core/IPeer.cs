namespace ServiceBus
{
    using System;

    /// <summary>
    /// Implementing classes define a representation of a remote <see cref="IServiceBus"/> instance.
    /// </summary>
    public interface IPeer
    {
        /// <summary>
        /// Gets the <see cref="System.Uri"/> of the location of the <see cref="IPeer"/>.
        /// </summary>
        Uri PeerAddress { get; }
    }
}
