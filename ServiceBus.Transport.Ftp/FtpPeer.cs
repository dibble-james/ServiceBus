// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpPeer.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Net;

    [Serializable]
    public class FtpPeer : Peer
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Peer"/> class.
        /// </summary>
        /// <param name="address">The remote location of the <see cref="IPeer"/>.</param>
        /// <param name="ftpCredential">The user information to connect to this <see cref="FtpPeer"/>.</param>
        public FtpPeer(Uri address, NetworkCredential ftpCredential) : base(address)
        {
            this.Credentials = ftpCredential;
        }

        /// <summary>
        /// Gets the user information to connect to this <see cref="FtpPeer"/>.
        /// </summary>
        public NetworkCredential Credentials { get; private set; }
    }
}