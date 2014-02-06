using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestHarness2
{
    using ServiceBus;
    using ServiceBus.Configuration;
    using ServiceBus.Messaging;
    using ServiceBus.Transport.Http.Configuration;
    using ServiceBus.Web.Mvc.Configuration;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithHostAddress(new Uri("http://localhost:55033"))
                    .WithHttpTransport(new JsonMessageSerialiser())
                    .AsMvcServiceBus(RouteTable.Routes)
                    .WithLocalEndpoint(new Endpoint(new Uri("http://localhost:55033/endpoint")))
                    .Build();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Bootstrapper.Initialise(serviceBus);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var thing = Server.GetLastError();
        }
    }
}