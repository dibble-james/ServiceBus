namespace ServiceBus.Events
{
    using System;
    using ServiceBus.Messaging;

    public class MessageRecievedEventArgs : EventArgs
    {
        public IMessage MessageRecieved { get; set; }
    }
}
