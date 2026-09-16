using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    internal static class IPEndPointExtension
    {
        public static async Task<bool> PingAsync(this IPEndPoint endPoint, int times = 4, int timeout = 1000)
        {
            while (times-- > 0)
            {
                try
                {
                    using Ping ping = new Ping();
                    PingReply reply = await ping.SendPingAsync(endPoint.Address.ToString(), timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                catch
                {
                    // ignore exceptions
                }
            }
            return false;
        }
    }
}
