namespace ServiceBus.Messaging
{
    using System.Threading.Tasks;

    /// <summary>
    /// DO NOT IMPLEMENT.  Always implement <see cref="IMessageHandler{TMessage}"/> so that it can be registered.
    /// </summary>
    public interface IMessageHandler
    {
    }

    /// <summary>
    /// Implementing classes can be used to receive messages of <typeparamref name="TMessage"/> type.
    /// </summary>
    /// <typeparam name="TMessage">The type of <see cref="IMessage"/> this <see cref="IMessageHandler{TMessage}"/> can receive.</typeparam>
    public interface IMessageHandler<in TMessage> : IEndpoint, IMessageHandler where TMessage : class, IMessage
    {
        /// <summary>
        /// Invoke services to deal with this <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="message">The <typeparamref name="TMessage"/> data.</param>
        /// <returns>An awaitable object representing the handling operation.</returns>
        Task ProcessMessageAsync(TMessage message);
    }
}