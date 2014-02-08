// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventHandler.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Event
{
    public interface IEventHandler
    {
        void Handle(IEvent @event);
    }

    public interface IEventHandler<in TEvent> : IEndpoint, IEventHandler where TEvent : class, IEvent
    {
        void Handle(TEvent @event);
    }
}