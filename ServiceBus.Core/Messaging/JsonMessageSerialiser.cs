// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonMessageTransformer.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    using Newtonsoft.Json;

    public class JsonMessageSerialiser : IMessageSerialiser
    {
        public TMessage Deserialise<TMessage>(string message)
        {
            return default(TMessage);
        }

        public string Serialise<TMessage>(TMessage message)
        {
            var asJson = JsonConvert.SerializeObject(message);

            return asJson;
        }
    }
}