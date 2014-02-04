namespace ServiceBus.Transport.Http.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Http;
    using System.Web.Mvc;

    using global::ServiceBus.Messages;

    using Newtonsoft.Json;

    public class PeerController : Controller
    {
        private readonly IServiceBus _bus;

        public PeerController(IServiceBus bus)
        {
            this._bus = bus;
        }

        public ActionResult Endpoints()
        {
            var endpointsMessage = new AvailableEndpointsMessage
                                   {
                                       Endpoints =
                                           this._bus.LocalEndpoints.Select(
                                               endpoint =>
                                           new EndpointDescriptor
                                           {
                                               EndpointAddress =
                                                   endpoint
                                                   .EndpointAddress
                                           }).ToList()
                                   };

            var json = JsonConvert.SerializeObject(endpointsMessage);

            return this.Content(json, "application/json", Encoding.UTF8);
        }
    }
}
