// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceBus.Queueing.Ftp
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A collection of helper methods for messages queuing using an FTP server.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Create a path to queue for the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The instance of <see cref="QueuedMessage"/>.</param>
        /// <returns>A path to queue for the given message.</returns>
        public static string MessageLocation(this QueuedMessage message)
        {
            return MessageLocation(message.Envelope.Recipient, message.QueuedAt);
        }

        /// <summary>
        /// Create a path to queue for the given <paramref name="peer"/> and <paramref name="messageQueuedTime"/>.
        /// </summary>
        /// <param name="peer">The peer to build the path for.</param>
        /// <param name="messageQueuedTime">The message file time to build a path for.</param>
        /// <returns>A path to queue for the given <paramref name="peer"/> and <paramref name="messageQueuedTime"/>.</returns>
        public static string MessageLocation(IPeer peer, DateTime messageQueuedTime)
        {
            var messageLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/queue/{1}",
                peer.EscapePeerAddress(),
                messageQueuedTime.ToFileTimeUtc());

            return messageLocation;
        }

        /// <summary>
        /// Build a path for the queue for a given <see cref="IPeer"/>.
        /// </summary>
        /// <param name="peer">The peer to build a path too.</param>
        /// <returns>A path for the queue for a given <see cref="IPeer"/>.</returns>
        public static string QueueLocation(this IPeer peer)
        {
            var messageLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/queue/",
                peer.EscapePeerAddress());

            return messageLocation;
        }

        /// <summary>
        /// Create a path to sent queue for the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The instance of <see cref="QueuedMessage"/>.</param>
        /// <returns>A path to sent queue for the given message.</returns>
        public static string SentLocation(this QueuedMessage message)
        {
            var messageLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/sent/{1}",
                message.Envelope.Recipient.EscapePeerAddress(),
                message.QueuedAt.ToFileTimeUtc());

            return messageLocation;
        }

        /// <summary>
        /// Create a file name friendly representation of the unique peer identifier; it's URI.
        /// </summary>
        /// <param name="peer">The <see cref="IPeer"/> instance to get the friendly representation of.</param>
        /// <returns>A file name friendly representation of the unique peer identifier; it's URI.</returns>
        public static string EscapePeerAddress(this IPeer peer)
        {
            return peer.PeerAddress.AbsoluteUri.Replace('/', '_');
        }
    }
}