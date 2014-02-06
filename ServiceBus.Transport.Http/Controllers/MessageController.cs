// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageController.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Http.Controllers
{
    using System.Net;
    using System.Web.Mvc;

    public class MessageController : ServiceBusController
    {
        private readonly IServiceBus _serviceBus;

        public MessageController(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;
        }

        [HttpPost]
        public ActionResult Recieve(string message)
        {
            var deserialisedMessage = this._serviceBus.Serialiser.Deserialise(message);

            this._serviceBus.Recieve(deserialisedMessage);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}