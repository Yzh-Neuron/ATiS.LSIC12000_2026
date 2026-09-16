using System;
using System.Net;

namespace ATiS.Sim.Communication
{
    public class SocketExceptionArgs : SocketArgs
    {
        public Exception Exception { get; }

        public SocketExceptionArgs(IPEndPoint local, IPEndPoint remote, Exception exception) : base(local, remote)
        {
            Exception = exception;
        }
    }
}
