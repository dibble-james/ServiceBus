// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouter.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Routing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ServiceBus.Event;
    using ServiceBus.Events;
    using ServiceBus.Messaging;

    internal class MessageRouter
    {
        private IEnumerable<IEndpoint> _endpoints;
        private IEnumerable<IEventHandler> _eventHandlers;

        internal MessageRouter(IEnumerable<IEndpoint> endpoints, IEnumerable<IEventHandler> eventHandlers)
        {
            this._endpoints = endpoints;
            this._eventHandlers = eventHandlers;
        }

        internal void RouteMessage(object sender, MessageRecievedEventArgs args)
        {
            if (args.MessageRecieved is IEvent)
            {
                this.HandleEvent(args.MessageRecieved as IEvent);
                return;
            }

            this.HandleMessage(args.MessageRecieved);
        }

        private void HandleMessage(IMessage message)
        {
            var messageHandlers = this._endpoints.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                    && i.GenericTypeArguments.Any(m => m == message.GetType())));

            foreach (var messageHandler in messageHandlers.Select(mh => mh as IMessageHandler))
            {
                var handlerPointer = messageHandler;

                Task.Factory.StartNew(() => handlerPointer.ProcessMessage(message));
            }
        }

        private void HandleEvent(IEvent @event)
        {
            var eventHandlers = this._endpoints.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    && i.GenericTypeArguments.Any(m => m == @event.GetType())));

            foreach (var eventHandler in eventHandlers.Select(eh => eh as IEventHandler))
            {
                var handlerPointer = eventHandler;

                Task.Factory.StartNew(() => handlerPointer.Handle(@event));
            }
        }
    }
}