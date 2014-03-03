// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingConfiguration.cs" company="James Dibble">
//    Copyright 2012 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Configuration
{
    using log4net;

    internal class LoggingConfiguration : ILoggingConfiguration
    {
        internal LoggingConfiguration(ILog logger)
        {
            this.Logger = logger;
        }

        public ILog Logger { get; private set; }
    }
}