// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageTransformer.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    public interface IMessageSerialiser
    {
        TMessage Deserialise<TMessage>(string message);

        string Serialise<TMessage>(TMessage message);
    }
}