namespace ServiceBus.Configuration
{
    using System;

    public class ServiceBusBuilder : IServiceBusBuilder
    {
        public static IServiceBusBuilder Configure()
        {
            return new ServiceBusBuilder();
        }

        public IHostAddressConfiguration WithHostAddress(Uri address)
        {
            return new HostAddressConfiguration(address);
        }
    }
}
