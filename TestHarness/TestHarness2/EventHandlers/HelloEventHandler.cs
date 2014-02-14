// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelloEventHandler.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness2.EventHandlers
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using ServiceBus.Event;

    using TestHarness2.Events;

    public class HelloEventHandler : IEventHandler<HelloEvent>
    {
        public string EndpointPath
        {
            get
            {
                return "HelloEventHandler";
            }
        }

        public void Handle(HelloEvent @event)
        {
            Debug.WriteLine(
                "HelloEvent Received [{0}], Raised [{1}], ThreadId[{2}]",
                DateTime.Now,
                @event.TimeEventRaised,
                Thread.CurrentThread.ManagedThreadId);
        }
    }
}