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
    using System.Threading.Tasks;

    using ServiceBus.Messaging;

    using TestHarness2.Events;
    using ServiceBus.Event;

    public class HelloEventHandler : IEventHandler<HelloEvent>
    {
        public async Task ProcessMessageAsync(Envelope<HelloEvent> envelope)
        {
            Debug.WriteLine(
                "HelloEvent Received [{0}], Raised [{1}], ThreadId[{2}]",
                DateTime.Now,
                envelope.Message.TimeEventRaised,
                Thread.CurrentThread.ManagedThreadId);
        }
    }
}