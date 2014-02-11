namespace ServiceBus.Events
{
    using ServiceBus.Messaging;
    using System;
    using ServiceBus.Queueing;

    public class MessageSentEventArgs : EventArgs
    {
        public QueuedMessage MessageSent { get; set; }

        public IPeer Recipient { get; set; }
    }
}
