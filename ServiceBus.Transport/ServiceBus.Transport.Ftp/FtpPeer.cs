// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpPeer.cs" company="James Dibble">
//    Copyright 2014 James Dibble
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ServiceBus.Transport.Ftp
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    /// <summary>
    /// An <see cref="IPeer"/> that is connected too via an FTP server.
    /// </summary>
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
        /// Initialises a new instance of the <see cref="Peer"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        protected FtpPeer(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets the user information to connect to this <see cref="FtpPeer"/>.
        /// </summary>
        public NetworkCredential Credentials { get; private set; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}