// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpClientFactory.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Ftp
{
    using System.Threading.Tasks;

    /// <summary>
    /// Implementing classes define methods for creating FTP client connections.
    /// </summary>
    public interface IFtpClientFactory
    {
        /// <summary>
        /// Build an <see cref="IFtpClient"/> and connect to the source FTP server.
        /// </summary>
        /// <param name="peerToConnectTo">The <see cref="FtpPeer"/> to create an FTP client for.</param>
        /// <returns>An awaitable object for the Connect operation.</returns>
        Task<IFtpClient> ConnectAsync(FtpPeer peerToConnectTo);
    }
}
