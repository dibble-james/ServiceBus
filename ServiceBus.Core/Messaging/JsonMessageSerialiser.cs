// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonMessageTransformer.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using System;

    using Newtonsoft.Json;

    public class JsonMessageSerialiser : IMessageSerialiser
    {
        private readonly MessageTypeDictionary _messageTypeDictionary;

        public JsonMessageSerialiser(MessageTypeDictionary messageTypeDictionary)
        {
            this._messageTypeDictionary = messageTypeDictionary;
        }

        public MessageTypeDictionary MessageTypeDictionary
        {
            get
            {
                return this._messageTypeDictionary;
            }
        }

        public IMessage Deserialise(string messageContent)
        {
            dynamic messageFromJson = JsonConvert.DeserializeObject(messageContent);

            var messageTypeName = messageFromJson.MessageType as string;

            if (!this._messageTypeDictionary.ContainsKey(messageTypeName))
            {
                return null;
            }

            var messageType = this._messageTypeDictionary[messageTypeName];

            return this.ConvertToMessage(messageContent, messageType);
        }

        public string Serialise<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            var asJson = JsonConvert.SerializeObject(message);

            return asJson;
        }

        private IMessage ConvertToMessage(string messageContent, Type messageType)
        {
            var method = typeof(JsonConvert).GetMethod("DeserializeObject").MakeGenericMethod(messageType);

            var message = method.Invoke(null, new object[] { messageContent });

            return message as IMessage;
        }
    }
}