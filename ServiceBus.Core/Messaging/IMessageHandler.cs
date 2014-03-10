namespace ServiceBus.Messaging
{
    using System.Threading.Tasks;

    /// <summary>
    /// Implementing classes can be used to receive messages of <typeparamref name="TMessage"/> type.
    /// </summary>
    /// <typeparam name="TMessage">The type of <see cref="IMessage"/> this <see cref="IMessageHandler{TMessage}"/> can receive.</typeparam>
    public interface IMessageHandler<TMessage> where TMessage : class, IMessage
    {
        /// <summary>
        /// Invoke services to deal with this <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="message">The <typeparamref name="TMessage"/> data.</param>
        /// <returns>An awaitable object representing the handling operation.</returns>
        Task ProcessMessageAsync(Envelope<TMessage> message);
    }
}