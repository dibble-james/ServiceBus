namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ServiceBus.Event;
    using ServiceBus.Routing;
    using ServiceBus.Messaging;
    using ServiceBus.Transport;

    public sealed class Bus : IServiceBus
    {
        private readonly IEnumerable<IPeer> _peers;
        private readonly object _peersLock;
        private readonly IEnumerable<IEndpoint> _endpoints;
        private readonly object _endpointsLock;
        private readonly ITransporter _transport;
        private readonly ICollection<IEventHandler> _eventHandlers;
        private readonly object _eventHandlersLock;

        public Bus(Uri hostAddress, ITransporter transporter, IEnumerable<IEndpoint> endpoints, IEnumerable<IPeer> peers)
        {
            this._peersLock = new object();
            this._endpointsLock = new object();
            this._eventHandlersLock = new object();

            this.HostAddress = hostAddress;
            this._endpoints = endpoints;
            this._peers = peers;
            this._transport = transporter;
        }

        public Uri HostAddress { get; private set; }

        public IEnumerable<IEndpoint> LocalEndpoints
        {
            get
            {
                lock (this._endpointsLock)
                {
                    return this._endpoints;
                }
            }
        }

        public IMessageSerialiser Serialiser
        {
            get
            {
                return this._transport.Serialiser;
            }
        }

        public void Receive(IMessage message)
        {
            MessageRouter.RouteMessage(message, this.LocalEndpoints);
        }

        public void Send(IPeer peer, IMessage message)
        {
            Task.Factory.StartNew(() => this._transport.SendMessage(peer, message));
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPeer> Peers
        {
            get
            {
                lock (this._peersLock)
                {
                    return this._peers;
                }
            }
        }
    }
}
