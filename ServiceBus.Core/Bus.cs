﻿namespace ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ServiceBus.Event;
    using ServiceBus.Routing;
    using ServiceBus.Messaging;
    using ServiceBus.Transport;
    using ServiceBus.Queueing;

    public sealed class Bus : IServiceBus
    {
        private readonly IEnumerable<IPeer> _peers;
        private readonly object _peersLock;
        private readonly IEnumerable<IEndpoint> _endpoints;
        private readonly object _endpointsLock;
        private readonly ITransporter _transport;
        private readonly ICollection<IEventHandler> _eventHandlers;
        private readonly object _eventHandlersLock;
        private readonly IQueueManager _queueManager;

        private bool _disposed;

        public Bus(
            Uri hostAddress, 
            ITransporter transporter, 
            IQueueManager queueManager,
            IEnumerable<IEndpoint> endpoints, 
            IEnumerable<IPeer> peers, 
            ICollection<IEventHandler> eventHandlers)
        {
            this._peersLock = new object();
            this._endpointsLock = new object();
            this._eventHandlersLock = new object();

            this.HostAddress = hostAddress;
            
            this._endpoints = endpoints;
            this._eventHandlers = eventHandlers;
            this._peers = peers;

            this._transport = transporter;
            this._queueManager = queueManager;

            this._disposed = false;
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

        public void Receive(IMessage message)
        {
            if(message == null)
            {
                return;
            }

            if (message is IEvent)
            {
                MessageRouter.HandleEvent(message as IEvent, this.EventHandlers);
            }
            else
            {
                MessageRouter.RouteMessage(message, this.LocalEndpoints);   
            }
        }

        public void Send<TMessage>(IPeer peer, TMessage message) where TMessage : class, IMessage, new()
        {
            this._queueManager.Enqueue(peer, message);
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            MessageRouter.HandleEvent(@event, this.EventHandlers);

            foreach (var peer in Peers)
            {
                var peerPointer = peer;

                Task.Factory.StartNew(() => this._transport.SendMessage(peerPointer, @event));
            }
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent
        {
            this.EventHandlers.Add(eventHandler);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
                this.Dispose(true);
            }
        }

        private ICollection<IEventHandler> EventHandlers
        {
            get
            {
                lock (this._eventHandlersLock)
                {
                    return this._eventHandlers;
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this._queueManager.Dispose();

            this._transport.Dispose();

            foreach (var eventHandler in this.EventHandlers.OfType<IDisposable>())
            {
                eventHandler.Dispose();
            }

            foreach (var localEndpoint in this.LocalEndpoints.OfType<IDisposable>())
            {
                localEndpoint.Dispose();
            }

            this._disposed = true;
        }
    }
}