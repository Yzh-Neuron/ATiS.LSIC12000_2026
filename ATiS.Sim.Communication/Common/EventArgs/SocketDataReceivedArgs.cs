
using System.Net;

namespace ATiS.Sim.Communication
{
    public class SocketDataReceivedArgs : SocketArgs
    {
        public byte[] Data { get; }

        public SocketDataReceivedArgs(IPEndPoint local, IPEndPoint remote, byte[] data) : base(local, remote)
        {
            Data = data;
        }
    }
}
