namespace ServiceBus
{
    using System;
    using System.Collections.Generic;

    public interface IServiceBus
    {
        Uri HostAddress { get; }

        IEnumerable<IPeer> Peers { get; }

        IEnumerable<IEndpoint> LocalEndpoints { get; }
    }
}
