namespace ServiceBus.Queueing
{
    using ServiceBus.Messaging;
    using System;

    internal class QueuedMessage
    {
        internal DateTime QueuedAt { get; set; }

        internal IPeer Peer { get; set; }

        internal IMessage Message { get; set; }

        internal bool HasSent { get; set; }
    }
}
