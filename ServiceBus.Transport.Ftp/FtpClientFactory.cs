// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpClientFactory.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Ftp
{
    using System.Threading.Tasks;

    /// <summary>
    /// The default implementation of the <see cref="IFtpClientFactory"/>.
    /// </summary>
    public class FtpClientFactory : IFtpClientFactory
    {
        /// <summary>
        /// Build an <see cref="IFtpClient"/> and connect to the source FTP server.
        /// </summary>
        /// <param name="peerToConnectTo">The <see cref="FtpPeer"/> to create an FTP client for.</param>
        /// <returns>An awaitable object for the Connect operation.</returns>
        public async Task<IFtpClient> ConnectAsync(FtpPeer peerToConnectTo)
        {
            var ftpClient = new FtpMessageSender();

            await ftpClient.ConnectAsync(peerToConnectTo);

            return ftpClient;
        }
    }
}