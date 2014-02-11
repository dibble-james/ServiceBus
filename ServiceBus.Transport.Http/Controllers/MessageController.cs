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
        public ActionResult Receive(string message)
        {
            this._serviceBus.Transporter.Receive(message);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}