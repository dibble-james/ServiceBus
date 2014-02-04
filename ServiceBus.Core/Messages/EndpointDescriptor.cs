namespace ServiceBus.Messages
{
    using System;
    using System.Runtime.Serialization;

    public class EndpointDescriptor : ISerializable
    {
        public EndpointDescriptor()
        {
        }

        public EndpointDescriptor(SerializationInfo info, StreamingContext context)
        {
            this.EndpointAddress = new Uri(info.GetString("EndpointAddress"));
        }

        public Uri EndpointAddress { get; set; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("EndpointAddress", this.EndpointAddress.ToString());
        }
    }
}
