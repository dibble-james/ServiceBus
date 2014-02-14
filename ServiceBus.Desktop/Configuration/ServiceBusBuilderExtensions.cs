namespace ServiceBus.Desktop
{
    using System.Net.Http;
    using ServiceBus.Configuration;
    using Microsoft.Owin.Hosting;

    public static class ServiceBusBuilderExtensions
    {
        public static IHostApplicationConfiguration AsDesktopApplication(this ITransportConfiguration transportConfiguration)
        {
            WebApp.Start<HttpServiceBusServer>(transportConfiguration.HostAddressConfiguration.HostAddress.AbsoluteUri);

            return new HostApplicationConfiguration(transportConfiguration);
        }
    }
}
