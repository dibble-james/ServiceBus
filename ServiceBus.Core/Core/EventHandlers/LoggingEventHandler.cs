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
        /// <param name="event">The <see cref="ServiceBus.Core.Events.PeerConnectedEvent"/> data.</param>
        /// <returns>An awaitable object representing the handling operation.</returns>
        public async Task HandleAsync(PeerConnectedEvent @event)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "Peer [{0}] connected at [{1}]",
                @event.ConnectedPeer.PeerAddress,
                DateTime.Now);

           this._serviceBus.Log.Info(message);
        }

        internal void LogMessageRecieved(EnvelopeBase envelope)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture, 
                "Message of type [{0}] Received at [{1}] from [{2}]", 
                envelope.Message.MessageType, 
                DateTime.Now,
                envelope.Sender.PeerAddress);

            this._serviceBus.Log.Info(message);
        }

        internal void LogMessageSent(QueuedMessage messageSent)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "Message of type [{0}] was sent to [{1}] at [{2}]",
                messageSent.Envelope.Message.MessageType,
                messageSent.Envelope.Recipient.PeerAddress,
                DateTime.Now);

            this._serviceBus.Log.Info(message);
        }

        internal void LogMessageFailedToSend(Exception ex, QueuedMessage failedMessage)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "Message of type [{0}] could not be sent to [{1}] at [{2}].  Reason [{3}]",
                failedMessage.Envelope.Message.MessageType,
                failedMessage.Envelope.Recipient.PeerAddress,
                DateTime.Now,
                this.GetInnerMostException(ex).Message);

            this._serviceBus.Log.Info(message);
        }

        internal void LogGeneralFailure(Exception ex, string methodName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "Method [{0}] failed to complete at [{1}].  Reason [{2}]",
                methodName,
                DateTime.Now,
                this.GetInnerMostException(ex).Message);

            this._serviceBus.Log.Error(message);
        }

        internal void LogUnrecognisedMessage(string messageTypeName, string sender)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "A message of type [{0}] was received at [{1}] from [{2}].",
                messageTypeName,
                DateTime.Now,
                sender);

            this._serviceBus.Log.Error(message);
        }

        internal void LogEventPublished(IEvent @event)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "A event of type [{0}] was published at [{1}].",
                @event.MessageType,
                DateTime.Now);

            this._serviceBus.Log.Info(message);
        }

        private Exception GetInnerMostException(Exception ex)
        {
            return ex.InnerException == null ? ex : this.GetInnerMostException(ex.InnerException);
        }
    }
}