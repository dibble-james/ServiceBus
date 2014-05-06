// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpQueueClient.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceBus.Queueing.Ftp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AlexPilotti.FTPS.Client;

    /// <summary>
    /// The default implementation of the <see cref="IFtpQueueClient"/> interface.
    /// </summary>
    public class FtpQueueClient : IFtpQueueClient
    {
        private readonly FTPSClient _ftpClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="FtpQueueClient"/> class.
        /// </summary>
        public FtpQueueClient()
        {
            this._ftpClient = new FTPSClient();
        }

        /// <summary>
        /// Initialise a connection with the FTP server.
        /// </summary>
        /// <param name="queueLocation">The location of the FTP server queue.</param>
        /// <param name="credentials">Authentication information for the FTP server.</param>
        /// <returns>An awaitable object for the connection operation.</returns>
        public async Task ConnectAsync(Uri queueLocation, NetworkCredential credentials)
        {
            await Task.Factory.StartNew(() => this._ftpClient.Connect(
                queueLocation.Host,
                queueLocation.Port,
                credentials,
                ESSLSupportMode.ClearText,
                null,
                null,
                0,
                0,
                0,
                Timeout.Infinite));
        }

        /// <summary>
        /// Place a raw message onto the FTP Server.
        /// </summary>
        /// <param name="messageLocation">The relative location where the message should be placed.</param>
        /// <param name="serialisedMessage">The raw message content.</param>
        /// <returns>An awaitable object for the Put operation.</returns>
        public async Task PutMessage(Uri messageLocation, string serialisedMessage)
        {
            using (var outputStream = this._ftpClient.PutFile(messageLocation.GetLeftPart(UriPartial.Path)))
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serialisedMessage)))
                {
                    await stream.CopyToAsync(outputStream);

                    await stream.FlushAsync();

                    await outputStream.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Remove a message from the FTP Server.
        /// </summary>
        /// <param name="messageLocation">The relative location where the message should be placed.</param>
        /// <returns>An awaitable object for the Put operation.</returns>
        public async Task DeleteMessage(Uri messageLocation)
        {
            await Task.Factory.StartNew(() => this._ftpClient.DeleteFile(messageLocation.GetLeftPart(UriPartial.Path)));
        }

        /// <summary>
        /// Get the names of the files in a given FTP server location.
        /// </summary>
        /// <param name="messageQueue">The location of the queue to search.</param>
        /// <returns>An awaitable object for the get file listings operation.</returns>
        public async Task<IEnumerable<DateTime>> GetFileListings(Uri messageQueue)
        {
            var fileNames = await Task.Factory.StartNew(() => this._ftpClient.GetDirectoryList(messageQueue.GetLeftPart(UriPartial.Path)));

            var fileNamesAsDate = fileNames.Select(name => DateTime.FromFileTimeUtc(long.Parse(name.Name)));

            return fileNamesAsDate;
        }

        /// <summary>
        /// Retrieve a message at the given <paramref name="messageLocation"/>.
        /// </summary>
        /// <param name="messageLocation">The message to retrieve.</param>
        /// <returns>An awaitable object for the get message operation.</returns>
        public Task<string> GetMessage(Uri messageLocation)
        {
            using (var inputStream = this._ftpClient.GetFile(messageLocation.GetLeftPart(UriPartial.Path)))
            {
                inputStream.Position = 0;

                using (var reader = new StreamReader(inputStream, Encoding.UTF8))
                {
                    return reader.ReadToEndAsync();
                }
            }
        }
    }
}