// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyMessage.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.UnitTests.DummyClasses
{
    using System.Runtime.Serialization;

    using ServiceBus.Messaging;

    public class DummyMessage : MessageBase
    {
        public DummyMessage()
        {
        }

        public DummyMessage(SerializationInfo info, StreamingContext context)
        {
            this.MessageProperty = info.GetString("MessageProperty");
        }

        public string MessageProperty { get; set; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("MessageProperty", this.MessageProperty);
        }
    }
}