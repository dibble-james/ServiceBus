namespace ServiceBus.Events
{
    using System;

    using ServiceBus.Messaging;
    using ServiceBus.Queueing;

    /// <summary>
    /// <see cref="System.EventArgs"/> for passing data when an <see cref="IMessage"/> is queued.
    /// </summary>
    public class MessageQueuedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message that was queued.
        /// </summary>
        public QueuedMessage MessageQueued { get; set; }
    }
}
