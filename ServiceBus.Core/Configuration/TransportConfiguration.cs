namespace ServiceBus.Configuration
{
    using ServiceBus.Transport;

    public class TransportConfiguration : ITransportConfiguration
    {
        public TransportConfiguration(IHostAddressConfiguration hostAddress, ITransporter transporter)
        {
            this.HostAddressConfiguration = hostAddress;
            this.Transporter = transporter;
        }

        public IHostAddressConfiguration HostAddressConfiguration { get; private set; }

        public ITransporter Transporter { get; private set; }
    }
}
