namespace TestHarness2.MessageHandlers
{
    using System.Globalization;
    using System.Threading.Tasks;
    using log4net;
    using ServiceBus.Messaging;
    using TestHarness.SharedMessages;

    public class SharedMessageHandler : IMessageHandler<SharedMessage>
    {
        private readonly ILog _logger;

        public SharedMessageHandler(ILog logger)
        {
            this._logger = logger;
        }

        public async Task ProcessMessageAsync(Envelope<SharedMessage> message)
        {
            this._logger.InfoFormat(
                CultureInfo.CurrentCulture,
                "Shared Message Handler invoked");
        }
    }
}