// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationExtensionMethods.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Configuration
{
    using System;

    /// <summary>
    /// Extension methods for the configuration fluid API methods.
    /// </summary>
    public static class CommonConfigurationExtensionMethods
    {
        /// <summary>
        /// Sets the address the <see cref="IServiceBus"/> will be accessible from.
        /// </summary>
        /// <param name="loggingConfiguration">The previously set logging configuration.</param>
        /// <param name="address">The address the <see cref="IServiceBus"/> will be accessible from.</param>
        /// <returns>The host address configuration.</returns>
        public static IHostAddressConfiguration WithHostAddress(this ILoggingConfiguration loggingConfiguration, Uri address)
        {
            Argument.CannotBeNull(address, "hostAddress", "The host address for the service bus cannot be null.");

            return new HostAddressConfiguration(loggingConfiguration, address);
        }
    }
}