namespace ServiceBus.Configuration
{
    using System;

    using ServiceBus.Event;

    /// <summary>
    /// Implementing classes define the configuration for the host application type of the <see cref="IServiceBus"/>.
    /// </summary>
    public interface IHostApplicationConfiguration
    {
        /// <summary>
        /// Build an instance of <see cref="IServiceBus"/> with all the information previously set.
        /// </summary>
        /// <returns>An <see cref="IServiceBus"/> instance.</returns>
        IServiceBus Build();

        /// <summary>
        /// Add a remote instance of <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="peer">The known <see cref="IServiceBus"/> location.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
        IHostApplicationConfiguration WithPeer(Uri peer);

        /// <summary>
        /// Register an <see cref="IEndpoint"/> to the <see cref="IServiceBus"/>.
        /// </summary>
        /// <param name="endpoint">The <see cref="IEndpoint"/> to register.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
        IHostApplicationConfiguration WithLocalEndpoint(IEndpoint endpoint);

        /// <summary>
        /// Register an <see cref="IEventHandler"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IEvent"/> this <see cref="IEventHandler"/> handles.</typeparam>
        /// <param name="eventHandler">The <see cref="IEventHandler"/> to register.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
        IHostApplicationConfiguration Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent<TEvent>;
    }
}
