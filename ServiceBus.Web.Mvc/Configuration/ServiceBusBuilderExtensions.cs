namespace ServiceBus.Web.Mvc.Configuration
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using ServiceBus.Configuration;
    using ServiceBus.Transport.Http.Controllers;

    /// <summary>
    /// Extends for the <see cref="IHostApplicationConfiguration"/> to add an MVC application as the host.
    /// </summary>
    public static class ServiceBusBuilderExtensions
    {
        /// <summary>
        /// Set the host application as an MVC website.
        /// </summary>
        /// <param name="transportConfiguration">The transportation configuration.</param>
        /// <param name="routes">The MVC route table to add the service bus actions too.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
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
