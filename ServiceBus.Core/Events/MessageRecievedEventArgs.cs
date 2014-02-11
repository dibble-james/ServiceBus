namespace ServiceBus.Events
{
    using ServiceBus.Messaging;
    using System;

    public class MessageRecievedEventArgs : EventArgs
    {
        public IMessage MessageRecieved { get; set; }
    }
}
