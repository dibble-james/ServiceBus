// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelloEvent.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness2.Events
{
    using System;
    using System.Runtime.Serialization;

    using ServiceBus.Event;

    public class HelloEvent : EventBase<HelloEvent>
    {
        public const string HelloEventType = "HelloEvent";

        public HelloEvent()
        {
        }

        public HelloEvent(SerializationInfo info, StreamingContext context)
        {
            this.TimeEventRaised = info.GetDateTime("TimeEventRaised");
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MessageType", this.MessageType);
            info.AddValue("TimeEventRaised", this.TimeEventRaised);
        }

        public DateTime TimeEventRaised { get; set; }

        public override string MessageType
        {
            get
            {
                return HelloEventType;
            }
        }
    }
}