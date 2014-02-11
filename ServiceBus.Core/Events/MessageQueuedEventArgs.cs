namespace ServiceBus.Events
{
    using ServiceBus.Messaging;
    using System;
    using ServiceBus.Queueing;

    public class MessageQueuedEventArgs : EventArgs
    {
        public QueuedMessage MessageQueued { get; set; }
    }
}
