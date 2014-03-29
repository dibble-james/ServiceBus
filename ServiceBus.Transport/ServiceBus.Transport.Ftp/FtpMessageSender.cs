namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using AlexPilotti.FTPS.Client;

    internal sealed class FtpMessageSender : IFtpClient
    {
        private readonly FTPSClient _ftpClient;

        internal FtpMessageSender()
        {
            this._ftpClient = new FTPSClient();
        }

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

        public void Dispose()
        {
            this._ftpClient.Dispose();
        }
    }
}
