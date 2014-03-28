namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.FtpClient;
    using System.Runtime.Remoting.Messaging;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    internal sealed class FtpMessageSender : IFtpClient
    {
        private readonly FtpClient _ftpClient;

        internal FtpMessageSender()
        {
            this._ftpClient = new FtpClient();
        }

        public async Task ConnectAsync(FtpPeer peerToConnectTo)
        {
            this._ftpClient.Host = peerToConnectTo.PeerAddress.AbsoluteUri;
            this._ftpClient.Credentials = peerToConnectTo.Credentials;

            var connectTask = Task.Factory.FromAsync(
                this._ftpClient.BeginConnect,
                this._ftpClient.EndConnect,
                null);

            await connectTask;
        }

        public async Task SendMessageAsync(string serialisedMessage)
        {
            await
                Task.Factory.FromAsync(
                    this._ftpClient.BeginOpenWrite,
                    result => this.SendMessageContent(serialisedMessage, result),
                    string.Concat("/", DateTime.Now.ToFileTimeUtc(), ".msg"),
                    null);
        }

        public void Dispose()
        {
            this._ftpClient.Dispose();
        }

        private void SendMessageContent(string messageContent, IAsyncResult callbackResult)
        {
            var connection = callbackResult.AsyncState as FtpClient;

            if (connection == null)
            {
                throw new InvalidOperationException("The FtpControlConnection object is null!");
            }

            var outputStream = connection.EndOpenWrite(callbackResult);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(messageContent));

            stream.CopyTo(outputStream);

            outputStream.Dispose();
            stream.Dispose();
        }
    }
}
