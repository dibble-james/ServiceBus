namespace ServiceBus.Transport.Http.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using ServiceBus.Messaging;

    /// <summary>
    /// A controller for managing requests for <see cref="IMessage"/>s.
    /// </summary>
    public class MessageController : Controller
    {
        private readonly IServiceBus _serviceBus;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageController"/> class.
        /// </summary>
        /// <param name="serviceBus">The <see cref="IServiceBus"/> instance.</param>
        public MessageController(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;
        }

        /// <summary>
        /// An action that takes raw message data and invokes the transportation layer.
        /// </summary>
        /// <param name="message">The raw message data.</param>
        /// <returns>An <see cref="ActionResult"/>.</returns>
        [HttpPost]
        public async Task<ActionResult> Receive(string message)
        {
            await this._serviceBus.Transporter.RecieveAsync(message);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}