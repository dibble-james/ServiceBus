namespace TestHarness1
{
    using System;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            Bootstrapper.Initialise();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new UnityHubActivator());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var thing = this.Server.GetLastError();
        }
    }
}