namespace TestHarness.SharedMessages
{
    using System.Runtime.Serialization;

    using ServiceBus.Event;

    public class SharedEvent : EventBase
    {
        public SharedEvent()
        {
        }

        private SharedEvent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
