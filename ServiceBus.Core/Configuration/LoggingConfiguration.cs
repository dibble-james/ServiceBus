// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingConfiguration.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Configuration
{
    using log4net;

    /// <summary>
    /// Settings for logging.
    /// </summary>
    public class LoggingConfiguration : ILoggingConfiguration
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LoggingConfiguration"/> class.
        /// </summary>
        /// <param name="logger">The previously set <see cref="ILoggingConfiguration"/>.</param>
        public LoggingConfiguration(ILoggingConfiguration logger)
        {
            this.LoggingConfigurationInstance = logger;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="LoggingConfiguration"/> class.
        /// </summary>
        /// <param name="logger">The logger to use with the <see cref="IServiceBus"/>.</param>
        public LoggingConfiguration(ILog logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the logger to use with the <see cref="IServiceBus"/>.
        /// </summary>
        public ILog Logger { get; private set; }

        /// <summary>
        /// Gets the previously set <see cref="ILoggingConfiguration"/>.
        /// </summary>
        public ILoggingConfiguration LoggingConfigurationInstance { get; private set; }
    }
}