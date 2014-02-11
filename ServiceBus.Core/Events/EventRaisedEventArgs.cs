namespace ServiceBus.Events
{
    using System;
    using ServiceBus.Event;

    /// <summary>
    /// <see cref="System.EventArgs"/> for passing data when an application event is raised.
    /// </summary>
    public class EventRaisedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the event that was raised.
        /// </summary>
        public IEvent EventRaised { get; set; }
    }
}
