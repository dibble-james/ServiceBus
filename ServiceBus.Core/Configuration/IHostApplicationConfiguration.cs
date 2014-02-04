namespace ServiceBus.Configuration
{
    using System;

    public interface IHostApplicationConfiguration
    {
        IServiceBus Build();

        IHostApplicationConfiguration WithPeer(Uri peer);

        IHostApplicationConfiguration WithLocalEndpoint(IEndpoint endpoint);
    }
}
