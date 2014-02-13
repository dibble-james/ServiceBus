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

        internal async void RouteMessage(object sender, MessageRecievedEventArgs args)
        {
            if (args.MessageRecieved is IEvent)
            {
                await this.HandleEvent(args.MessageRecieved as IEvent);
                return;
            }

            await this.HandleMessage(args.MessageRecieved);
        }

        private async Task HandleMessage(IMessage message)
        {
            var messageHandlers = this._endpoints.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                    && i.GenericTypeArguments.Any(m => m == message.GetType())));

            var handlingTasks = messageHandlers.Select(mh => Task.Factory.StartNew(() => (mh as IMessageHandler).ProcessMessage(message)));

            await Task.WhenAll(handlingTasks);
        }

        private async Task HandleEvent(IEvent @event)
        {
            var eventHandlers = this._endpoints.Where(
                e => e.GetType().GetInterfaces()
                .Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    && i.GenericTypeArguments.Any(m => m == @event.GetType())));

            var handlingTasks = eventHandlers.Select(mh => Task.Factory.StartNew(() => (mh as IEventHandler).Handle(@event)));

            await Task.WhenAll(handlingTasks);
        }
    }
}