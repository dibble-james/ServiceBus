namespace ServiceBus.Configuration
{
    using Transport;
    using System;

    public interface IServiceBusBuilder
    {
        IHostAddressConfiguration WithHostAddress(Uri address);
    }
}
