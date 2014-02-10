namespace ServiceBus.Queueing
{
    using ServiceBus.Messaging;
    using System;

    internal class QueuedMessage<TMessage> where TMessage : class, IMessage, new()
    {
        internal DateTime QueuedAt { get; set; }

        internal IPeer Peer { get; set; }

        internal TMessage Message { get; set; }

        internal bool HasSent { get; set; }
    }
}
