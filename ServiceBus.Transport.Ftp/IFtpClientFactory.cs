namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Threading.Tasks;

    public interface IFtpClientFactory
    {
        Task<IFtpClient> Connect(Uri ftpLocation);
    }
}
