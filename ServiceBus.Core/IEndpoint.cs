namespace ServiceBus
{
    /// <summary>
    /// Implementing classes define a service that can be reached from a remote <see cref="IServiceBus"/>.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Gets the <see cref="System.Uri"/> part that defines the location of this <see cref="IEndpoint"/>.
        /// </summary>
        string EndpointPath { get; }
    }
}
