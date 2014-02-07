namespace ServiceBus
{
    using System;
    using System.Collections.Generic;

    using Messaging;

    public interface IServiceBus
    {
        Uri HostAddress { get; }

        IEnumerable<IPeer> Peers { get; }

        IEnumerable<IEndpoint> LocalEndpoints { get; }

        IMessageSerialiser Serialiser { get; }

        void Receive(IMessage message);

        void Send(IPeer peer, IMessage message);
    }
}
