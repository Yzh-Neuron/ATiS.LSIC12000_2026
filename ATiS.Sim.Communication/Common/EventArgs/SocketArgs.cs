using System;
using System.Net;

namespace ATiS.Sim.Communication
{
    public class SocketArgs : EventArgs
    {
        public IPEndPoint LocalEndPoint { get; }

        public IPEndPoint RemoteEndPoint { get; }

        public SocketArgs(IPEndPoint local, IPEndPoint remote)
        {
            LocalEndPoint = local;
            RemoteEndPoint = remote;
        }
    }
}
