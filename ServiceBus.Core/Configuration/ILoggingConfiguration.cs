// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoggingConfiguration.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Configuration
{
    using log4net;

    /// <summary>
    /// Implementing classes define methods for configuring logging for the <see cref="IServiceBus"/>.
    /// </summary>
    public interface ILoggingConfiguration
    {
        /// <summary>
        /// Gets the logger to use with the <see cref="IServiceBus"/>.
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// Gets the previously set <see cref="ILoggingConfiguration"/>.
        /// </summary>
        ILoggingConfiguration LoggingConfigurationInstance { get; }
    }
}