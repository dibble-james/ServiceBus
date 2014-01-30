namespace ServiceBus.Web.Mvc
{
    using System.Collections.Generic;
    using System.Web.Http;

    public class PeerController : ApiController
    {
        private readonly IServiceBus _bus;

        public PeerController(IServiceBus bus)
        {
            this._bus = bus;
        }

        public IEnumerable<IEndpoint> Endpoints()
        {
            return this._bus.LocalEndpoints;
        }
    }
}
