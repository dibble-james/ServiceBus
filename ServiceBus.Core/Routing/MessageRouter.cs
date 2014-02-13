namespace ServiceBus.Routing
{
    using System.Collections.Generic;
    using System.Linq;
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

        internal async void RouteMessage(IMessage message)
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
            var messageHandlers = this._endpoints.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                    && i.GenericTypeArguments.Any(m => m == message.GetType())));

            var handlingTasks = messageHandlers.Select(mh => Task.Factory.StartNew(() => ((IMessageHandler)mh).ProcessMessage(message)));

            await Task.WhenAll(handlingTasks);
        }

        private async Task HandleEvent(IEvent @event)
        {
            var eventHandlers = this._eventHandlers.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    && i.GenericTypeArguments.Any(m => m == @event.GetType())));

            var handlingTasks = eventHandlers.Select(mh => Task.Factory.StartNew(() => mh.Handle(@event)));

            await Task.WhenAll(handlingTasks);
        }
    }
}