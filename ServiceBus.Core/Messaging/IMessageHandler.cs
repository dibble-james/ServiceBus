// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    public interface IMessageHandler
    {
        string HandledMessageType { get; }
    }
}