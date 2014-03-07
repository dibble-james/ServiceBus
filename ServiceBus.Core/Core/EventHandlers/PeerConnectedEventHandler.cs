namespace ServiceBus.Core.EventHandlers
{
    using System.Threading.Tasks;

    using ServiceBus.Core.Events;
    using ServiceBus.Event;
    using ServiceBus.Messaging;

    internal sealed class PeerConnectedEventHandler : IEventHandler<PeerConnectedEvent>
    {
        private readonly IServiceBus _serviceBus;

        internal PeerConnectedEventHandler(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;
        }

        /// <summary>
        /// Invoke services to deal with this PeerConnectedEvent.
        /// </summary>
        /// <param name="envelope">The PeerConnectedEvent data.</param>
        /// <returns>An awaitable object representing the handling operation.</returns>
        public async Task ProcessMessageAsync(Envelope<PeerConnectedEvent> envelope)
        {
            await this._serviceBus.SynchroniseAsync(envelope.Message.ConnectedPeer);
        }
    }
}
