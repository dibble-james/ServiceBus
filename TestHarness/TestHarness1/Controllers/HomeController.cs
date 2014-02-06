// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness1.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using ServiceBus;

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
        public ActionResult SendMessage(string message)
        {
            this._serviceBus.Send(this._serviceBus.Peers.First(), new HelloMessage { World = message });

            return this.RedirectToAction("index");
        }
    }
}