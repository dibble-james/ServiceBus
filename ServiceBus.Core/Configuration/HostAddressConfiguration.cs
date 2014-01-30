namespace ServiceBus.Configuration
{
    using System;

    public class HostAddressConfiguration : IHostAddressConfiguration
    {
        public HostAddressConfiguration(Uri hostAddress)
        {
            this.HostAddress = hostAddress;
        }

        public Uri HostAddress
        {
            get; private set;
        }
    }
}
