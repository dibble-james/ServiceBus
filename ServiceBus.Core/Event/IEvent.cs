// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEvent.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Event
{
    using ServiceBus.Messaging;

    /// <summary>
    /// Implementing classes define an event that can be raised and subscribed to.
    /// </summary>
    public interface IEvent : IMessage
    {
    }
}