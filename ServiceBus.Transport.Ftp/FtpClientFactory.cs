// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpClientFactory.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Ftp
{
    using System.Threading.Tasks;

    public class FtpClientFactory : IFtpClientFactory
    {
        public async Task<IFtpClient> ConnectAsync(FtpPeer peerToConnectTo)
        {
            var ftpClient = new FtpMessageSender();

            await ftpClient.ConnectAsync(peerToConnectTo);

            return ftpClient;
        }
    }
}