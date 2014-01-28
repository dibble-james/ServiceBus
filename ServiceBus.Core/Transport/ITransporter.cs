namespace ServiceBus.Transport
{
    using Messaging;
    using System.Collections.Generic;

    public interface ITransporter
    {
        AvailableEndpointsMessage RequestEnpoints(IPeer peer);
    }
}
