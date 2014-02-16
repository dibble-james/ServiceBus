using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestHarness2
{
    using ServiceBus.Configuration;
    using ServiceBus.Messaging;
    using ServiceBus.Transport.Http.Configuration;
    using ServiceBus.Web.Mvc.Configuration;

    using TestHarness2.EventHandlers;
    using TestHarness2.Events;
    using TestHarness2.MessageHandlers;
    using TestHarness2.Messages;

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
                                        { HelloMessage.HelloMessageType, typeof(HelloMessage) },
                                        { GoodbyeMessage.GoodbyeMessageType, typeof(GoodbyeMessage) },
                                        { HelloEvent.HelloEventType, typeof(HelloEvent) }
                                    };
            
            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithHostAddress(new Uri("http://localhost:55033"))
                    .WithHttpTransport(new JsonMessageSerialiser(messageDictionary))
                    .AsMvcServiceBus(RouteTable.Routes)
                    .WithLocalEndpoint(new HelloMessageHandler())
                    .WithLocalEndpoint(new GoodbyeMessageHandler())
                    .Subscribe(new HelloEventHandler())
                    .WithPeer(new Uri("http://localhost:55001"))
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