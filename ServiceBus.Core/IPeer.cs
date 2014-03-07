namespace ServiceBus
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Implementing classes define a representation of a remote <see cref="IServiceBus"/> instance.
    /// </summary>
    public interface IPeer : ISerializable
    {
        /// <summary>
        /// Gets the <see cref="System.Uri"/> of the location of the <see cref="IPeer"/>.
        /// </summary>
        Uri PeerAddress { get; }
    }
}
