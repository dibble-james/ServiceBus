namespace ServiceBus.Configuration
{
    using System;

    public interface IServiceBusBuilder
    {
        IHostAddressConfiguration WithHostAddress(Uri address);
    }
}
