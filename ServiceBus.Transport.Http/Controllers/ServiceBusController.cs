// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerExtensions.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Http.Controllers
{
    using System.Text;
    using System.Web.Mvc;

    using ServiceBus.Messaging;

    using Newtonsoft.Json;

    public abstract class ServiceBusController : Controller
    {
        protected internal ActionResult JsonResult(IMessage message)
        {
            var json = JsonConvert.SerializeObject(message);

            return this.Content(json, "application/json", Encoding.UTF8);
        }
    }
}