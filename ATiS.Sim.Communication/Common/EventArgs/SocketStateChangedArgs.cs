using System.Net;

namespace ATiS.Sim.Communication
{
    public class SocketStateChangedArgs : SocketArgs
    {
        public SocketStateChangedArgs(IPEndPoint local, IPEndPoint remote, bool connected) : base(local, remote)
        {
            Connected = connected;
        }

        public bool Connected { get; }
    }
}
