namespace ServiceBus.Messaging
{
    using System.Runtime.Serialization;

    public class AvailableEndpointsMessage : IMessage
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
