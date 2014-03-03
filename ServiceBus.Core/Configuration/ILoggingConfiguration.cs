// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoggingConfiguration.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Configuration
{
    using log4net;

    public interface ILoggingConfiguration
    {
        ILog Logger { get; }
    }
}