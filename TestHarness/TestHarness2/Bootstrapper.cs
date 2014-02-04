using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;

namespace TestHarness2
{
    using ServiceBus;

    public static class Bootstrapper
    {
        public static IUnityContainer Initialise(IServiceBus serviceBus)
    {
      var container = BuildUnityContainer(serviceBus);

      DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      return container;
    }

        private static IUnityContainer BuildUnityContainer(IServiceBus serviceBus)
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    
            RegisterTypes(container, serviceBus);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container, IServiceBus serviceBus)
        {
            container.RegisterInstance(serviceBus);
        }
    }
}