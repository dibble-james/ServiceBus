// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFtpQueueClient.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace ServiceBus.Queueing.Ftp
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementing classes define methods for utilising an FTP server as a queue.
    /// </summary>
    public interface IFtpQueueClient
    {
        /// <summary>
        /// Initialise a connection with the FTP server.
        /// </summary>
        /// <param name="queueLocation">The location of the FTP server queue.</param>
        /// <param name="credentials">Authentication information for the FTP server.</param>
        /// <returns>An awaitable object for the connection operation.</returns>
        Task ConnectAsync(Uri queueLocation, NetworkCredential credentials);

        /// <summary>
        /// Place a raw message onto the FTP Server.
        /// </summary>
        /// <param name="messageLocation">The relative location where the message should be placed.</param>
        /// <param name="serialisedMessage">The raw message content.</param>
        /// <returns>An awaitable object for the Put operation.</returns>
        Task PutMessage(Uri messageLocation, string serialisedMessage);

        /// <summary>
        /// Remove a message from the FTP Server.
        /// </summary>
        /// <param name="messageLocation">The relative location where the message should be placed.</param>
        /// <returns>An awaitable object for the Put operation.</returns>
        Task DeleteMessage(Uri messageLocation);

        /// <summary>
        /// Get the names of the files in a given FTP server location.
        /// </summary>
        /// <param name="messageQueue">The location of the queue to search.</param>
        /// <returns>An awaitable object for the get file listings operation.</returns>
        Task<IEnumerable<DateTime>> GetFileListings(Uri messageQueue);

        /// <summary>
        /// Retrieve a message at the given <paramref name="messageLocation"/>.
        /// </summary>
        /// <param name="messageLocation">The message to retrieve.</param>
        /// <returns>An awaitable object for the get message operation.</returns>
        Task<string> GetMessage(Uri messageLocation);
    }
}