namespace ServiceBus
{
    using System;

    public class Endpoint : IEndpoint
    {
        public Endpoint(Uri address)
        {
            this.EndpointAddress = address;
        }

        public Uri EndpointAddress { get; private set; }
    }
}
