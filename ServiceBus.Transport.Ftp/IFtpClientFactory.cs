namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Threading.Tasks;

    public interface IFtpClientFactory
    {
        Task<IFtpClient> ConnectAsync(FtpPeer peerToConnectTo);
    }
}
