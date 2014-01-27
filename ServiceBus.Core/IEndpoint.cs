namespace ServiceBus
{
    using System;

    public interface IEndpoint
    {
        Uri EndpointAddress { get; }
    }
}
