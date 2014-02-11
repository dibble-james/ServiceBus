namespace ServiceBus.Events
{
    using System;
    using ServiceBus.Event;

    public class EventRaisedEventArgs : EventArgs
    {
        public IEvent EventRaised { get; set; }
    }
}
