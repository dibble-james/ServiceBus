namespace ServiceBus.Configuration
{
    using System;

    public class HostApplicationConfiguration : IHostApplicationConfiguration
    {
        private ITransportConfiguration _transportConfiguration;

        public HostApplicationConfiguration(ITransportConfiguration transportConfiguration)
        {
            this._transportConfiguration = transportConfiguration;
        }

        public IServiceBus Build()
        {
            return new ServiceBus(this._transportConfiguration.HostAddressConfiguration.HostAddress, this._transportConfiguration.Transporter, null, null);
        }

        public IHostApplicationConfiguration WithPeer(Uri peer)
        {
            return this;
        }

        public IHostApplicationConfiguration WithLocalEndpoint(IEndpoint endpoint)
        {
            return this;
        }
    }
}
