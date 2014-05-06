// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFtpQueueClient.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
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
    }
}