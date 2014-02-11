namespace ServiceBus.Events
{
    using System;
    using ServiceBus.Messaging;

    /// <summary>
    /// <see cref="System.EventArgs"/> for passing data when a message is received.
    /// </summary>
    public class MessageRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the <see cref="IMessage"/> that was received.
        /// </summary>
        public IMessage MessageRecieved { get; set; }
    }
}
