namespace ServiceBus.UnitTests.Messaging.Json
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ServiceBus.Messaging;
    using ServiceBus.Messaging.Json;
    using ServiceBus.UnitTests.DummyClasses;

    [TestClass]
    public class JsonMessageSerialiserTests
    {
        private MessageTypeDictionary _messageTypeDictionary;

        [TestInitialize]
        public void TestSetup()
        {
            this._messageTypeDictionary = new MessageTypeDictionary
                                          {
                                              {
                                                  MessageExtensions
                                                  .MessageTypeSignature<DummyMessage>(),
                                                  typeof(DummyMessage)
                                              },
                                              {
                                                  MessageExtensions
                                                  .MessageTypeSignature<DummyEvent>(),
                                                  typeof(DummyEvent)
                                              }
                                          };
        }

        [TestMethod]
        public void TestDeserialise()
        {
            const string sender = "ftp://127.0.0.1:22/";
            const string recipient = "ftp://127.0.0.1/";
            const string messageProperty = "Something";
            var messageCreated = DateTime.Now;

            const string messageTemplate = @"{{""Sender"":{{""PeerAddress"":""{0}""}},""Recipient"":{{""PeerAddress"":""{1}""}},""Message"":{{""MessageType"":""{2}"",""MessageProperty"":""{3}""}},""MessageCreated"":""{4}""}}";
            
            var serialiser = new JsonMessageSerialiser(this._messageTypeDictionary);

            var messageContent = string.Format(
                messageTemplate,
                sender,
                recipient,
                MessageExtensions.MessageTypeSignature<DummyMessage>(),
                messageProperty,
                messageCreated.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"));

            var deserialised = serialiser.Deserialise(messageContent);

            Assert.IsInstanceOfType(deserialised, typeof(Envelope<DummyMessage>));
            Assert.AreEqual(sender, deserialised.Sender.PeerAddress.AbsoluteUri);
            Assert.AreEqual(recipient, deserialised.Recipient.PeerAddress.AbsoluteUri);
            Assert.AreEqual(messageCreated, deserialised.MessageCreated);
            Assert.AreEqual(messageProperty, (deserialised.Message as DummyMessage).MessageProperty);
        }

        [TestMethod]
        public void TestSerialise()
        {
            var message = new DummyMessage { MessageProperty = "Something" };

            var serialised =
                new JsonMessageSerialiser(this._messageTypeDictionary).Serialise(
                    new Envelope<DummyMessage> { Message = message });

            Assert.IsFalse(string.IsNullOrEmpty(serialised));
        }
    }
}
