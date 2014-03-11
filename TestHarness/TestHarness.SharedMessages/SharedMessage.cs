namespace TestHarness.SharedMessages
{
    using System.Runtime.Serialization;
    using ServiceBus.Messaging;

    public sealed class SharedMessage : MessageBase
    {
        public SharedMessage()
        {
        }

        private SharedMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
