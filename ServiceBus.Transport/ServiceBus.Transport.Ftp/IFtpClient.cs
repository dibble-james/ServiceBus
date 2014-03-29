// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpPeer.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementing classes define methods for transporting messages over FTP.
    /// </summary>
    public interface IFtpClient : IDisposable
    {
        /// <summary>
        /// Connect to the FTP server.
        /// </summary>
        /// <param name="peerToConnectTo">The address and credentials of the FTP server.</param>
        /// <returns>An awaitable object for the Connect operation.</returns>
        Task ConnectAsync(FtpPeer peerToConnectTo);

        /// <summary>
        /// Transport the given <paramref name="serialisedMessage"/>.
        /// </summary>
        /// <param name="serialisedMessage">The content of the message to send.</param>
        /// <returns>An awaitable object for the Send operation.</returns>
        Task SendMessageAsync(string serialisedMessage);
    }
}
