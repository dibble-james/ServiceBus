// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using ServiceBus.Events;

    public interface IMessageHandler
    {
        void ProcessMessage(IMessage message);
    }

    public interface IMessageHandler<in TMessage> : IEndpoint, IMessageHandler where TMessage : class, IMessage
    {
        void ProcessMessage(TMessage message);
    }
}