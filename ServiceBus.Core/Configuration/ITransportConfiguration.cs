namespace ServiceBus.Configuration
{
    using Transport;

    public interface ITransportConfiguration
    {
        IHostAddressConfiguration HostAddressConfiguration { get; }

        ITransporter Transporter { get; }
    }
}
