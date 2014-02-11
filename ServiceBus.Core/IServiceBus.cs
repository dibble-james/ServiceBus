namespace ServiceBus
{
    using System;
    using System.Collections.Generic;

    using ServiceBus.Event;
    using ServiceBus.Messaging;
    using ServiceBus.Events;
    using ServiceBus.Transport;

    public interface IServiceBus : IDisposable
    {
        Uri HostAddress { get; }

        IEnumerable<IPeer> Peers { get; }

        IEnumerable<IEndpoint> LocalEndpoints { get; }

        IMessageSerialiser Serialiser { get; }

        ITransporter Transporter { get; }

        void Send<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new();

        void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent, new();

        void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent, new();
    }
}
