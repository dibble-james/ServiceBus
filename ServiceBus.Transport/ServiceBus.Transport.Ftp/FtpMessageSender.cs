// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpMessageSender.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using AlexPilotti.FTPS.Client;

    /// <summary>
    /// An implementation of the <see cref="IFtpClient"/> interface.
    /// </summary>
    public sealed class FtpMessageSender : IFtpClient
    {
        private readonly FTPSClient _ftpClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="FtpMessageSender"/> class.
        /// </summary>
        /// <param name="client">An object to communicate over FTP.</param>
        public FtpMessageSender(FTPSClient client)
        {
            this._ftpClient = client;
        }
        
        /// <summary>
        /// Connect to the FTP server.
        /// </summary>
        /// <param name="peerToConnectTo">The address and credentials of the FTP server.</param>
        /// <returns>An awaitable object for the Connect operation.</returns>
        public async Task ConnectAsync(FtpPeer peerToConnectTo)
        {
            await Task.Factory.StartNew(() => this._ftpClient.Connect(
                peerToConnectTo.PeerAddress.Host,
                peerToConnectTo.PeerAddress.Port,
                peerToConnectTo.Credentials,
                ESSLSupportMode.ClearText, 
                null,
                null,
                0,
                0,
                0,
                Timeout.Infinite));
        }

        /// <summary>
        /// Transport the given <paramref name="serialisedMessage"/>.
        /// </summary>
        /// <param name="serialisedMessage">The content of the message to send.</param>
        /// <returns>An awaitable object for the Send operation.</returns>
        public async Task SendMessageAsync(string serialisedMessage)
        {
            var outputStream = this._ftpClient.PutFile(string.Concat("/", DateTime.Now.ToFileTimeUtc(), ".msg"));

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(serialisedMessage));

            await stream.CopyToAsync(outputStream);

            await stream.FlushAsync();

            await outputStream.FlushAsync();

            outputStream.Dispose();
            stream.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this._ftpClient.Dispose();
        }
    }
}
