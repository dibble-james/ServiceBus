namespace TestHarness2.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using ServiceBus;

    using TestHarness.SharedMessages;

    using TestHarness2.Messages;

    public class HomeController : Controller
    {
        private readonly IServiceBus _serviceBus;

        public HomeController(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendSharedMessage()
        {
            await this._serviceBus.SendAsync(this._serviceBus.Peers.First(), new SharedMessage());

            return this.RedirectToAction(ExpressionExtensions.MethodName(() => this.Index()));
        }

        [HttpPost]
        public async Task<ActionResult> SendNonSharedMessage()
        {
            await this._serviceBus.SendAsync(this._serviceBus.Peers.First(), new NonSharedMessage());

            return this.RedirectToAction(ExpressionExtensions.MethodName(() => this.Index()));
        }

        [HttpPost]
        public async Task<ActionResult> RaiseSharedEvent()
        {
            await this._serviceBus.PublishAsync(new SharedEvent());

            return this.RedirectToAction(ExpressionExtensions.MethodName(() => this.Index()));
        }
    }
}
