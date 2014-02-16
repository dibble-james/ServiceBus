// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness1.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using ServiceBus;

    using TestHarness1.Events;
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
        public async Task<ActionResult> SendMessage(string message)
        {
            await this._serviceBus.Send(this._serviceBus.Peers.First(), new HelloMessage { World = message });

            return this.RedirectToAction("index");
        }

        [HttpPost]
        public async Task<ActionResult> SendGoodbyeMessage(string message)
        {
            await this._serviceBus.Send(this._serviceBus.Peers.First(), new GoodbyeMessage { Planet = message });

            return this.RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult RaiseHelloEvent()
        {
            this._serviceBus.PublishAsync(new HelloEvent { TimeEventRaised = DateTime.Now }).RunSynchronously();

            return this.RedirectToAction("index");
        }
    }
}