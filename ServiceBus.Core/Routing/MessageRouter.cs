namespace ServiceBus.Routing
{
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
            var handleEventGeneric = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == "HandleEvent" && m.IsGenericMethod)
                .MakeGenericMethod(@event.GetType());

            await Task.Factory.StartNew(() => handleEventGeneric.Invoke(this, new object[] { @event }));
        }

        private async Task HandleEvent<TEvent>(TEvent @event) where TEvent : class, IEvent<TEvent>
        {
            foreach (var eventHandler in this._eventHandlers.OfType<IEventHandler<TEvent>>())
            {
                @event.EventRaised += eventHandler.Handle;
            }

            await @event.RaiseLocalAsync();
        }
    }
}