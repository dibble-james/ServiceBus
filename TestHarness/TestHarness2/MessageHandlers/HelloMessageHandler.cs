// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelloMessageHandler.cs" company="James Dibble">
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

    public class HelloMessageHandler : IMessageHandler<HelloMessage>, IMessageHandler<GoodbyeMessage>
    {
        public async Task ProcessMessageAsync(Envelope<HelloMessage> envelope)
        {
            Debug.WriteLine(
                "Hello Message received [{0}], Thread Id [{1}], Content [{2}]",
                DateTime.Now,
                Thread.CurrentThread.ManagedThreadId, 
                envelope.Message.World);
        }

        public async Task ProcessMessageAsync(Envelope<GoodbyeMessage> envelope)
        {
            Debug.WriteLine(
                "Goodbye Message received in HelloMessageHandler [{0}], Thread Id [{1}], Content [{2}]", 
                DateTime.Now, 
                Thread.CurrentThread.ManagedThreadId, 
                envelope.Message.Planet);
        }
    }
}