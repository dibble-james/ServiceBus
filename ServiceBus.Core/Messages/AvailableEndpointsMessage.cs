namespace ServiceBus.Messages
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Messaging;

    public class AvailableEndpointsMessage : IMessage
    {
        public IEnumerable<EndpointDescriptor> Endpoints { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
