namespace ServiceBus
{
    using System;
    using System.Collections.Generic;

    public interface IPeer
    {
        Uri PeerAddress { get; }
    }
}
