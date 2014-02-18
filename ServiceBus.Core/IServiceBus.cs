namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ServiceBus.Event;
    using ServiceBus.Messaging;
    using ServiceBus.Transport;

    /// <summary>
    /// Implementing classes define an object for orchestrating reliable message and event managing.
    /// </summary>
    public interface IServiceBus : IPeer, IDisposable
    {
        /// <summary>
        /// Gets the <see cref="IPeer"/>s that are known to the <see cref="IServiceBus"/>.
        /// </summary>
        IEnumerable<IPeer> Peers { get; }

        /// <summary>
        /// Gets the <see cref="IMessageSerialiser"/> that is registered to the <see cref="IServiceBus"/>es <see cref="ITransporter"/>.
        /// </summary>
        IMessageSerialiser Serialiser { get; }

        /// <summary>
        /// Gets the <see cref="ITransporter"/> that is registered to the <see cref="IServiceBus"/>.
        /// </summary>
        ITransporter Transporter { get; }

        /// <summary>
        /// Directly send an <paramref name="message"/> to a given <paramref name="peer"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the <see cref="IMessage"/> to send.</typeparam>
        /// <param name="peer">The peer who should receive the <paramref name="message"/>.</param>
        /// <param name="message">The <see cref="IMessage"/> to send.</param>
        /// <returns>An awaitable object representing the send operation.</returns>
        Task SendAsync<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new();

        /// <summary>
        /// Raise an instance of <typeparamref name="TEvent"/> to the <see cref="P:Peers"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IEvent"/> to raise.</typeparam>
        /// <param name="event">The event data to publish.</param>
        /// <returns>An awaitable object representing the publish operation.</returns>
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent<TEvent>, new();

        /// <summary>
        /// Register an instance of an <see cref="IEventHandler{TEvent}"/> to the <see cref="IServiceBus"/> so it can handle a <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of event the <paramref name="eventHandler"/> handles.</typeparam>
        /// <param name="eventHandler">The <see cref="IEventHandler{TEvent}"/> to register.</param>
        /// <returns>The <see cref="IServiceBus"/>.</returns>
        IServiceBus Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent<TEvent>, new();

        /// <summary>
        /// Transmit all queued messages to the given <paramref name="peer"/>.
        /// </summary>
        /// <param name="peer">The peer to synchronise.</param>
        /// <returns>An awaitable object representing the synchronise operation.</returns>
        Task SynchroniseAsync(IPeer peer);

        /// <summary>
        /// Add a remote instance of <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="peer">The known <see cref="IServiceBus"/> location.</param>
        /// <returns>The <see cref="IServiceBus"/>.</returns>
        Task<IServiceBus> WithPeerAsync(Uri peer);

        /// <summary>
        /// Register an <see cref="IEndpoint"/> to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="endpoint">The <see cref="IEndpoint"/> to register.</param>
        /// <returns>The <see cref="IServiceBus"/>.</returns>
        IServiceBus WithLocalEndpoint(IEndpoint endpoint);
    }
}
