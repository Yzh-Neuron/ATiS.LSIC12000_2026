using System.Net;

namespace ATiS.Sim.Communication
{
    public class DualUdpDataReceivedArgs : SocketDataReceivedArgs
    {
        public DualUdpDataReceivedArgs(IPEndPoint local, IPEndPoint remote, byte code, byte[] data) : base(local, remote, data)
        {
            Code = code;
        }

        public byte Code { get; }
    }
}
