using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ServiceBus;
using ServiceBus.Event;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TestHarness2.Controllers
{
    [HubName("ServiceBusHub")]
    public class ServiceBusHub : Hub
    {
        private readonly IServiceBus _serviceBus;

        public ServiceBusHub(IServiceBus serviceBus)
        {
            this._serviceBus = serviceBus;

            this._serviceBus.UnhandledExceptionOccurs += (error, method) => this.BroadCastLogEntry(new { 
                                                                                                           Time = DateTime.Now.ToString("hh:mm:ss:fff"),
                                                                                                           Type = "danger",
                                                                                                           Message = string.Format(
                                                                                                               CultureInfo.CurrentCulture,
                                                                                                               "Method [{0}] caused error [{1}]",
                                                                                                               method,
                                                                                                               error.Message)
                                                                                                       });

            this._serviceBus.Transporter.MessageFailedToSend += (error, message) => this.BroadCastLogEntry(new 
                                                                                                           {
                                                                                                               Time = DateTime.Now.ToString("hh:mm:ss:fff"),
                                                                                                               Type = "danger",
                                                                                                               Message = string.Format(
                                                                                                                   CultureInfo.CurrentCulture,
                                                                                                                   "A [{0}] message could not be sent to [{1}]",
                                                                                                                   message.Message.MessageType,
                                                                                                                   message.Peer.PeerAddress.ToString())
                                                                                                           });

            this._serviceBus.Transporter.MessageSent += message => this.BroadCastLogEntry(new
                                                                                          {
                                                                                              Time = DateTime.Now.ToString("hh:mm:ss:fff"),
                                                                                              Type = "info",
                                                                                              Message = string.Format(
                                                                                                  CultureInfo.CurrentCulture,
                                                                                                  "A [{0}] message has been sucessfully sent to [{1}]",
                                                                                                  message.Message.MessageType,
                                                                                                  message.Peer.PeerAddress.ToString())
                                                                                          });

            this._serviceBus.Transporter.MessageRecieved += message => this.BroadCastLogEntry(new
                                                                                              {
                                                                                                  Time = DateTime.Now.ToString("hh:mm:ss:fff"),
                                                                                                  Type = "info",
                                                                                                  Message = string.Format(
                                                                                                      CultureInfo.CurrentCulture,
                                                                                                      "A [{0}] message has been recieved",
                                                                                                      message.MessageType)
                                                                                              });
        }

        public string GetHostAddress()
        {
            return this._serviceBus.PeerAddress.ToString();
        }

        public IEnumerable<IPeer> GetAllPeers()
        {
            return this._serviceBus.Peers;
        }

        public void BroadCastLogEntry(dynamic logEntry)
        {
            this.Clients.All.updateLog(logEntry);
        }
    }
}