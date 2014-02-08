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
    using ServiceBus.Messaging;

    internal static class MessageRouter
    {
        internal static void RouteMessage(IMessage message, IEnumerable<IEndpoint> endpoints)
        {
            var messageHandlers = endpoints.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType 
                    && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>) 
                    && i.GenericTypeArguments.Any(m => m == message.GetType())));

            foreach (var messageHandler in messageHandlers.Select(mh => mh as IMessageHandler))
            {
                var handler = messageHandler;

                Task.Factory.StartNew(() => handler.ProcessMessage(message));
            }
        }

        internal static void HandleEvent(IEvent @event, IEnumerable<IEventHandler> endpoints)
        {
            var eventHandlers = endpoints.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    && i.GenericTypeArguments.Any(m => m == @event.GetType())));

            foreach (var eventHandler in eventHandlers)
            {
                var handler = eventHandler;

                Task.Factory.StartNew(() => handler.Handle(@event));
            }
        }

        internal static void HandleEvent<TEvent>(TEvent @event, IEnumerable<IEventHandler> eventHandlers) where TEvent : class, IEvent
        {
            var subscribedEventHandlers = eventHandlers.OfType<IEventHandler<TEvent>>();

            foreach (var subscribedEventHandler in subscribedEventHandlers)
            {
                var eventHandlerPointer = subscribedEventHandler;

                Task.Factory.StartNew(() => eventHandlerPointer.Handle(@event));
            }
        }
    }
}