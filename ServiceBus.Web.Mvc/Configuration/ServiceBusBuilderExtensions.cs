namespace ServiceBus.Web.Mvc.Configuration
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Transport.Http.Controllers;

    using ServiceBus.Configuration;
    using System.Web;
    using System.IO;

    public static class ServiceBusBuilderExtensions
    {
        public static IHostApplicationConfiguration AsMvcServiceBus(this ITransportConfiguration transportConfiguration, RouteCollection routes)
        {
            routes.MapRoute(
                "MessageReceive", 
                "service-bus/message", 
                new { controller = "Message", action = "Receive" },
                new string[] { typeof(MessageController).Namespace });

            var appDataPath = HttpContext.Current.Server.MapPath("~/App_Data");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            return new HostApplicationConfiguration(transportConfiguration, appDataPath);
        }
    }
}
