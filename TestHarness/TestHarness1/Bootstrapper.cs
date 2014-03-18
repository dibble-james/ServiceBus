using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;

namespace TestHarness2
{
    using System;
    using System.Web;
    using System.Web.Routing;
    using Db4objects.Db4o;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;
    using ServiceBus;
    using ServiceBus.Configuration;
    using ServiceBus.Messaging;
    using ServiceBus.Queueing;
    using ServiceBus.Transport.Http.Configuration;
    using ServiceBus.Web.Mvc.Configuration;
    using TestHarness.SharedMessages;
    using TestHarness1.EventHandlers;
    using TestHarness1.Messages;

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
            var messageDictionary = new MessageTypeDictionary
                                    {
                                        { MessageExtensions.MessageTypeSignature<SharedMessage>(), typeof(SharedMessage) },
                                        { MessageExtensions.MessageTypeSignature<NonSharedMessage>(), typeof(NonSharedMessage) }
                                    };

            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders();

            hierarchy.Root.Level = Level.Debug;
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);

            FileAppender fileAppender = new RollingFileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.File = HttpContext.Current.Server.MapPath("~/TestHarness1.log.txt");
            var patternLayout = new PatternLayout { ConversionPattern = "%d [%2%t] %-5p [%-10c]   %m%n%n" };
            patternLayout.ActivateOptions();

            fileAppender.Layout = patternLayout;
            fileAppender.ActivateOptions();

            BasicConfigurator.Configure(fileAppender);

            var logger = LogManager.GetLogger(typeof(IServiceBus));

            container.RegisterInstance<ILog>(logger, new ContainerControlledLifetimeManager());

            container.RegisterInstance<IObjectContainer>(Db4oEmbedded.OpenFile(HttpContext.Current.Server.MapPath("~/App_Data/queue.db4o")), new ContainerControlledLifetimeManager());

            container.RegisterType<IQueueManager, QueueManager>(new ContainerControlledLifetimeManager());

            container.RegisterType<SharedEventHandler>();

            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithLogger(container.Resolve<ILog>())
                    .WithHostAddress(new Uri("http://localhost:55001"))
                    .WithHttpTransport(new JsonMessageSerialiser(messageDictionary))
                    .AsMvcServiceBus(RouteTable.Routes, container.Resolve<IQueueManager>())
                    .Build()
                        .Subscribe(container.Resolve<SharedEventHandler>())
                        .WithPeerAsync(new Uri("http://localhost:55033"));

            container.RegisterInstance(serviceBus.Result);
        }
    }
}