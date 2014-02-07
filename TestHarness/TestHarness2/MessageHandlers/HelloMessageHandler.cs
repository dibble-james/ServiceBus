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

    using ServiceBus.Messaging;

    using TestHarness2.Messages;

    public class HelloMessageHandler : IMessageHandler<HelloMessage>, IMessageHandler<GoodbyeMessage>
    {
        public void ProcessMessage(IMessage message)
        {
            if (message is HelloMessage)
            {
                this.ProcessMessage(message as HelloMessage);   
            }

            if (message is GoodbyeMessage)
            {
                this.ProcessMessage(message as GoodbyeMessage);
            }
        }

        public void ProcessMessage(HelloMessage message)
        {
            Debug.WriteLine(
                "Hello Message received [{0}], Thread Id [{1}], Content [{2}]",
                DateTime.Now,
                Thread.CurrentThread.ManagedThreadId, 
                message.World);
        }

        public void ProcessMessage(GoodbyeMessage message)
        {
            Debug.WriteLine(
                "Goodbye Message received in HelloMessageHandler [{0}], Thread Id [{1}], Content [{2}]", 
                DateTime.Now, 
                Thread.CurrentThread.ManagedThreadId, 
                message.Planet);
        }

        public string EndpointPath
        {
            get
            {
                return "hello";
            }
        }
    }
}