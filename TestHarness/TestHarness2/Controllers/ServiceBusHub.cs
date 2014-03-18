using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ServiceBus;
using ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TestHarness2.Controllers
{
    using Db4objects.Db4o;
    using Db4objects.Db4o.Linq;

    using ServiceBus.Queueing;

    [HubName("ServiceBusHub")]
    public class ServiceBusHub : Hub
    {
        private readonly IServiceBus _serviceBus;

        public ServiceBusHub(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;

            this._serviceBus.UnhandledExceptionOccurs += (error, method) => this.BroadcastLogEntry(new { 
                                                                                                           Time = DateTime.Now.ToString("hh:mm:ss.fff"),
                                                                                                           Type = "danger",
                                                                                                           Message = string.Format(
                                                                                                               CultureInfo.CurrentCulture,
                                                                                                               "Method [{0}] caused error [{1}]",
                                                                                                               method,
                                                                                                               error.Message)
                                                                                                       });

            this._serviceBus.Transporter.MessageFailedToSend += (error, envelope) => this.BroadcastLogEntry(new 
                                                                                                           {
                                                                                                               Time = DateTime.Now.ToString("hh:mm:ss.fff"),
                                                                                                               Type = "danger",
                                                                                                               Message = string.Format(
                                                                                                                   CultureInfo.CurrentCulture,
                                                                                                                   "A [{0}] message could not be sent to [{1}]",
                                                                                                                   envelope.Envelope.Message.MessageType,
                                                                                                                   envelope.Envelope.Recipient.PeerAddress.ToString())
                                                                                                           });

            this._serviceBus.Transporter.MessageSent += (envelope, rawMessage) => this.BroadcastLogEntry(new
                                                                                          {
                                                                                              Sent = envelope.Envelope.MessageCreated.ToString("hh:mm:ss.fff"),
                                                                                              Time = DateTime.Now.ToString("hh:mm:ss.fff"),
                                                                                              Type = "info",
                                                                                              Message = string.Format(
                                                                                                  CultureInfo.CurrentCulture,
                                                                                                  "A [{0}] message has been successfully sent to [{1}]",
                                                                                                  envelope.Envelope.Message.MessageType,
                                                                                                  envelope.Envelope.Recipient.PeerAddress.ToString())
                                                                                          });

            this._serviceBus.Transporter.MessageRecieved += (envelope, rawMessage) => this.BroadcastLogEntry(new
                                                                                              {
                                                                                                  Sent = envelope.MessageCreated.ToString("hh:mm:ss.fff"),
                                                                                                  Time = DateTime.Now.ToString("hh:mm:ss.fff"),
                                                                                                  Type = "info",
                                                                                                  Message = string.Format(
                                                                                                      CultureInfo.CurrentCulture,
                                                                                                      "A [{0}] message has been received",
                                                                                                      envelope.Message.MessageType)
                                                                                              });

            this._serviceBus.Transporter.MessageSent += (message, s) => this.BroadcastMessageSent(message);

            this._serviceBus.MessageQueued += this.BroadcastMessageQueued;
        }

        public string GetHostAddress()
        {
            return this._serviceBus.PeerAddress.ToString();
        }

        public IEnumerable<IPeer> GetAllPeers()
        {
            return this._serviceBus.Peers;
        }

        public IEnumerable<QueuedMessage> GetQueuedMessages()
        {
            using (var queue = Db4oEmbedded.OpenFile(HttpContext.Current.Server.MapPath("~/App_Data/queue.db4o")))
            {
                var queuedMessages = queue.AsQueryable<QueuedMessage>().Where(qm => !qm.HasSent);
                
                return queuedMessages;   
            }
        }

        public IEnumerable<QueuedMessage> GetSentMessages()
        {
            using (var queue = Db4oEmbedded.OpenFile(HttpContext.Current.Server.MapPath("~/App_Data/queue.db4o")))
            {
                var sentMessages = queue.AsQueryable<QueuedMessage>().Where(qm => qm.HasSent);

                return sentMessages;
            }
        }

        public void BroadcastLogEntry(dynamic logEntry)
        {
            this.Clients.All.updateLog(logEntry);
        }

        public void BroadcastMessageSent(QueuedMessage sentMessage)
        {
            this.Clients.All.messageSent(sentMessage);
        }

        public void BroadcastMessageQueued(QueuedMessage queuedMessage)
        {
            this.Clients.All.messageQueued(queuedMessage);
        }
    }
}