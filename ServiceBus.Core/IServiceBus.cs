namespace ServiceBus
{
    using System;
    using System.Collections.Generic;

    using global::ServiceBus.Messaging;

    public interface IServiceBus
    {
        Uri HostAddress { get; }

        IEnumerable<IPeer> Peers { get; }

        IEnumerable<IEndpoint> LocalEndpoints { get; }

        IMessageSerialiser Serialiser { get; }
    }
}
