﻿namespace ServiceBus.Web.Mvc.Configuration
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using ServiceBus.Configuration;
    using ServiceBus.Queueing;
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
        /// <param name="queueManager">The embedded persisted message queue.</param>
        /// <returns>The <see cref="IHostApplicationConfiguration"/>.</returns>
        public static IHostApplicationConfiguration AsMvcServiceBus(this ITransportConfiguration transportConfiguration, RouteCollection routes, IQueueManager queueManager)
        {
            Argument.CannotBeNull(routes, "routes", "Route collection must be used to add service bus HTTP methods.");

            routes.MapRoute(
                "MessageReceive", 
                "service-bus/message", 
                new { controller = "Message", action = "Receive" },
                new[] { typeof(MessageController).Namespace });

            return new HostApplicationConfiguration(transportConfiguration, queueManager);
        }
    }
}
