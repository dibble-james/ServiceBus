﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageTransformer.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Messaging
{
    public interface IMessageSerialiser
    {
        MessageTypeDictionary MessageTypeDictionary { get; }

        IMessage Deserialise(string messageContent);

        string Serialise<TMessage>(TMessage message) where TMessage : class, IMessage;
    }
}