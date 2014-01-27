namespace ServiceBus.Web.Mvc
{
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class InterceptServiceBusRequestAttribute : ActionFilterAttribute
    {
        private readonly IServiceBus _bus;

        public InterceptServiceBusRequestAttribute(IServiceBus bus)
        {
            this._bus = bus;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Headers.AllKeys.Any(k => k == "X-Service-Bus"))
            {
            }
        }
    }
}
