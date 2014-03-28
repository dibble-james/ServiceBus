namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Threading.Tasks;

    public interface IFtpClient : IDisposable
    {
        Task Connect(Uri ftpLocation);

        Task SendMessage(string serialisedMessage);
    }
}
