namespace ServiceBus.Transport
{
    using System.Threading.Tasks;

    using Messages;
    
    public interface ITransporter
    {
        Task<AvailableEndpointsMessage> RequestEnpoints(IPeer peer);
    }
}