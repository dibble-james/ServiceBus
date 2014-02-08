namespace ServiceBus.Configuration
{
    using ServiceBus.Transport;

    public interface ITransportConfiguration
    {
        IHostAddressConfiguration HostAddressConfiguration { get; }

        ITransporter Transporter { get; }
    }
}
