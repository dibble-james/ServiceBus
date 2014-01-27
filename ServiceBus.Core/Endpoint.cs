namespace ServiceBus
{
    using System;

    public class Endpoint : IEndpoint
    {
        public Uri EndpointAddress { get; private set; }
    }
}
