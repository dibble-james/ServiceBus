// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoodbyeMessageHandler.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness2.MessageHandlers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using ServiceBus.Messaging;

    using TestHarness2.Messages;

    public class GoodbyeMessageHandler : IMessageHandler<GoodbyeMessage>
    {
        public async Task ProcessMessageAsync(Envelope<GoodbyeMessage> envelope)
        {
            Debug.WriteLine(
                "Goodbye Message received in GoodbyeMessageHandler [{0}], Thread Id [{1}], Content [{2}]", 
                DateTime.Now, 
                Thread.CurrentThread.ManagedThreadId,
                envelope.Message.Planet);
        }
    }
}