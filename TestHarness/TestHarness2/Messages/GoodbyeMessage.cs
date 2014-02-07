// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoodbyeMessage.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TestHarness2.Messages
{
    using System.Runtime.Serialization;

    using ServiceBus.Messaging;

    public class GoodbyeMessage : IMessage
    {
        public const string GoodbyeMessageType = "GoodbyeMessage";

        public string Planet { get; set; }

        public GoodbyeMessage()
        {
        }

        public GoodbyeMessage(SerializationInfo info, StreamingContext context)
        {
            this.Planet = info.GetString("Planet");
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Planet", this.Planet);
            info.AddValue("MessageType", this.MessageType);
        }

        public string MessageType
        {
            get
            {
                return GoodbyeMessageType;
            }
        }
    }
}