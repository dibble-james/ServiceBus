namespace ServiceBus.Messages
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Messaging;

    public class AvailableEndpointsMessage : IMessage
    {
        public AvailableEndpointsMessage()
        {
        }

        public AvailableEndpointsMessage(SerializationInfo info, StreamingContext context)
        {
            this.Endpoints = info.GetValue("Endpoints", typeof(IList<EndpointDescriptor>)) as IList<EndpointDescriptor>;
        }

        public IList<EndpointDescriptor> Endpoints { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Endpoints", this.Endpoints);
        }
    }
}
