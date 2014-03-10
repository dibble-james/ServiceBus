namespace TestHarness2.Messages
{
    using System.Runtime.Serialization;
    
    using ServiceBus.Messaging;

    public class NonSharedMessage : MessageBase
    {
        public NonSharedMessage()
        {
        }

        private NonSharedMessage(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}