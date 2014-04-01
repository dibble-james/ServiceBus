// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFtpQueueClient.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Queueing.Ftp
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using ServiceBus.Messaging;

    public interface IFtpQueueClient
    {
        Task ConnectAsync(Uri queueLocation, NetworkCredential credentials);

        Task PutMessage(Uri messageLocation, string serialisedMessage);
    }
}