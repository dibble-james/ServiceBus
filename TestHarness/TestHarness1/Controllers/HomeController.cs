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
        public async Task<ActionResult> SendSharedMessage(string message)
        {
            await this._serviceBus.SendAsync(this._serviceBus.Peers.First(), new SharedMessage());

            return this.RedirectToAction("index");
        }

        [HttpPost]
        public async Task<ActionResult> SendNonSharedMessage(string message)
        {
            await this._serviceBus.SendAsync(this._serviceBus.Peers.First(), new NonSharedMessage());

            return this.RedirectToAction("index");
        }
    }
}