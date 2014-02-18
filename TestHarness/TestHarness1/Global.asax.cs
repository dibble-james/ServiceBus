using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestHarness1
{
    using ServiceBus.Configuration;
    using ServiceBus.Messaging;
    using ServiceBus.Transport.Http.Configuration;
    using ServiceBus.Web.Mvc.Configuration;

    using TestHarness1.Messages;

    using TestHarness2;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            var messageDictionary = new MessageTypeDictionary
                                    {
                                        { HelloMessage.HelloMessageType, typeof(HelloMessage) }
                                    };
            
            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithHostAddress(new Uri("http://localhost:55001"))
                    .WithHttpTransport(new JsonMessageSerialiser(messageDictionary))
                    .AsMvcServiceBus(RouteTable.Routes)
                    .Build()
                        .WithPeerAsync(new Uri("http://localhost:55033"));

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Bootstrapper.Initialise(serviceBus.Result);
        }
    }
}