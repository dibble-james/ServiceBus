using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestHarness2;

[assembly: OwinStartup(typeof(TestHarness2.HubStartup))]
namespace TestHarness2
{
    public class HubStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}