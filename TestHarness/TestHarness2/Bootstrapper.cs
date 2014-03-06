using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;

namespace TestHarness2
{
    using System;
    using System.Web;
    using System.Web.Routing;

    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    using ServiceBus;
    using ServiceBus.Configuration;
    using ServiceBus.Messaging;
    using ServiceBus.Transport.Http.Configuration;
    using ServiceBus.Web.Mvc.Configuration;

    using TestHarness2.EventHandlers;
    using TestHarness2.Events;
    using TestHarness2.MessageHandlers;
    using TestHarness2.Messages;
    using TestHarness2.Controllers;
    using Microsoft.AspNet.SignalR.Hubs;

    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders();

            hierarchy.Root.Level = Level.Debug;
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);

            FileAppender fileAppender = new RollingFileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.File = HttpContext.Current.Server.MapPath("~/ServiceBusLog.xml");
            var patternLayout = new PatternLayout { ConversionPattern = "%d [%2%t] %-5p [%-10c]   %m%n%n" };
            patternLayout.ActivateOptions();

            fileAppender.Layout = patternLayout;
            fileAppender.ActivateOptions();

            BasicConfigurator.Configure(fileAppender);

            var logger = LogManager.GetLogger(typeof(IServiceBus));

            var messageDictionary = new MessageTypeDictionary
                                    {
                                        { HelloMessage.HelloMessageType, typeof(HelloMessage) },
                                        { GoodbyeMessage.GoodbyeMessageType, typeof(GoodbyeMessage) },
                                        { HelloEvent.HelloEventType, typeof(HelloEvent) }
                                    };

            var messageHandler = new HelloMessageHandler();

            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithLogger(logger)
                    .WithHostAddress(new Uri("http://localhost:55033"))
                    .WithHttpTransport(new JsonMessageSerialiser(messageDictionary))
                    .AsMvcServiceBus(RouteTable.Routes)
                    .Build()
                        .WithMessageHandler<HelloMessage>(messageHandler)
                        .WithMessageHandler<GoodbyeMessage>(messageHandler)
                        .WithMessageHandler(new GoodbyeMessageHandler())
                        .Subscribe(new HelloEventHandler())
                        .WithPeerAsync(new Uri("http://localhost:55001"));

            container.RegisterInstance(serviceBus.Result, new ContainerControlledLifetimeManager());

            container.RegisterType<ServiceBusHub>(new ContainerControlledLifetimeManager());
        }
    }

    public class UnityHubActivator : IHubActivator
    {
        public IHub Create(HubDescriptor descriptor)
        {
            return DependencyResolver.Current.GetService(descriptor.HubType) as IHub;
        }
    }
}