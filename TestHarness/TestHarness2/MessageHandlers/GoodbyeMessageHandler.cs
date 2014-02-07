﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoodbyeMessageHandler.cs" company="James Dibble">
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

    public class GoodbyeMessageHandler : IMessageHandler<GoodbyeMessage>
    {
        public void ProcessMessage(IMessage message)
        {
            this.ProcessMessage(message as GoodbyeMessage);
        }

        public void ProcessMessage(GoodbyeMessage message)
        {
            Debug.WriteLine(
                "Goodbye Message received in GoodbyeMessageHandler [{0}], Thread Id [{1}], Content [{2}]", 
                DateTime.Now, 
                Thread.CurrentThread.ManagedThreadId,
                message.Planet);
        }

        public string EndpointPath
        {
            get
            {
                return "goodbye";
            }
        }
    }
}