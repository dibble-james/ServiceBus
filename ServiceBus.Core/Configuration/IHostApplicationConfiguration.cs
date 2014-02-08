namespace ServiceBus.Configuration
{
    using System;

    using ServiceBus.Event;

    public interface IHostApplicationConfiguration
    {
        IServiceBus Build();

        IHostApplicationConfiguration WithPeer(Uri peer);

        IHostApplicationConfiguration WithLocalEndpoint(IEndpoint endpoint);

        IHostApplicationConfiguration Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent;
    }
}
