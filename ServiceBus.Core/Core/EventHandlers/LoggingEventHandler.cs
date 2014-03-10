// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerConnectedLoggingEventHandler.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Core.EventHandlers
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    using ServiceBus.Core.Events;
    using ServiceBus.Event;
    using ServiceBus.Messaging;    
    using ServiceBus.Queueing;

    internal sealed class LoggingEventHandler : IEventHandler<PeerConnectedEvent>
    {
        private readonly IServiceBus _serviceBus;

        internal LoggingEventHandler(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;
        }

        /// <summary>
        /// Invoke services to deal with this <see cref="ServiceBus.Core.Events.PeerConnectedEvent"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="ServiceBus.Core.Events.PeerConnectedEvent"/> data.</param>
        /// <returns>An awaitable object representing the handling operation.</returns>
        public async Task ProcessMessageAsync(Envelope<PeerConnectedEvent> envelope)
        {
            this._serviceBus.Log.InfoFormat(
                CultureInfo.CurrentCulture,
                 "Peer [{0}] connected at [{1}]",
                 envelope.Message.ConnectedPeer.PeerAddress,
                 DateTime.Now);
        }

        internal void LogMessageRecieved(EnvelopeBase envelope, string messageContent)
        {
            this._serviceBus.Log.InfoFormat(
                CultureInfo.CurrentCulture,
                "Message of type [{0}] Received at [{1}] from [{2}]",
                envelope.Message.MessageType,
                DateTime.Now,
                envelope.Sender.PeerAddress);

            this._serviceBus.Log.DebugFormat(
                CultureInfo.CurrentCulture,
                "Raw Message Content:{0},{1}{0}================{0}",
                Environment.NewLine,
                messageContent);
        }

        internal void LogMessageSent(QueuedMessage messageSent, string messageContent)
        {
            this._serviceBus.Log.InfoFormat(
                CultureInfo.CurrentCulture,
                "Message of type [{0}] was sent to [{1}] at [{2}]",
                messageSent.Envelope.Message.MessageType,
                messageSent.Envelope.Recipient.PeerAddress,
                DateTime.Now);

            this._serviceBus.Log.DebugFormat(
                CultureInfo.CurrentCulture,
                "Raw Message Content:{0},{1}{0}================{0}",
                Environment.NewLine,
                messageContent);
        }

        internal void LogMessageFailedToSend(Exception ex, QueuedMessage failedMessage)
        {
            this._serviceBus.Log.InfoFormat(
                CultureInfo.CurrentCulture,
                "Message of type [{0}] could not be sent to [{1}] at [{2}].  Reason [{3}]",
                failedMessage.Envelope.Message.MessageType,
                failedMessage.Envelope.Recipient.PeerAddress,
                DateTime.Now,
                this.GetInnerMostException(ex).Message);
        }

        internal void LogGeneralFailure(Exception ex, string methodName)
        {
            this._serviceBus.Log.ErrorFormat(
                CultureInfo.CurrentCulture,
                "Method [{0}] failed to complete at [{1}].  Reason [{2}]",
                methodName,
                DateTime.Now,
                this.GetInnerMostException(ex).Message);
        }

        internal void LogUnrecognisedMessage(string messageTypeName, string sender)
        {
            this._serviceBus.Log.ErrorFormat(
                CultureInfo.CurrentCulture,
                "A message of type [{0}] was received at [{1}] from [{2}].",
                messageTypeName,
                DateTime.Now,
                sender);
        }

        internal void LogEventPublished(IEvent @event)
        {
            this._serviceBus.Log.InfoFormat(
                CultureInfo.CurrentCulture,
                "A event of type [{0}] was published at [{1}].",
                @event.MessageType,
                DateTime.Now);
        }

        private Exception GetInnerMostException(Exception ex)
        {
            return ex.InnerException == null ? ex : this.GetInnerMostException(ex.InnerException);
        }
    }
}