using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;

namespace TestHarness2
{
    using System;
    using System.Net;
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
    using ServiceBus.Messaging.Json;
    using ServiceBus.Queueing.Db4o;
    using ServiceBus.Transport.Http.Configuration;
    using ServiceBus.Web.Mvc.Configuration;

    using TestHarness2.Controllers;
    using Microsoft.AspNet.SignalR.Hubs;
    using TestHarness.SharedMessages;
    using TestHarness2.Messages;
    using TestHarness2.MessageHandlers;
    using TestHarness2.EventHandlers;
    using Db4objects.Db4o;
    using ServiceBus.Queueing;

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
            fileAppender.File = HttpContext.Current.Server.MapPath("~/TestHarness2.log.txt");
            var patternLayout = new PatternLayout { ConversionPattern = "%d [%2%t] %-5p [%-10c]   %m%n%n" };
            patternLayout.ActivateOptions();

            fileAppender.Layout = patternLayout;
            fileAppender.ActivateOptions();

            BasicConfigurator.Configure(fileAppender);

            var logger = LogManager.GetLogger(typeof(IServiceBus));

            container.RegisterInstance<ILog>(logger, new ContainerControlledLifetimeManager());

            container.RegisterInstance<IObjectContainer>(Db4oEmbedded.OpenFile(HttpContext.Current.Server.MapPath("~/App_Data/queue.db4o")), new ContainerControlledLifetimeManager());

            container.RegisterType<IQueueManager, Db4oQueueManager>(new ContainerControlledLifetimeManager());

            container.RegisterType<SharedMessageHandler>();
                        
            var messageDictionary = new MessageTypeDictionary
                                    {
                                        { MessageExtensions.MessageTypeSignature<SharedMessage>(), typeof(SharedMessage) },
                                        { MessageExtensions.MessageTypeSignature<NonSharedMessage>(), typeof(NonSharedMessage) },
                                        { MessageExtensions.MessageTypeSignature<SharedEvent>(), typeof(SharedEvent) }
                                    };

            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithLogger(container.Resolve<ILog>())
                    .WithHostAddress(new Uri("http://servicebus2.jdibble.co.uk"))
                    .WithHttpTransport(new JsonMessageSerialiser(messageDictionary))
                    .AsMvcServiceBus(RouteTable.Routes, container.Resolve<IQueueManager>())
                    .Build()
                        .WithMessageHandler(container.Resolve<SharedMessageHandler>())
                        .Subscribe(container.Resolve<SharedEventHandler>())
                        .WithPeerAsync(new Peer(new Uri("http://servicebus.jdibble.co.uk")));

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