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
    using ServiceBus.Transport.Http.Configuration;
    using ServiceBus.Web.Mvc.Configuration;

    using TestHarness2.Controllers;
    using Microsoft.AspNet.SignalR.Hubs;
    using TestHarness.SharedMessages;
    using TestHarness2.Messages;
    using TestHarness2.MessageHandlers;
    using TestHarness2.EventHandlers;
    using ServiceBus.Queueing;
    using ServiceBus.Queueing.Ftp;

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
                        
            container.RegisterType<IQueueManager, FtpQueueManager>(new ContainerControlledLifetimeManager());

            container.RegisterType<SharedMessageHandler>();
                        
            var messageDictionary = new MessageTypeDictionary
                                    {
                                        { ServiceBus.Messaging.MessageExtensions.MessageTypeSignature<SharedMessage>(), typeof(SharedMessage) },
                                        { ServiceBus.Messaging.MessageExtensions.MessageTypeSignature<NonSharedMessage>(), typeof(NonSharedMessage) },
                                        { ServiceBus.Messaging.MessageExtensions.MessageTypeSignature<SharedEvent>(), typeof(SharedEvent) }
                                    };

            container.RegisterInstance<IMessageSerialiser>(new JsonMessageSerialiser(messageDictionary), new ContainerControlledLifetimeManager());

            var ftpClient = new FtpQueueClient();
            ftpClient.ConnectAsync(new Uri("ftp://ftp.jdibble.co.uk/site1/Personal/service-bus-ftp2/queue"), new NetworkCredential("jdibble-001", "jli798ik")).Wait();

            container.RegisterInstance<IFtpQueueClient>(ftpClient, new ContainerControlledLifetimeManager());

            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithLogger(container.Resolve<ILog>())
                    .WithHostAddress(new Uri("http://servicebus-ftp2.jdibble.co.uk"))
                    .WithHttpTransport(container.Resolve<IMessageSerialiser>())
                    .AsMvcServiceBus(RouteTable.Routes, container.Resolve<IQueueManager>())
                    .Build()
                        .WithMessageHandler(container.Resolve<SharedMessageHandler>())
                        .Subscribe(container.Resolve<SharedEventHandler>())
                        .WithPeerAsync(new Peer(new Uri("http://servicebus-ftp.jdibble.co.uk"))).Result;

            serviceBus.WithPeerAsync(new Peer(new Uri("http://servicebus.jdibble.co.uk")));
            serviceBus.WithPeerAsync(new Peer(new Uri("http://servicebus2.jdibble.co.uk")));

            container.RegisterInstance(serviceBus, new ContainerControlledLifetimeManager());

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