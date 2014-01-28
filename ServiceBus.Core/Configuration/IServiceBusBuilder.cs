namespace ServiceBus.Configuration
{
    using Transport;
    using System;

    public interface IServiceBusBuilder
    {
        void Configure();

        IServiceBusBuilder WithTransport<TTransport>(TTransport transporter) where TTransport : ITransporter;

        IServiceBusBuilder WithHostAddress(Uri address);

        IServiceBusBuilder WithPeer(Uri address);

        IServiceBusBuilder WithLocalEndpoint<TEndpoint>(TEndpoint endpoint) where TEndpoint : IEndpoint;

        IServiceBus Build();
    }
}
