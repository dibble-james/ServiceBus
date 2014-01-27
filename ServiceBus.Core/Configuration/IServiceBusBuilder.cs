namespace ServiceBus.Configuration
{
    using System;

    public interface IServiceBusBuilder
    {
        IServiceBusBuilder WithHostAddress(Uri address);

        IServiceBusBuilder WithPeer(Uri address);

        IServiceBus Build();
    }
}
