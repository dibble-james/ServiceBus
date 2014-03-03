namespace ServiceBus.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using ServiceBus.Event;
    using ServiceBus.Messaging;

    internal class MessageRouter
    {
        private readonly IEnumerable<IEndpoint> _endpoints;
        private readonly IEnumerable<IEventHandler> _eventHandlers;

        internal MessageRouter(IEnumerable<IEndpoint> endpoints, IEnumerable<IEventHandler> eventHandlers)
        {
            this._endpoints = endpoints;
            this._eventHandlers = eventHandlers;
        }

        internal async void RouteMessageAsync(IMessage message)
        {
            if (message is IEvent)
            {
                await this.HandleEvent(message as IEvent);
                return;
            }

            await this.HandleMessage(message);
        }

        private async Task HandleMessage(IMessage message)
        {
            var handleMessageGeneric = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == ExpressionExtensions.MethodName(() => this.HandleMessage(message)) && m.IsGenericMethod)
                .MakeGenericMethod(message.GetType());

            await Task.Factory.StartNew(() => handleMessageGeneric.Invoke(this, new object[] { message }));
        }

        private async Task HandleMessage<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            var handlingTasks = this._endpoints
                    .OfType<IMessageHandler<TMessage>>()
                    .Select(mh => mh.ProcessMessageAsync(message));

            await Task.WhenAll(handlingTasks);
        }

        private async Task HandleEvent(IEvent @event)
        {
            var handleEventGeneric = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == ExpressionExtensions.MethodName(() => this.HandleEvent(@event)) && m.IsGenericMethod)
                .MakeGenericMethod(@event.GetType());

            await Task.Factory.StartNew(() => handleEventGeneric.Invoke(this, new object[] { @event }));
        }

        private async Task HandleEvent<TEvent>(TEvent @event) where TEvent : class, IEvent<TEvent>
        {
            foreach (var eventHandler in this._eventHandlers.OfType<IEventHandler<TEvent>>())
            {
                var handlerPointer = eventHandler;
                @event.EventRaised += e => handlerPointer.HandleAsync(e);
            }

            await @event.RaiseLocalAsync();
        }
    }
}