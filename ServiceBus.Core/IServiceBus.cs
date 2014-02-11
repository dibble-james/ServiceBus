namespace ServiceBus
{
    using System;
    using System.Collections.Generic;

    using ServiceBus.Event;
    using ServiceBus.Messaging;
    using ServiceBus.Transport;

    /// <summary>
    /// Implementing classes define methods for interacting with a messaging system with event notifications.
    /// </summary>
    public interface IServiceBus : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="System.Uri"/> that this <see cref="IServiceBus"/> is hosted upon.
        /// </summary>
        Uri HostAddress { get; }

        /// <summary>
        /// Gets the <see cref="IPeer"/>s that are known to the <see cref="IServiceBus"/>.
        /// </summary>
        IEnumerable<IPeer> Peers { get; }

        /// <summary>
        /// Gets the <see cref="IEndpoint"/>s that are known to the <see cref="IServiceBus"/>.
        /// </summary>
        IEnumerable<IEndpoint> LocalEndpoints { get; }

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
        void Send<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new();

        /// <summary>
        /// Raise an instance of <typeparamref name="TEvent"/> to the <see cref="P:Peers"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IEvent"/> to raise.</typeparam>
        /// <param name="event">The event data to publish.</param>
        void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent, new();

        /// <summary>
        /// Register an instance of an <see cref="IEventHandler{TEvent}"/> to the <see cref="IServiceBus"/> so it can handle a <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of event the <paramref name="eventHandler"/> handles.</typeparam>
        /// <param name="eventHandler">The <see cref="IEventHandler{TEvent}"/> to register.</param>
        void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent, new();
    }
}
