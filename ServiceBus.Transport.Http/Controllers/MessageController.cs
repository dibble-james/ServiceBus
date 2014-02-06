// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageController.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Http.Controllers
{
    using System.Web.Mvc;

    public class MessageController : Controller
    {
        [HttpPost]
        public ActionResult Recieve(string message)
        {
            return null;
        }
    }
}