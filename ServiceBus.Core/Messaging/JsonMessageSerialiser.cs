namespace ServiceBus.Messaging
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json;

    /// <summary>
    /// An <see cref="IMessageSerialiser"/> to transform to and from the JSON format.
    /// </summary>
    public class JsonMessageSerialiser : IMessageSerialiser
    {
        private readonly MessageTypeDictionary _messageTypeDictionary;

        /// <summary>
        /// Initialises a new instance of the <see cref="JsonMessageSerialiser"/> class.
        /// </summary>
        /// <param name="messageTypeDictionary">Message type to message key mappings.</param>
        public JsonMessageSerialiser(MessageTypeDictionary messageTypeDictionary)
        {
            this._messageTypeDictionary = messageTypeDictionary;
        }

        /// <summary>
        /// Gets the message type mappings.
        /// </summary>
        public MessageTypeDictionary MessageTypeDictionary
        {
            get
            {
                return this._messageTypeDictionary;
            }
        }

        /// <summary>
        /// Take raw message content and transform it into its concrete implementation.
        /// </summary>
        /// <param name="messageContent">The raw message content.</param>
        /// <returns>The concrete message data.</returns>
        public IMessage Deserialise(string messageContent)
        {
            dynamic messageFromJson = JsonConvert.DeserializeObject(messageContent);

            var messageTypeName = messageFromJson.MessageType.ToString();

            if (!this._messageTypeDictionary.ContainsKey(messageTypeName))
            {
                return null;
            }

            var messageType = this._messageTypeDictionary[messageTypeName];

            return this.ConvertToMessage(messageContent, messageType);
        }

        /// <summary>
        /// Transform a <typeparamref name="TMessage"/> into raw message data.
        /// </summary>
        /// <typeparam name="TMessage">The type of <see cref="IMessage"/> to transform.</typeparam>
        /// <param name="message">The message to transform.</param>
        /// <returns>The raw message data.</returns>
        public string Serialise<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            var asJson = JsonConvert.SerializeObject(message);

            return asJson;
        }

        private IMessage ConvertToMessage(string messageContent, Type messageType)
        {
            var method = 
                typeof(JsonConvert)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod)
                    .First(
                    m => m.IsGenericMethod 
                        && m.Name == "DeserializeObject" 
                        && m.GetParameters().Count() == 1
                        && m.GetParameters().Count(p => p.ParameterType == typeof(string)) == 1)
                    .MakeGenericMethod(messageType);

            var message = method.Invoke(null, new object[] { messageContent });

            return message as IMessage;
        }
    }
}