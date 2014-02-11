namespace ServiceBus.Events
{
    using System;

    using ServiceBus.Messaging;
    using ServiceBus.Queueing;

    /// <summary>
    /// <see cref="System.EventArgs"/> for passing data when an <see cref="IMessage"/> is successfully sent.
    /// </summary>
    public class MessageSentEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message that was sent.
        /// </summary>
        public QueuedMessage MessageSent { get; set; }

        /// <summary>
        /// Gets or sets the recipient of the message.
        /// </summary>
        public IPeer Recipient { get; set; }
    }
}
