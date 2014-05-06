// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Queueing.Ftp
{
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
        public static string QueueLocation(this QueuedMessage message)
        {
            var messageLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/queue/{1}.msg",
                message.Envelope.Recipient.EscapePeerAddress(),
                message.QueuedAt.ToFileTimeUtc());

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
                "{0}/sent/{1}.msg",
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