namespace ServiceBus.Events
{
    using System;
    using ServiceBus.Queueing;

    public class MessageQueuedEventArgs : EventArgs
    {
        public QueuedMessage MessageQueued { get; set; }
    }
}
