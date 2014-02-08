namespace ServiceBus.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using ServiceBus.Event;

    public class HostApplicationConfiguration : IHostApplicationConfiguration
    {
        private readonly ITransportConfiguration _transportConfiguration;
        private readonly ICollection<IPeer> _peers;
        private readonly ICollection<IEndpoint> _endpoints;
        private readonly ICollection<IEventHandler> _eventHandlers; 

        public HostApplicationConfiguration(ITransportConfiguration transportConfiguration)
        {
            this._transportConfiguration = transportConfiguration;
            this._peers = new Collection<IPeer>();
            this._endpoints = new Collection<IEndpoint>();
            this._eventHandlers = new Collection<IEventHandler>();
        }

        public IServiceBus Build()
        {
            return new Bus(this._transportConfiguration.HostAddressConfiguration.HostAddress, this._transportConfiguration.Transporter, this._endpoints, this._peers);
        }

        public IHostApplicationConfiguration WithPeer(Uri peer)
        {
            this._peers.Add(new Peer(peer));

            return this;
        }

        public IHostApplicationConfiguration WithLocalEndpoint(IEndpoint endpoint)
        {
            this._endpoints.Add(endpoint);

            return this;
        }

        public IHostApplicationConfiguration Subscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEvent
        {
            
        }
    }
}
