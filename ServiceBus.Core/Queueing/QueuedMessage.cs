namespace ServiceBus.Queueing
{
    using System;
    using ServiceBus.Messaging;

    /// <summary>
    /// A message that is has been placed into the persisted queue.
    /// </summary>
    public class QueuedMessage
    {
        /// <summary>
        /// Gets or sets the <see cref="System.DateTime"/> the message has persisted.
        /// </summary>
        public DateTime QueuedAt { get; set; }

        /// <summary>
        /// Gets or sets the message data that was queued.
        /// </summary>
        public Envelope Envelope { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="QueuedMessage"/> has been
        /// successfully received.
        /// </summary>
        public bool HasSent { get; set; }
    }
}
