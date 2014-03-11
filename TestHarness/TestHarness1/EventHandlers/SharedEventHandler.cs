namespace TestHarness1.EventHandlers
{
    using System.Threading.Tasks;
    using log4net;
    using ServiceBus.Event;
    using ServiceBus.Messaging;
    using TestHarness.SharedMessages;

    public class SharedEventHandler : IEventHandler<SharedEvent>
    {
        private ILog _logger;

        public SharedEventHandler(ILog logger)
        {
            this._logger = logger;
        }

        public async Task ProcessMessageAsync(Envelope<SharedEvent> message)
        {
            this._logger.Info("Shared Event Handler invoked on TestHarness1");
        }
    }
}