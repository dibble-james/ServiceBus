namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Threading.Tasks;

    public interface IFtpClient : IDisposable
    {
        Task ConnectAsync(FtpPeer peerToConnectTo);

        Task SendMessageAsync(string serialisedMessage);
    }
}
