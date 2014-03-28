namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Threading.Tasks;

    internal sealed class FtpMessageSender : IFtpClient
    {
        Task Connect(Uri ftpLocation)
        {
            throw new NotImplementedException();
        }

        Task SendMessage(string serialisedMessage)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
