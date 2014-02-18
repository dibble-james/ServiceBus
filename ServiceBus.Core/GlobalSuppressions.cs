// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design", 
    "CA1009:DeclareEventHandlersCorrectly", 
    Scope = "member", 
    Target = "ServiceBus.Event.IEvent`1.#EventRaised",
    Justification = "Event handler does not require EventArgs.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design", 
    "CA1009:DeclareEventHandlersCorrectly", 
    Scope = "member", 
    Target = "ServiceBus.Queueing.IQueueManager.#MessageQueued",
    Justification = "Event handler does not require EventArgs.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design", 
    "CA1009:DeclareEventHandlersCorrectly", 
    Scope = "member", 
    Target = "ServiceBus.Transport.ITransporter.#MessageRecieved",
    Justification = "Event handler does not require EventArgs.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design", 
    "CA1009:DeclareEventHandlersCorrectly", 
    Scope = "member", 
    Target = "ServiceBus.Transport.ITransporter.#MessageSent",
    Justification = "Event handler does not require EventArgs.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Usage", 
    "CA2240:ImplementISerializableCorrectly", 
    Scope = "type", 
    Target = "ServiceBus.Event.EventBase`1",
    Justification = "EventBase adds no functionality to the GetObjectData method.")]
