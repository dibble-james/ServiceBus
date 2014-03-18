// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness1.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ServiceBus;
    using TestHarness.SharedMessages;
    using TestHarness1.Messages;

    public class HomeController : Controller
    {
        private readonly IServiceBus _serviceBus;

        public HomeController(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;
        }

        public ActionResult Index()
        {
            return this.View(this._serviceBus);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SendSharedMessage()
        {
            await this._serviceBus.SendAsync(this._serviceBus.Peers.First(), new SharedMessage());

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SendNonSharedMessage()
        {
            await this._serviceBus.SendAsync(this._serviceBus.Peers.First(), new NonSharedMessage());

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> RaiseSharedEvent()
        {
            await this._serviceBus.PublishAsync(new SharedEvent());

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}