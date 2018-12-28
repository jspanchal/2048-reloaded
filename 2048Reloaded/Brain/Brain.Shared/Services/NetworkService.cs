using System.Net.NetworkInformation;

namespace Brain.Services
{
    public class NetworkService : INetworkService
    {
        public bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
