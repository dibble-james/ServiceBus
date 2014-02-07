// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelloMessageHandler.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness2.MessageHandlers
{
    using System;
    using System.Diagnostics;

    using ServiceBus.Messaging;

    using TestHarness2.Messages;

    public class HelloMessageHandler : IMessageHandler<HelloMessage>
    {
        public void ProcessMessage(IMessage message)
        {
            this.ProcessMessage(message as HelloMessage);
        }

        public void ProcessMessage(HelloMessage message)
        {
            Debug.WriteLine("Message received [{0}], Content [{1}]", DateTime.Now, message.World);
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