namespace ServiceBus.Messaging
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Implementing classes define an <see cref="System.Runtime.Serialization.ISerializable"/> message that can be transported
    /// between <see cref="IServiceBus" />s.
    /// </summary>
    public interface IMessage : ISerializable
    {
        /// <summary>
        /// Gets the identifier of this <see cref="IMessage"/>.
        /// </summary>
        string MessageType { get; }
    }
}
