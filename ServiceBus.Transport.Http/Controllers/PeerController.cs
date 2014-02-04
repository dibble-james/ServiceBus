namespace ServiceBus.Transport.Http.Controllers
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Mvc;

    public class PeerController : Controller
    {
        private readonly IServiceBus _bus;

        public PeerController(IServiceBus bus)
        {
            this._bus = bus;
        }

        public ActionResult Endpoints()
        {
            return this.Json(this._bus.LocalEndpoints, JsonRequestBehavior.AllowGet);
        }
    }
}
